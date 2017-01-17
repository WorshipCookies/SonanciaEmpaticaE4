using UnityEngine;

public class VA_PopupAttribute : PropertyAttribute
{
	public string[] Names;
	
	public VA_PopupAttribute(params string[] newNames)
	{
		Names = newNames;
	}
}