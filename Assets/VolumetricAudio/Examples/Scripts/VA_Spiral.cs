using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class VA_Spiral : MonoBehaviour
{
	public int SegmentCount = 100;
	
	public float SegmentThickness = 1.0f;
	
	public float InitialAngle;
	
	public float InitialDistance = 1.0f;
	
	public float AngleStep = 10.0f;
	
	public float DistanceStep = 0.1f;
	
	private MeshFilter meshFilter;
	
	private Mesh mesh;
	
	private Vector3[] positions;
	
	private Vector2[] uvs;
	
	private int[] indices;
	
	public void Regenerate()
	{
		if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
		
		if (mesh == null)
		{
			mesh = new Mesh();
			mesh.name      = "Spiral";
			mesh.hideFlags = HideFlags.DontSave;
		}
		
		// Prevent this from dirtying the scene when exiting play mode
		VA_Helper.StealthSet(meshFilter, mesh);
		
		if (positions == null || positions.Length != SegmentCount * 2 + 2)
		{
			positions = new Vector3[SegmentCount * 2 + 2];
		}
		
		if (uvs == null || uvs.Length != SegmentCount * 2 + 2)
		{
			uvs = new Vector2[SegmentCount * 2 + 2];
		}
		
		// Generate indices?
		if (indices == null || indices.Length != SegmentCount * 6)
		{
			indices = new int[SegmentCount * 6];
			
			for (var i = 0; i < SegmentCount; i++)
			{
				indices[i * 6 + 0] = i * 2 + 0;
				indices[i * 6 + 1] = i * 2 + 1;
				indices[i * 6 + 2] = i * 2 + 2;
				indices[i * 6 + 3] = i * 2 + 3;
				indices[i * 6 + 4] = i * 2 + 2;
				indices[i * 6 + 5] = i * 2 + 1;
			}
		}
		
		var angle    = InitialAngle;
		var distance = InitialDistance;
		
		for (var i = 0; i <= SegmentCount; i++)
		{
			positions[i * 2 + 0] = VA_Helper.SinCos(angle * Mathf.Deg2Rad) *  distance;
			positions[i * 2 + 1] = VA_Helper.SinCos(angle * Mathf.Deg2Rad) * (distance + SegmentThickness);
			
			angle    += AngleStep;
			distance += DistanceStep;
		}
		
		// Update mesh
		mesh.Clear();
		mesh.vertices  = positions;
		mesh.triangles = indices;
		mesh.uv        = uvs;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}
	
	protected virtual void Awake()
	{
		Regenerate();
	}
	
	protected virtual void OnValidate()
	{
		Regenerate();
	}
	
	protected virtual void OnDestroy()
	{
		VA_Helper.Destroy(mesh);
	}
}