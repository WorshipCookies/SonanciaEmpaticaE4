using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Volumetric Audio/VA Mesh")]
public class VA_Mesh : VA_VolumetricShape
{
	public MeshCollider MeshCollider;
	
	public MeshFilter MeshFilter;
	
	public Mesh Mesh;
	
	public float RaySeparation = 0.1f;
	
	[SerializeField]
	private VA_MeshTree tree = new VA_MeshTree();
	
	public bool IsBaked
	{
		get
		{
			return tree != null && tree.Nodes != null && tree.Nodes.Count > 0;
		}
	}
	
	public void ClearBake()
	{
		if (tree != null)
		{
			tree.Clear();
		}
	}
	
	public void Bake()
	{
		if (tree == null) tree = new VA_MeshTree();
		
		tree.Bake(Mesh);
	}
	
	protected virtual void Reset()
	{
		IsHollow     = true; // NOTE: This is left as true by default to prevent applying volume to meshes with holes
		MeshCollider = GetComponent<MeshCollider>();
		MeshFilter   = GetComponent<MeshFilter>();
	}
	
	protected override void LateUpdate()
	{
		base.LateUpdate();
		
		if (VA_Helper.AudioListener != null)
		{
			UpdateFields();
			
			var worldPoint = VA_Helper.AudioListener.transform.position;
			var localPoint = transform.InverseTransformPoint(worldPoint);
			
			if (Mesh != null)
			{
				if (IsHollow == true)
				{
					localPoint = SnapLocalPoint(localPoint);
					worldPoint = transform.TransformPoint(localPoint);
					
					SetOuterPoint(worldPoint);
				}
				else
				{
					if (LocalPointInMesh(localPoint, worldPoint) == true)
					{
						SetInnerPoint(worldPoint, true);
						
						localPoint = SnapLocalPoint(localPoint);
						worldPoint = transform.TransformPoint(localPoint);
						
						SetOuterPoint(worldPoint);
					}
					else
					{
						localPoint = SnapLocalPoint(localPoint);
						worldPoint = transform.TransformPoint(localPoint);
						
						SetInnerOuterPoint(worldPoint, false);
					}
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
			
			if (Mesh != null)
			{
				var positions = Mesh.vertices;
				
				Gizmos.color  = Color.red;
				Gizmos.matrix = transform.localToWorldMatrix;
				
				for (var i = 0; i < Mesh.subMeshCount; i++)
				{
					switch (Mesh.GetTopology(i))
					{
						case MeshTopology.Triangles:
						{
							var indices = Mesh.GetTriangles(i);
							
							for (var j = 0; j < indices.Length; j += 3)
							{
								var point1 = positions[indices[j + 0]];
								var point2 = positions[indices[j + 1]];
								var point3 = positions[indices[j + 2]];
								
								Gizmos.DrawLine(point1, point2);
								Gizmos.DrawLine(point2, point3);
								Gizmos.DrawLine(point3, point1);
							}
						}
						break;
					}
				}
			}
		}
	}
#endif
	
	private Vector3 FindClosestLocalPoint(Vector3 localPoint)
	{
		// Tree search?
		if (tree != null && tree.Nodes != null && tree.Nodes.Count > 0)
		{
			return tree.FindClosestPoint(localPoint);
		}
		// Linear search?
		else
		{
			return VA_MeshLinear.FindClosestPoint(Mesh, localPoint);
		}
	}
	
	private void UpdateFields()
	{
		if (MeshCollider != null)
		{
			Mesh = MeshCollider.sharedMesh;
		}
		else if (MeshFilter != null)
		{
			Mesh = MeshFilter.sharedMesh;
		}
	}
	
	private int RaycastHitCount(Vector3 origin, Vector3 direction, float separation)
	{
		var hitCount = 0;
		
		if (MeshCollider != null && separation > 0.0f)
		{
			var meshSize = Vector3.Magnitude(MeshCollider.bounds.size);
			var lengthA  = meshSize;
			var lengthB  = meshSize;
			var rayA     = new Ray(origin, direction);
			var rayB     = new Ray(origin + direction * meshSize, -direction);
			var hit      = default(RaycastHit);
			
			for (var i = 0; i < 50; i++)
			{
				if (MeshCollider.Raycast(rayA, out hit, lengthA) == true)
				{
					lengthA -= hit.distance + separation;
					
					rayA.origin = hit.point + rayA.direction * separation; hitCount += 1;
				}
				else
				{
					break;
				}
			}
			
			for (var i = 0; i < 50; i++)
			{
				if (MeshCollider.Raycast(rayB, out hit, lengthB) == true)
				{
					lengthB -= hit.distance + separation;
					
					rayB.origin = hit.point + rayB.direction * separation; hitCount += 1;
				}
				else
				{
					break;
				}
			}
		}
		
		return hitCount;
	}
	
	private bool LocalPointInMesh(Vector3 localPoint, Vector3 worldPoint)
	{
		if (Mesh.bounds.Contains(localPoint) == false) return false;
		
		var hitCount = RaycastHitCount(worldPoint, Vector3.up, RaySeparation);
		
		if (hitCount == 0 || hitCount % 2 == 0) return false;
		
		return true;
	}
	
	private Vector3 SnapLocalPoint(Vector3 localPoint)
	{
		return FindClosestLocalPoint(localPoint);
	}
}