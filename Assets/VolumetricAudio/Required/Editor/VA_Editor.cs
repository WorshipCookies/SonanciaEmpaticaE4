using UnityEngine;
using UnityEditor;

public abstract class VA_Editor<T> : Editor
	where T : MonoBehaviour
{
	protected T   Target;
	protected T[] Targets;
	
	public override void OnInspectorGUI()
	{
		VA_Helper.BaseRect    = VA_Helper.Reserve(0.0f);
		VA_Helper.BaseRectSet = true;
		
		EditorGUI.BeginChangeCheck();
		
		serializedObject.UpdateIfDirtyOrScript();
		
		Target  = (T)target;
		Targets = System.Array.ConvertAll<Object, T>(targets, t => (T)t);
		
		EditorGUILayout.Separator();
		
		OnInspector();
		
		EditorGUILayout.Separator();
		
		serializedObject.ApplyModifiedProperties();
		
		if (EditorGUI.EndChangeCheck() == true)
		{
			GUI.changed = true; Repaint();
			
			foreach (var t in Targets)
			{
				VA_Helper.SetDirty(t);
			}
		}
		
		VA_Helper.BaseRectSet = false;
	}
	
	public virtual void OnSceneGUI()
	{
		Target = (T)target;
		
		OnScene();
		
		if (GUI.changed == true)
		{
			VA_Helper.SetDirty(target);
		}
	}
	
	protected void Each(System.Action<T> update)
	{
		foreach (var t in Targets)
		{
			update(t);
		}
	}
	
	protected bool Any(System.Func<T, bool> check)
	{
		foreach (var t in Targets)
		{
			if (check(t) == true)
			{
				return true;
			}
		}
		
		return false;
	}
	
	protected bool All(System.Func<T, bool> check)
	{
		foreach (var t in Targets)
		{
			if (check(t) == false)
			{
				return false;
			}
		}
		
		return true;
	}
	
	protected void BeginError(bool error = true)
	{
		EditorGUILayout.BeginVertical(error == true ? VA_Helper.Error : VA_Helper.NoError);
	}
	
	protected void EndError()
	{
		EditorGUILayout.EndVertical();
	}
	
	protected void DrawDefault(string proeprtyPath)
	{
		EditorGUILayout.BeginVertical(VA_Helper.NoError);
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty(proeprtyPath), true);
		}
		EditorGUILayout.EndVertical();
	}
	
	protected virtual void OnInspector()
	{
	}
	
	protected virtual void OnScene()
	{
	}
}