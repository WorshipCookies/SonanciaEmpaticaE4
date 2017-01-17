using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Volumetric Audio/VA Capsule")]
public class VA_Capsule : VA_VolumetricShape
{
	public CapsuleCollider CapsuleCollider;
	
	public Vector3 Center;
	
	public float Radius = 1.0f;
	
	public float Height = 2.0f;
	
	[VA_PopupAttribute("X-Axis", "Y-Axis", "Z-Axis")]
	public int Direction = 1;
	
	private static Matrix4x4 RotationX = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, 0.0f, 90.0f), Vector3.one);
	
	private static Matrix4x4 RotationY = Matrix4x4.identity;
	
	private static Matrix4x4 RotationZ = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(90.0f, 0.0f, 0.0f), Vector3.one);
	
	public Matrix4x4 GetMatrix()
	{
		var matrix = default(Matrix4x4);
		
		switch (Direction)
		{
			case 0: matrix = RotationX; break;
			case 1: matrix = RotationY; break;
			case 2: matrix = RotationZ; break;
		}
		
		var position = transform.TransformPoint(Center);
		var rotation = transform.rotation;
		var scale    = transform.lossyScale;
		
		matrix = VA_Helper.TranslationMatrix(position) * VA_Helper.RotationMatrix(rotation) * matrix * VA_Helper.ScalingMatrix(scale);
		
		return matrix;
	}
	
	protected virtual void Reset()
	{
		CapsuleCollider = GetComponent<CapsuleCollider>();
	}
	
	protected override void LateUpdate()
	{
		base.LateUpdate();
		
		if (VA_Helper.AudioListener != null)
		{
			UpdateFields();
			
			var matrix     = GetMatrix();
			var worldPoint = VA_Helper.AudioListener.transform.position;
			var localPoint = matrix.inverse.MultiplyPoint(worldPoint);
			var halfHeight = Mathf.Max(0.0f, Height * 0.5f - Radius);
			
			if (IsHollow == true)
			{
				localPoint = SnapLocalPoint(localPoint, halfHeight);
				worldPoint = matrix.MultiplyPoint(localPoint);
				
				SetOuterPoint(worldPoint);
			}
			else
			{
				if (LocalPointInCapsule(localPoint, halfHeight) == true)
				{
					SetInnerPoint(worldPoint, true);
					
					localPoint = SnapLocalPoint(localPoint, halfHeight);
					worldPoint = matrix.MultiplyPoint(localPoint);
					
					SetOuterPoint(worldPoint);
				}
				else
				{
					localPoint = SnapLocalPoint(localPoint, halfHeight);
					worldPoint = matrix.MultiplyPoint(localPoint);
					
					SetInnerOuterPoint(worldPoint, false);
				}
			}
		}
	}
	
#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		if (VA_Helper.Enabled(this) == true)
		{
			UpdateFields();
			
			var halfHeight = Mathf.Max(0.0f, Height * 0.5f - Radius);
			var point1     = Vector3.up *  halfHeight;
			var point2     = Vector3.up * -halfHeight;
			
			Gizmos.color  = Color.red;
			Gizmos.matrix = GetMatrix();
			Gizmos.DrawWireSphere(point1, Radius);
			Gizmos.DrawWireSphere(point2, Radius);
			Gizmos.DrawLine(point1 + Vector3.right   * Radius, point2 + Vector3.right   * Radius);
			Gizmos.DrawLine(point1 - Vector3.right   * Radius, point2 - Vector3.right   * Radius);
			Gizmos.DrawLine(point1 + Vector3.forward * Radius, point2 + Vector3.forward * Radius);
			Gizmos.DrawLine(point1 - Vector3.forward * Radius, point2 - Vector3.forward * Radius);
		}
	}
#endif
	
	private void UpdateFields()
	{
		if (CapsuleCollider != null)
		{
			Center    = CapsuleCollider.center;
			Radius    = CapsuleCollider.radius;
			Height    = CapsuleCollider.height;
			Direction = CapsuleCollider.direction;
		}
	}
	
	private bool LocalPointInCapsule(Vector3 localPoint, float halfHeight)
	{
		// Top
		if (localPoint.y > halfHeight)
		{
			localPoint.y -= halfHeight;
			
			return localPoint.sqrMagnitude < Radius * Radius;
		}
		// Bottom
		else if (localPoint.y < -halfHeight)
		{
			localPoint.y += halfHeight;
			
			return localPoint.sqrMagnitude < Radius * Radius;
		}
		// Middle
		else
		{
			localPoint.y = 0.0f;
			
			return localPoint.sqrMagnitude < Radius * Radius;
		}
	}
	
	private Vector3 SnapLocalPoint(Vector3 localPoint, float halfHeight)
	{
		// Top
		if (localPoint.y > halfHeight)
		{
			localPoint.y -= halfHeight;
			
			localPoint = localPoint.normalized * Radius;
			localPoint.y += halfHeight;
		}
		// Bottom
		else if (localPoint.y < -halfHeight)
		{
			localPoint.y += halfHeight;
			
			localPoint = localPoint.normalized * Radius;
			localPoint.y -= halfHeight;
		}
		// Middle
		else
		{
			var oldY = localPoint.y; localPoint.y = 0.0f;
			
			localPoint = localPoint.normalized * Radius;
			localPoint.y = oldY;
		}
		
		return localPoint;
	}
}