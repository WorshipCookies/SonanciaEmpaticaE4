using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(VA_AudioSource))]
public class VA_AudioSource_Editor : VA_Editor<VA_AudioSource>
{
	protected override void OnInspector()
	{
		DrawDefault("Compound");
		
		if (Any(t => t.Compound == false))
		{
			BeginError(Any(t => t.Shape == null));
			{
				DrawDefault("Shape");
			}
			EndError();
		}
		
		if (Any(t => t.Compound == true))
		{
			BeginError(Any(t => t.Shapes.Count == 0 || t.Shapes.Exists(s => s == null)));
			{
				DrawDefault("Shapes");
			}
			EndError();
		}
		
		if (Any(t => t.Compound == false))
		{
			DrawDefault("ExcludedShape");
		}
		
		if (Any(t => t.Compound == true))
		{
			BeginError(Any(t => t.ExcludedShapes.Exists(s => s == null)));
			{
				DrawDefault("ExcludedShapes");
			}
			EndError();
		}
		
		EditorGUILayout.Separator();
		
		DrawDefault("Blend");
		
		if (Any(t => t.Blend == true))
		{
			BeginError(Any(t => t.BlendMinDistance < 0.0f || t.BlendMinDistance > t.BlendMaxDistance));
			{
				DrawDefault("BlendMinDistance");
			}
			EndError();
			
			BeginError(Any(t => t.BlendMaxDistance < 0.0f || t.BlendMinDistance > t.BlendMaxDistance));
			{
				DrawDefault("BlendMaxDistance");
			}
			EndError();
			
			DrawDefault("BlendCurve");
		}
		
		EditorGUILayout.Separator();
		
		DrawDefault("Volume");
		
		if (Any(t => t.Volume == true))
		{
			BeginError(Any(t => t.VolumeMinDistance < 0.0f || t.VolumeMinDistance > t.VolumeMaxDistance));
			{
				DrawDefault("VolumeMinDistance");
			}
			EndError();
			
			BeginError(Any(t => t.VolumeMaxDistance < 0.0f || t.VolumeMinDistance > t.VolumeMaxDistance));
			{
				DrawDefault("VolumeMaxDistance");
			}
			EndError();
			
			DrawDefault("VolumeCurve");
		}
		
		if (Any(t => IsSoundWrong(t)))
		{
			EditorGUILayout.HelpBox("This sound's Spatial Blend isn't set to 3D, which is required if you're not using the Volume or Blend settings.", MessageType.Warning);
		}
	}
	
	private bool IsSoundWrong(VA_AudioSource a)
	{
		if (a.Volume == false && a.Blend == false)
		{
			var s = a.GetComponent<AudioSource>();
			
			if (s.spatialBlend != 1.0f)
			{
				return true;
			}
		}
		
		return false;
	}
}