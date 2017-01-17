using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class ExperimentFileLoader : MonoBehaviour {

    // Use this for initialization

    private GameObject loadPanel;
    private GameObject filePanel;
    private InputField userInput;
    private InputField empaticaID;
    private Button startButton;
    private Text exp_Load;

    //public string path_lvl1 = "N/A";
    //public string path_lvl2 = "N/A";
    public string expID = "N/A";

    public List<string> lvlPaths;

    FileBrowser fb;

    private bool fileLoaded = false;
    private bool usrIDLoaded = false;

    private GameObject EmpaticaClientObject;
    private ICATEmpaticaBLEClient clientScript;
    private Button empaticaDeviceConenct;

	void Start () {
        loadPanel = GameObject.Find("LoadPanel");
        filePanel = GameObject.Find("FilePanel");
        userInput = GameObject.Find("InputFieldUser").GetComponent<InputField>();
        empaticaID = GameObject.Find("InputEmpaticaID").GetComponent<InputField>();
        exp_Load = GameObject.Find("TextExpID_Loaded").GetComponent<Text>();
        startButton = GameObject.Find("ButtonStart").GetComponent<Button>();

        filePanel.SetActive(false);

        // Empatica Code
        EmpaticaClientObject = GameObject.Find("EmpaticaClient");
        empaticaDeviceConenct = GameObject.Find("ButtonEmpatica").GetComponent<Button>();
        clientScript = EmpaticaClientObject.GetComponent("ICATEmpaticaBLEClient") as ICATEmpaticaBLEClient;

        empaticaID.text = ICATEmpaticaBLEClient.EmpaticaDeviceID;
    }
	
	// Update is called once per frame
	void Update () {
        //lvl1_Load.text = path_lvl1;
        //lvl2_Load.text = path_lvl2;
        exp_Load.text = expID;

        if (fileLoaded && usrIDLoaded && clientScript.empaticaReady)
        {
            startButton.interactable = true;
        }
        else
        {
            startButton.interactable = false;
        }
    }

    public void openFilePanel()
    {
        filePanel.SetActive(true);
        loadPanel.SetActive(false);
    }

    public void isFileLoaded()
    {
        fileLoaded = true;
    }

    public void isUsrIDLoaded()
    {
        Debug.Log(userInput.text);
        if (userInput.text == "")
        {
            usrIDLoaded = false;
        }
        else
        {
            usrIDLoaded = true;
        }
        
    }
    
    public void startExperiment()
    {
        // Experiment Start -- IT IS IMPORTANT TO KEEP ALL LEVEL FILES IN A SPECIFIC FOLDER
        string userID = userInput.text;
        string ExpID = expID;

        // Add lvls to the logistic loader
        List<string> lvls = new List<string>();
        foreach(string s in lvlPaths)
        {
            string lvl_struct = s.Split('.')[0];
            string lvl_soundtype = s.Split('.')[1];
            Debug.Log(s);
            lvls.Add(s);

        }

        //string lvl1_struct = path_lvl1.Split('.')[0];
        //string lvl1_soundtype = path_lvl1.Split('.')[1];

        //string lvl2_struct = path_lvl2.Split('.')[0];
        //string lvl2_soundtype = path_lvl2.Split('.')[1];

        //Debug.Log("Experiment " + ExpID + " of User " + userID + " Start: Level " + lvl1_struct + ", Type " + lvl1_soundtype);

        //List<string> lvls = new List<string>();
        //lvls.Add(path_lvl1);
        //lvls.Add(path_lvl2);

        LogisticLoader.setParam(lvls, Convert.ToInt32(ExpID), Convert.ToInt32(userID));

        // Keep the Empatica Object Ready for the next scene!
        GameObject.DontDestroyOnLoad(EmpaticaClientObject);

        // Load Level Builder here!
        SceneManager.LoadScene("baselineScreen");
    }

    public void newEmpaticaID()
    {
        ICATEmpaticaBLEClient.EmpaticaDeviceID = empaticaID.text;
    }

}
