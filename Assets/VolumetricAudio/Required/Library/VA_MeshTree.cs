using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class VA_MeshTree
{
	[System.Serializable]
	public class Node
	{
		public Bounds Bound;
		
		public int PositiveIndex;
		
		public int NegativeIndex;
		
		public int TriangleIndex;
		
		public int TriangleCount;
	}
	
	[System.Serializable]
	public class Triangle
	{
		public Vector3 A;
		
		public Vector3 B;
		
		public Vector3 C;
		
		public Vector3 Min
		{
			get
			{
				return Vector3.Min(A, Vector3.Min(B, C));
			}
		}
		
		public Vector3 Max
		{
			get
			{
				return Vector3.Max(A, Vector3.Max(B, C));
			}
		}
		
		public float MidX
		{
			get
			{
				return (A.x + B.x + C.x) / 3.0f;
			}
		}
		
		public float MidY
		{
			get
			{
				return (A.y + B.y + C.y) / 3.0f;
			}
		}
		
		public float MidZ
		{
			get
			{
				return (A.z + B.z + C.z) / 3.0f;
			}
		}
	}
	
	public List<Node> Nodes = new List<Node>();
	
	public List<Triangle> Triangles = new List<Triangle>();
	
	private static List<Triangle> closestTriangles = new List<Triangle>();
	
	private static float closestSqrDistance;
	
	public void Clear()
	{
		Nodes.Clear();
		
		Triangles.Clear();
	}
	
	public void Bake(Mesh mesh)
	{
		Clear();
		
		if (mesh != null)
		{
			var rootNode = new Node(); Nodes.Add(rootNode);
			var tris     = GetAllTriangles(mesh);
			
			Pack(rootNode, tris);
		}
	}
	
	public Vector3 FindClosestPoint(Vector3 point)
	{
		//Check(null, Nodes[0]);return point;
		
		if (Nodes.Count > 0)
		{
			closestTriangles.Clear();
			
			closestSqrDistance = float.PositiveInfinity;
			
			Search(Nodes[0], point);
			
			var closestDistanceSq = float.PositiveInfinity;
			var closestPoint      = point;
			
			foreach (var triangle in closestTriangles)
			{
				var closePoint      = VA_Helper.ClosestPointToTriangle(triangle.A, triangle.B, triangle.C, point);
				var closeDistanceSq = (closePoint - point).sqrMagnitude;
				
				if (closeDistanceSq < closestDistanceSq)
				{
					closestDistanceSq = closeDistanceSq;
					closestPoint      = closePoint;
				}
			}
			
			if (closestTriangles.Count == 0)
			{
				Debug.Log("Fao");
			}
			
			return closestPoint;
		}
		
		return point;
	}
	
	private void Search(Node node, Vector3 point)
	{
		// Leaf?
		if (node.TriangleCount > 0)
		{
			AddToResults(node); return;
		}
		
		var nodeNear = NearSqrDistance(node, point);
		
		// Too far?
		if (nodeNear - 0.001f > closestSqrDistance)
		{
			return;
		}
		
		var nodeFar = FarSqrDistance(node, point);
		
		// Nearer than outer radius?
		if (nodeFar + 0.001f < closestSqrDistance)
		{
			closestSqrDistance = nodeFar + 0.1f;
		}
		
		if (node.PositiveIndex != 0) Search(Nodes[node.PositiveIndex], point);
		if (node.NegativeIndex != 0) Search(Nodes[node.NegativeIndex], point);
	}
	
	private float NearSqrDistance(Node node, Vector3 point)
	{
		return node.Bound.SqrDistance(point);
	}
	
	private float FarSqrDistance(Node node, Vector3 point)
	{
		var min      = node.Bound.min;
		var max      = node.Bound.max;
		var distance = 0.0f;
		
		distance = Mathf.Max(distance, (new Vector3(min.x, min.y, min.z) - point).sqrMagnitude);
		distance = Mathf.Max(distance, (new Vector3(max.x, min.y, min.z) - point).sqrMagnitude);
		distance = Mathf.Max(distance, (new Vector3(min.x, min.y, max.z) - point).sqrMagnitude);
		distance = Mathf.Max(distance, (new Vector3(max.x, min.y, max.z) - point).sqrMagnitude);
		
		distance = Mathf.Max(distance, (new Vector3(min.x, max.y, min.z) - point).sqrMagnitude);
		distance = Mathf.Max(distance, (new Vector3(max.x, max.y, min.z) - point).sqrMagnitude);
		distance = Mathf.Max(distance, (new Vector3(min.x, max.y, max.z) - point).sqrMagnitude);
		distance = Mathf.Max(distance, (new Vector3(max.x, max.y, max.z) - point).sqrMagnitude);
		
		return distance;
	}
	
	private void AddToResults(Node node)
	{
		for (var i = node.TriangleIndex; i < node.TriangleIndex + node.TriangleCount; i++)
		{
			closestTriangles.Add(Triangles[i]);
		}
	}
	
	private List<Triangle> GetAllTriangles(Mesh mesh)
	{
		var tris = new List<Triangle>();
		var positions = mesh.vertices;
		
		for (var i = 0; i < mesh.subMeshCount; i++)
		{
			switch (mesh.GetTopology(i))
			{
				case MeshTopology.Triangles:
				{
					var indices = mesh.GetTriangles(i);
					
					for (var j = 0; j < indices.Length; j += 3)
					{
						var triangle = new Triangle(); tris.Add(triangle);
						
						triangle.A = positions[indices[j + 0]];
						triangle.B = positions[indices[j + 1]];
						triangle.C = positions[indices[j + 2]];
					}
				}
				break;
			}
		}
		
		return tris;
	}
	
	private void Pack(Node node, List<Triangle> tris)
	{
		CalculateBound(node, tris);
		
		if (tris.Count < 5)
		{
			node.TriangleIndex = Triangles.Count;
			node.TriangleCount = tris.Count;
			
			Triangles.AddRange(tris);
		}
		else
		{
			var positiveTris = new List<Triangle>();
			var negativeTris = new List<Triangle>();
			var axis         = default(int);
			var pivot        = default(float);
			
			CalculateAxisAndPivot(tris, ref axis, ref pivot);
			
			// Switch axis
			switch (axis)
			{
				case 0:
				{
					foreach (var triangle in tris)
					{
						if (triangle.MidX >= pivot) positiveTris.Add(triangle); else negativeTris.Add(triangle);
					}
				}
				break;
				
				case 1:
				{
					foreach (var triangle in tris)
					{
						if (triangle.MidY >= pivot) positiveTris.Add(triangle); else negativeTris.Add(triangle);
					}
				}
				break;
				
				case 2:
				{
					foreach (var triangle in tris)
					{
						if (triangle.MidZ >= pivot) positiveTris.Add(triangle); else negativeTris.Add(triangle);
					}
				}
				break;
			}
			
			// Overlapping triangles?
			if (positiveTris.Count == 0 || negativeTris.Count == 0)
			{
				positiveTris.Clear();
				negativeTris.Clear();
				
				var split = tris.Count / 2;
				
				for (var i = 0; i < split; i++)
				{
					positiveTris.Add(tris[i]);
				}
				
				for (var i = split; i < tris.Count; i++)
				{
					negativeTris.Add(tris[i]);
				}
			}
			
			node.PositiveIndex = Nodes.Count; var positiveNode = new Node(); Nodes.Add(positiveNode); Pack(positiveNode, positiveTris);
			node.NegativeIndex = Nodes.Count; var negativeNode = new Node(); Nodes.Add(negativeNode); Pack(negativeNode, negativeTris);
		}
	}
	
	private void CalculateBound(Node node, List<Triangle> tris)
	{
		if (tris.Count > 0)
		{
			var min = tris[0].Min;
			var max = tris[0].Max;
			
			foreach (var tri in tris)
			{
				min = Vector3.Min(min, tri.Min);
				max = Vector3.Max(max, tri.Max);
			}
			
			node.Bound.SetMinMax(min, max);
		}
	}
	
	private void CalculateAxisAndPivot(List<Triangle> tris, ref int axis, ref float pivot)
	{
		var min = tris[0].Min;
		var max = tris[0].Max;
		var mid = Vector3.zero;
		
		foreach (var tri in tris)
		{
			min  = Vector3.Min(min, tri.Min);
			max  = Vector3.Max(max, tri.Max);
			mid += tri.A + tri.B + tri.C;
		}
		
		var size = max - min;
		
		if (size.x > size.y && size.x > size.z)
		{
			axis  = 0;
			pivot = VA_Helper.Divide(mid.x, tris.Count * 3.0f);
		}
		else if (size.y > size.x && size.y > size.z)
		{
			axis  = 1;
			pivot = VA_Helper.Divide(mid.y, tris.Count * 3.0f);
		}
		else
		{
			axis  = 2;
			pivot = VA_Helper.Divide(mid.z, tris.Count * 3.0f);
		}
	}
}