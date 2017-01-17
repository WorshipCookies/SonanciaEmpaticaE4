using UnityEngine;

public abstract class VA_VolumetricShape : VA_Shape
{
	public bool IsHollow;
	
	public bool InnerPointSet;
	
	public Vector3 InnerPoint;
	
	public float InnerPointDistance;
	
	public bool InnerPointInside;
	
	public override bool FinalPointSet
	{
		get
		{
			return IsHollow == true ? OuterPointSet : InnerPointSet;
		}
	}
	
	public override Vector3 FinalPoint
	{
		get
		{
			return IsHollow == true ? OuterPoint : InnerPoint;
		}
	}
	
	public override float FinalPointDistance
	{
		get
		{
			return IsHollow == true ? OuterPointDistance : InnerPointDistance;
		}
	}
	
	public void SetInnerPoint(Vector3 newInnerPoint, bool inside)
	{
		if (VA_Helper.AudioListener != null)
		{
			InnerPointSet      = true;
			InnerPoint         = newInnerPoint;
			InnerPointDistance = Vector3.Distance(VA_Helper.AudioListener.transform.position, newInnerPoint);
			InnerPointInside   = inside;
		}
	}
	
	public void SetInnerOuterPoint(Vector3 newInnerOuterPoint, bool inside)
	{
		SetInnerPoint(newInnerOuterPoint, inside);
		SetOuterPoint(newInnerOuterPoint);
	}
	
	protected override void LateUpdate()
	{
		base.LateUpdate();
		
		InnerPointSet = false;
	}
}