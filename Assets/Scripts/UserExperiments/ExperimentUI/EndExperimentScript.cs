using UnityEngine;
using System.Collections;

public class EndExperimentScript : MonoBehaviour {

    private float time;

	// Use this for initialization
	void Start () {
        time = 25f;
	}
	
	// Update is called once per frame
	void Update () {
        if(time > 0)
        {
            time = -Time.deltaTime;
        }
        else
        {
            Application.Quit();
        }
	}
}
