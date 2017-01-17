using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(VA_Box))]
public class VA_Box_Editor : VA_Editor<VA_Box>
{
	protected override void OnInspector()
	{
		DrawDefault("BoxCollider");
		
		if (Any(t => t.BoxCollider == null))
		{
			DrawDefault("Center");
			DrawDefault("Size");
		}
		
		DrawDefault("IsHollow");
	}
}