using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class BaseLineScript : MonoBehaviour {

    private Text timerText;
    private Text startText;
    public static bool calculatingBaseline;
    private float timer;

    private AudioSource relaxSound;
    private EventDataBase logSystem;

    private GameObject EmpaticaClientObject;

    private bool waitingForEmpaticaResponse;

    // Use this for initialization
    void Start ()
    {
        Text[] textObjects = this.GetComponentInChildren<Canvas>().GetComponentsInChildren<Text>();
        foreach(Text t in textObjects)
        {
            if(t.name == "TimerText")
            {
                timerText = t;
            }
            if(t.name == "StartText")
            {
                startText = t;
            }
        }
        timer = 30f;
        relaxSound = this.GetComponentInChildren<AudioSource>();

        // Do Baseline stuff Here -- Stream Baseline
        EmpaticaClientObject = GameObject.Find("EmpaticaClient");
        logSystem = EmpaticaClientObject.GetComponent<EventDataBase>();

        logSystem.initializeLogSystem(Convert.ToString(LogisticLoader.EXP_ID), Convert.ToString(LogisticLoader.USR_ID));
        logSystem.startBaselineCapture();

        waitingForEmpaticaResponse = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!waitingForEmpaticaResponse)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timerText.text = "The level is loading. Please Wait ..";
                startText.text = "";

                if (relaxSound.isPlaying)
                {
                    relaxSound.Stop();
                }

                // Load Level Here
                logSystem.stopStreaming();
                waitingForEmpaticaResponse = true;
            }
            else
            {
                timerText.text = Convert.ToString(Mathf.Floor(timer)) + " seconds";
            }
        }
        else
        {
            if (!ICATEmpaticaBLEClient.isStreaming)
            {
                loadLevel();
            }
            else
            {
                Debug.Log("Waiting for Empatica Stream to Finish...");
            }
        }
	}

    public void loadLevel()
    {

        // Keep the Empatica Object Ready for the next scene!
        GameObject.DontDestroyOnLoad(EmpaticaClientObject);

        // Load Level Builder here!
        SceneManager.LoadScene("buildingscenes");
    }
}
