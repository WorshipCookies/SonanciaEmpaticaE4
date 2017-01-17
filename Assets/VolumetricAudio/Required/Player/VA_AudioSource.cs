using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("Volumetric Audio/VA Audio Source")]
public class VA_AudioSource : MonoBehaviour
{
	public bool Compound;
	
	public VA_Shape Shape;
	
	public List<VA_Shape> Shapes = new List<VA_Shape>();
	
	public VA_VolumetricShape ExcludedShape;
	
	public List<VA_VolumetricShape> ExcludedShapes = new List<VA_VolumetricShape>();
	
	public bool Blend;
	
	public float BlendMinDistance = 0.0f;
	
	public float BlendMaxDistance = 5.0f;
	
	public AnimationCurve BlendCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));
	
	public bool Volume;
	
	public float VolumeMinDistance = 0.0f;
	
	public float VolumeMaxDistance = 5.0f;
	
	public AnimationCurve VolumeCurve = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 0.0f));
	
	private AudioSource audioSource;
	
	public bool HasVolumetricShape
	{
		get
		{
			for (var i = Shapes.Count - 1; i >= 0; i--)
			{
				var shape = Shapes[i];
				
				if (shape != null)
				{
					var  sphereShape = shape as VA_Sphere;  if ( sphereShape != null &&  sphereShape.IsHollow == false) return true;
					var     boxShape = shape as VA_Box;     if (    boxShape != null &&     boxShape.IsHollow == false) return true;
					var capsuleShape = shape as VA_Capsule; if (capsuleShape != null && capsuleShape.IsHollow == false) return true;
					var    meshShape = shape as VA_Mesh;    if (   meshShape != null &&    meshShape.IsHollow == false) return true;
				}
			}
			
			return false;
		}
	}
	
	protected virtual void LateUpdate()
	{
		UpdateShapes();
		
		var closestDistance = float.PositiveInfinity;
		var closestShape    = default(VA_Shape);
		var closestPoint    = default(Vector3);
		
		for (var i = Shapes.Count - 1; i >= 0; i--)
		{
			var shape = Shapes[i];
			
			if (VA_Helper.Enabled(shape) == true && shape.FinalPointSet == true && shape.FinalPointDistance < closestDistance)
			{
				closestDistance = shape.FinalPointDistance;
				closestPoint    = shape.FinalPoint;
				closestShape    = shape;
			}
		}
		
		// If the closest point 
		for (var i = ExcludedShapes.Count - 1; i >= 0; i--)
		{
			var excludedShape = ExcludedShapes[i];
			
			if (VA_Helper.Enabled(excludedShape) == true && excludedShape.IsHollow == false && excludedShape.InnerPointInside == true)
			{
				if (excludedShape.OuterPointSet == true && excludedShape.OuterPointDistance > closestDistance)
				{
					closestDistance = excludedShape.OuterPointDistance;
					closestPoint    = excludedShape.OuterPoint;
					closestShape    = excludedShape;
					
					break;
				}
			}
		}
		
		if (closestShape != null)
		{
			VA_Helper.SetPosition(transform, closestPoint);
			
			if (Blend == true)
			{
				if (audioSource == null) audioSource = GetComponent<AudioSource>();
				
				if (audioSource != null)
				{
					var distance01 = Mathf.InverseLerp(BlendMinDistance, BlendMaxDistance, closestDistance);
					
					SetPanLevel(BlendCurve.Evaluate(distance01));
				}
			}
			
			if (Volume == true)
			{
				if (audioSource == null) audioSource = GetComponent<AudioSource>();
				
				if (audioSource != null)
				{
					var distance01 = Mathf.InverseLerp(VolumeMinDistance, VolumeMaxDistance, closestDistance);
					
					SetVolume(VolumeCurve.Evaluate(distance01));
				}
			}
		}
	}
	
	protected virtual void SetPanLevel(float newPanLevel)
	{
		if (audioSource == null) audioSource = GetComponent<AudioSource>();
		
		if (audioSource != null)
		{
			audioSource.spatialBlend = newPanLevel;
		}
	}
	
	protected virtual void SetVolume(float newVolume)
	{
		if (audioSource == null) audioSource = GetComponent<AudioSource>();
		
		if (audioSource != null)
		{
			audioSource.volume = newVolume;
		}
	}
	
#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		if (VA_Helper.Enabled(this) == true)
		{
			UpdateShapes();
			
			Gizmos.color = Color.red;
			
			for (var i = Shapes.Count - 1; i >= 0; i--)
			{
				var shape = Shapes[i];
				
				if (VA_Helper.Enabled(shape) == true && shape.FinalPointSet == true)
				{
					Gizmos.DrawLine(transform.position, shape.FinalPoint);
				}
			}
			
			if (Blend == true)
			{
				Gizmos.DrawWireSphere(transform.position, BlendMinDistance);
				Gizmos.DrawWireSphere(transform.position, BlendMaxDistance);
			}
			
			if (Volume == true)
			{
				Gizmos.DrawWireSphere(transform.position, VolumeMinDistance);
				Gizmos.DrawWireSphere(transform.position, VolumeMaxDistance);
			}
		}
	}
#endif
	
	private void UpdateShapes()
	{
		if (Compound == false)
		{
			if (Shapes.Count != 1)
			{
				Shapes.Clear();
				Shapes.Add(Shape);
			}
			else
			{
				Shapes[0] = Shape;
			}
			
			if (ExcludedShape != null)
			{
				if (ExcludedShapes.Count != 1)
				{
					ExcludedShapes.Clear();
					ExcludedShapes.Add(ExcludedShape);
				}
				else
				{
					ExcludedShapes[0] = ExcludedShape;
				}
			}
			else
			{
				ExcludedShapes.Clear();
			}
		}
	}
}