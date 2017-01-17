using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(VA_Sphere))]
public class VA_Sphere_Editor : VA_Editor<VA_Sphere>
{
	protected override void OnInspector()
	{
		DrawDefault("SphereCollider");
		
		if (Any(t => t.SphereCollider == null))
		{
			DrawDefault("Center");
			DrawDefault("Radius");
		}
		
		DrawDefault("IsHollow");
	}
}