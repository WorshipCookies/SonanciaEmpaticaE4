using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(VA_Capsule))]
public class VA_Capsule_Editor : VA_Editor<VA_Capsule>
{
	protected override void OnInspector()
	{
		DrawDefault("CapsuleCollider");
		
		if (Any(t => t.CapsuleCollider == null))
		{
			DrawDefault("Center");
			DrawDefault("Radius");
			DrawDefault("Height");
			DrawDefault("Direction");
		}
		
		DrawDefault("IsHollow");
	}
}