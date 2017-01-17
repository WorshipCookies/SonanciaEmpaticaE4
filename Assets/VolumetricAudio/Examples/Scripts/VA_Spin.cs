using UnityEngine;

public class VA_Spin : MonoBehaviour
{
	public Vector3 DegreesPerSecond;
	
	protected virtual void Update()
	{
		transform.Rotate(DegreesPerSecond * Time.deltaTime);
	}
}