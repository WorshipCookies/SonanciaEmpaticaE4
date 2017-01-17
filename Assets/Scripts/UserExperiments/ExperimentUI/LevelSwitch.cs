using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSwitch : MonoBehaviour {

    private GameObject EmpaticaClientObject;
    private Button startButton;
    private Text corpus;
    private Text title;

    private float timer;
    // Use this for initialization
    void Start () {
        EmpaticaClientObject = GameObject.Find("EmpaticaClient");
        startButton = GameObject.Find("Button").GetComponent<Button>();
        startButton.interactable = false;

        corpus = GameObject.Find("Corpus").GetComponent<Text>();
        title = GameObject.Find("Title").GetComponent<Text>();

        // Hold for 20 seconds so that users don't immediately press the button.
        timer = 20f;
    }
	
	// Update is called once per frame
	void Update () {
	    
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            startButton.interactable = false;

        }
        else
        {
            startButton.interactable = true;
        }

	}

    public void loadNextLevel()
    {
        // This is where this scene is reloaded to load the next level.
        Debug.Log("Loading Next Level");

        title.text = "";
        corpus.text = "Now Loading Level.\nPlease Wait...";

        // Keep the Empatica Object Ready for the next scene!
        GameObject.DontDestroyOnLoad(EmpaticaClientObject);

        // Load Level Builder here!
        SceneManager.LoadScene("buildingscenes");
    }
}
