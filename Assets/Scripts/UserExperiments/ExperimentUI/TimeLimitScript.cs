using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TimeLimitScript : MonoBehaviour {

    private GameObject EmpaticaClientObject;

    // Use this for initialization
    void Start () {
        EmpaticaClientObject = GameObject.Find("EmpaticaClient");
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (LogisticLoader.HASFINISHED())
            {
                SceneManager.LoadScene("endScreen");
            }
            else
            {
                // Keep the Empatica Object Ready for the next scene!
                GameObject.DontDestroyOnLoad(EmpaticaClientObject);

                SceneManager.LoadScene("levelswitch");
            }
        }
	}
}
