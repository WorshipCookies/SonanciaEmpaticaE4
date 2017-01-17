using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using ProjectMaze.GeneticAlgorithm;
using UnityEngine.SceneManagement;

public class TensionGraphSelection : MonoBehaviour {

    private Button confirmButton;
    public ToggleGroup graphSelected;

	// Use this for initialization
	void Start () {
        confirmButton = GameObject.Find("ConfirmButton").GetComponent<Button>();
        confirmButton.interactable = false;
        
	}
	
	// Update is called once per frame
	void Update () {

        if (graphSelected.AnyTogglesOn())
        {
            confirmButton.interactable = true;
        }
	}
    
    public void buttonClick()
    {
        List<Toggle> active = new List<Toggle>( graphSelected.ActiveToggles() );
        Debug.Log("Active Togles = " + active.Count);
        if(active.Count == 1)
        {
            Toggle t = active[0];

            if(t.name == "MidPointRest")
            {
                LevelBuilder.tension_map = Tension_Maps.U_Shape;
                Debug.Log("MidPointRest Selected");

                // Start Level Generation
                SceneManager.LoadScene("UIScene");
            }
            else if(t.name == "MidPointTense")
            {
                LevelBuilder.tension_map = Tension_Maps.Inverse_U;
                Debug.Log("MidPointTense Selected");

                // Start Level Generation
                SceneManager.LoadScene("UIScene");
            }
            else if(t.name == "ZigZagTension")
            {
                LevelBuilder.tension_map = Tension_Maps.Zig_Zag;
                Debug.Log("ZigZagTension Selected");

                // Start Level Generation
                SceneManager.LoadScene("UIScene");
            }
            else if(t.name == "SystemDefined")
            {
                Debug.Log("System Defined Tension Selected");

                // Start Tension Graph Generation
                SceneManager.LoadScene("TensionGenScene");
            }
        }
    }
}
