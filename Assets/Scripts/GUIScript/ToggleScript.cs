using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToggleScript : MonoBehaviour {

    private Toggle t;

	// Use this for initialization
	void Start () {
        t = GetComponent<Toggle>();
	}
	
	// Update is called once per frame
	void Update () {
        if (t.isOn)
        {
            t.targetGraphic.color = t.colors.highlightedColor;
        }
        else
        {
            t.targetGraphic.color = t.colors.normalColor;
        }
    }
}
