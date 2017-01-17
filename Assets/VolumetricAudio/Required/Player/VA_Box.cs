using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Volumetric Audio/VA Box")]
public class VA_Box : VA_VolumetricShape
{
	public BoxCollider BoxCollider;
	
	public Vector3 Center;
	
	public Vector3 Size = Vector3.one;
	
	public Matrix4x4 GetMatrix()
	{
		var position = transform.TransformPoint(Center);
		var rotation = transform.rotation;
		var scale    = transform.lossyScale;
		
		scale.x *= Size.x;
		scale.y *= Size.y;
		scale.z *= Size.z;
		
		return VA_Helper.TranslationMatrix(position) * VA_Helper.RotationMatrix(rotation) * VA_Helper.ScalingMatrix(scale);
	}
	
	protected virtual void Reset()
	{
		BoxCollider = GetComponent<BoxCollider>();
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
				if (LocalPointInBox(localPoint) == true)
				{
					SetInnerPoint(worldPoint, true);
					
					localPoint = SnapLocalPoint(localPoint);
					worldPoint = matrix.MultiplyPoint(localPoint);
					
					SetOuterPoint(worldPoint);
				}
				else
				{
					localPoint = ClipLocalPoint(localPoint);
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
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		}
	}
#endif
	
	private void UpdateFields()
	{
		if (BoxCollider != null)
		{
			Center = BoxCollider.center;
			Size   = BoxCollider.size;
		}
	}
	
	private bool LocalPointInBox(Vector3 localPoint)
	{
		if (localPoint.x < -0.5f) return false;
		if (localPoint.x >  0.5f) return false;
		
		if (localPoint.y < -0.5f) return false;
		if (localPoint.y >  0.5f) return false;
		
		if (localPoint.z < -0.5f) return false;
		if (localPoint.z >  0.5f) return false;
		
		return true;
	}
	
	private Vector3 SnapLocalPoint(Vector3 localPoint)
	{
		var x = Mathf.Abs(localPoint.x);
		var y = Mathf.Abs(localPoint.y);
		var z = Mathf.Abs(localPoint.z);
		
		// X largest?
		if (x > y && x > z)
		{
			localPoint *= VA_Helper.Reciprocal(x * 2.0f);
		}
		// Y largest?
		else if (y > x && y > z)
		{
			localPoint *= VA_Helper.Reciprocal(y * 2.0f);
		}
		// Z largest?
		else
		{
			localPoint *= VA_Helper.Reciprocal(z * 2.0f);
		}
		
		return localPoint;
	}
	
	private Vector3 ClipLocalPoint(Vector3 localPoint)
	{
		if (localPoint.x < -0.5f) localPoint.x = -0.5f;
		if (localPoint.x >  0.5f) localPoint.x =  0.5f;
		
		if (localPoint.y < -0.5f) localPoint.y = -0.5f;
		if (localPoint.y >  0.5f) localPoint.y =  0.5f;
		
		if (localPoint.z < -0.5f) localPoint.z = -0.5f;
		if (localPoint.z >  0.5f) localPoint.z =  0.5f;
		
		return localPoint;
	}
}