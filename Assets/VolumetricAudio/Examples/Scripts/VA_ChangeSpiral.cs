using UnityEngine;

public class VA_ChangeSpiral : MonoBehaviour
{
	public VA_Spiral Spiral;
	
	public float AngleStepA = 10.0f;
	
	public float AngleStepB = -10.0f;
	
	public float Interval = 5.0f;
	
	private float position;
	
	protected virtual void Update()
	{
		position += Time.deltaTime;
		
		if (Spiral != null && Interval > 0.0f)
		{
			Spiral.AngleStep = Mathf.Lerp(AngleStepA, AngleStepB, Mathf.PingPong(position, Interval) / Interval);
			Spiral.Regenerate();
		}
	}
}