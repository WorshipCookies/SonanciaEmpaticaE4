using UnityEngine;

public abstract class VA_Shape : MonoBehaviour
{
	public bool OuterPointSet;
	
	public Vector3 OuterPoint;
	
	public float OuterPointDistance;
	
	public virtual bool FinalPointSet
	{
		get
		{
			return OuterPointSet;
		}
	}
	
	public virtual Vector3 FinalPoint
	{
		get
		{
			return OuterPoint;
		}
	}
	
	public virtual float FinalPointDistance
	{
		get
		{
			return OuterPointDistance;
		}
	}
	
	public void SetOuterPoint(Vector3 newOuterPoint)
	{
		if (VA_Helper.AudioListener != null)
		{
			OuterPointSet      = true;
			OuterPoint         = newOuterPoint;
			OuterPointDistance = Vector3.Distance(VA_Helper.AudioListener.transform.position, newOuterPoint);
		}
	}
	
	protected virtual void LateUpdate()
	{
		OuterPointSet = false;
	}
}