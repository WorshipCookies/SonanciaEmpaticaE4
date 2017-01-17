using UnityEngine;

public class VA_MeshLinear
{
	public static Vector3 FindClosestPoint(Mesh mesh, Vector3 point)
	{
		if (mesh != null)
		{
			var positions         = mesh.vertices;
			var closestDistanceSq = float.PositiveInfinity;
			var closestPoint      = point;
			
			for (var i = 0; i < mesh.subMeshCount; i++)
			{
				switch (mesh.GetTopology(i))
				{
					case MeshTopology.Triangles:
					{
						var indices = mesh.GetTriangles(i);
						
						for (var j = 0; j < indices.Length; j += 3)
						{
							var point1          = positions[indices[j + 0]];
							var point2          = positions[indices[j + 1]];
							var point3          = positions[indices[j + 2]];
							var closePoint      = VA_Helper.ClosestPointToTriangle(point1, point2, point3, point);
							var closeDistanceSq = (closePoint - point).sqrMagnitude;
							
							if (closeDistanceSq < closestDistanceSq)
							{
								closestDistanceSq = closeDistanceSq;
								closestPoint      = closePoint;
							}
						}
					}
					break;
				}
			}
			
			return closestPoint;
		}
		
		return point;
	}
}