using UnityEngine;

[ExecuteInEditMode]
public class VA_Freeflight : MonoBehaviour
{
	public float LinearSpeed = 10.0f;
	
	public float AngularSpeed = 1000.0f;
	
	public Vector3 LinearVelocity;
	
	public float LinearDampening = 5.0f;
	
	public Vector3 AngularVelocity;
	
	public float AngularDampening = 5.0f;
	
	public Vector3 EulerAngles;
	
	protected virtual void Update()
	{
		if (Application.isPlaying == true)
		{
			LinearVelocity += transform.right   * Input.GetAxis("Horizontal") * LinearSpeed * Time.deltaTime;
			LinearVelocity += transform.forward * Input.GetAxis("Vertical")   * LinearSpeed * Time.deltaTime;
			
			if (Cursor.lockState == CursorLockMode.Locked && Cursor.visible == false && Input.GetMouseButton(0) == true)
			{
				AngularVelocity.y += Input.GetAxis("Mouse X") * AngularSpeed * Time.deltaTime;
				AngularVelocity.x -= Input.GetAxis("Mouse Y") * AngularSpeed * Time.deltaTime;
			}
			else
			{
				var screen = new Rect(0.0f, 0.0f, Screen.width, Screen.height);
				
				if (screen.Contains(Input.mousePosition) == true && Input.GetMouseButton(0) == true)
				{
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible   = false;
				}
				else
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible   = true;
				}
			}
			
			LinearVelocity  = VA_Helper.Dampen3( LinearVelocity, Vector3.zero,  LinearDampening, Time.deltaTime, 0.1f);
			AngularVelocity = VA_Helper.Dampen3(AngularVelocity, Vector3.zero, AngularDampening, Time.deltaTime, 0.1f);
		}
		
		EulerAngles += AngularVelocity * Time.deltaTime;
		EulerAngles.x = Mathf.Clamp(EulerAngles.x, -89.0f, 89.0f);
		
		VA_Helper.SetPosition(transform, transform.position + LinearVelocity * Time.deltaTime);
		
		VA_Helper.SetRotation(transform, Quaternion.Euler(EulerAngles));
	}
}