using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Volumetric Audio/VA Sphere")]
public class VA_Sphere : VA_VolumetricShape
{
	public SphereCollider SphereCollider;
	
	public Vector3 Center;
	
	public float Radius = 1.0f;
	
	public Matrix4x4 GetMatrix()
	{
		var position = transform.TransformPoint(Center);
		var rotation = transform.rotation;
		var scale    = transform.lossyScale;
		
		return VA_Helper.TranslationMatrix(position) * VA_Helper.RotationMatrix(rotation) * VA_Helper.ScalingMatrix(scale);
	}
	
	protected virtual void Reset()
	{
		SphereCollider = GetComponent<SphereCollider>();
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
			
			if (IsHollow == true)
			{
				localPoint = SnapLocalPoint(localPoint);
				worldPoint = matrix.MultiplyPoint(localPoint);
				
				SetOuterPoint(worldPoint);
			}
			else
			{
				if (LocalPointInSphere(localPoint) == true)
				{
					SetInnerPoint(worldPoint, true);
					
					localPoint = SnapLocalPoint(localPoint);
					worldPoint = matrix.MultiplyPoint(localPoint);
					
					SetOuterPoint(worldPoint);
				}
				else
				{
					localPoint = SnapLocalPoint(localPoint);
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
			
			Gizmos.color  = Color.red;
			Gizmos.matrix = GetMatrix();
			Gizmos.DrawWireSphere(Vector3.zero, Radius);
		}
	}
#endif
	
	private void UpdateFields()
	{
		if (SphereCollider != null)
		{
			Center = SphereCollider.center;
			Radius = SphereCollider.radius;
		}
	}
	
	private bool LocalPointInSphere(Vector3 localPoint)
	{
		return localPoint.sqrMagnitude < Radius * Radius;
	}
	
	private Vector3 SnapLocalPoint(Vector3 localPoint)
	{
		return localPoint.normalized * Radius;
	}
}