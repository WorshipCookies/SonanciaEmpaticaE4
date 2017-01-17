using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(VA_Mesh))]
public class VA_Mesh_Editor : VA_Editor<VA_Mesh>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.MeshCollider == null && t.IsHollow == false));
		{
			DrawDefault("MeshCollider");
		}
		EndError();
		
		DrawDefault("MeshFilter");
		
		BeginError(Any(t => t.Mesh == null));
		{
			DrawDefault("Mesh");
		}
		EndError();
		
		if (Any(t => t.Mesh != null && t.Mesh.isReadable == false))
		{
			EditorGUILayout.HelpBox("This mesh is not readable.", MessageType.Error);
		}
		
		if (Any(t => t.Mesh != null && t.Mesh.vertexCount > 2000 && t.IsBaked == false))
		{
			EditorGUILayout.HelpBox("This mesh has a lot of vertices, so it may run slowly. If this mesh isn't dynamic then click Bake below.", MessageType.Warning);
		}
		
		DrawDefault("IsHollow");
		
		if (Any(t => t.IsHollow == false && t.MeshCollider == null))
		{
			EditorGUILayout.HelpBox("Non hollow meshes require a MeshCollider to be set.", MessageType.Error);
		}
		
		if (Any(t => t.IsHollow == false))
		{
			BeginError(Any(t => t.RaySeparation <= 0.0f));
			{
				DrawDefault("RaySeparation");
			}
			EndError();
		}
		
		EditorGUILayout.Separator();
		
		if (Any(t => t.Mesh != null))
		{
			var rect1 = VA_Helper.Reserve(); if (GUI.Button(rect1, "Bake Mesh") == true) Each(t => t.Bake());
		}
		
		if (Any(t => t.IsBaked == true))
		{
			var rect1 = VA_Helper.Reserve(); if (GUI.Button(rect1, "Clear Baked Mesh") == true) Each(t => t.ClearBake());
		}
	}
}