using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("Volumetric Audio/VA Path")]
public class VA_Path : VA_Shape
{
	public List<Vector3> Points = new List<Vector3>();
	
	protected override void LateUpdate()
	{
		base.LateUpdate();
		
		if (VA_Helper.AudioListener != null)
		{
			if (Points.Count > 1)
			{
				var worldPoint        = VA_Helper.AudioListener.transform.position;
				var localPoint        = transform.InverseTransformPoint(worldPoint);
				var closestDistanceSq = float.PositiveInfinity;
				var closestPoint      = Vector3.zero;
				
				for (var i = 1; i < Points.Count; i++)
				{
					var closePoint      = VA_Helper.ClosestPointToLineSegment(Points[i - 1], Points[i], localPoint);
					var closeDistanceSq = (closePoint - localPoint).sqrMagnitude;
					
					if (closeDistanceSq < closestDistanceSq)
					{
						closestDistanceSq = closeDistanceSq;
						closestPoint      = closePoint;
					}
				}
				
				worldPoint = transform.TransformPoint(closestPoint);
				
				SetOuterPoint(worldPoint);
			}
		}
	}
}