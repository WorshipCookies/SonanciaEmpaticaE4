using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

public class EventDataBase : MonoBehaviour {

    // This Class has the list of events for the Asynchronous Logging.
    public Stopwatch currentTimeStamp;
    public UserLogSystem logSystem;

    private List<ObjectLogger> loggingObjectList;
    private ICATEmpaticaBLEClient empaticaClient;

    private int currFrame;
    public int logPerFrame = 20;

    public static bool startLogging = false;

    // Use this for initialization
    void Awake () {
        loggingObjectList = new List<ObjectLogger>();
        //empaticaClient = GameObject.Find("EmpaticaClient").GetComponent<ICATEmpaticaBLEClient>();
        empaticaClient = GetComponent<ICATEmpaticaBLEClient>();
        empaticaClient.setEventDataBase(this);

        currFrame = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (startLogging)
        {
            if (currFrame % logPerFrame == 0)
            { //If the remainder of the current frame divided by 10 is 0 run the function.
                sendSyncSignal();
            }
            currFrame++;
        }
    }

    void OnApplicationQuit()
    {
        if(logSystem != null)
        {
            logSystem.terminateLoggerThread();
        }
    }

    public void initializeLogSystem(string ExpID, string UserID)
    {
        this.logSystem = new UserLogSystem(ExpID, UserID);
    }

    public void createNewExperiment(string ExpID, string UserID, string LevelID)
    {
        //this.logSystem = new UserLogSystem(ExpID, UserID);
        setLevel(LevelID); // This Starts the Level
        initializeTimestamp();
        startLevelLog(ExpID, UserID, LevelID);
    }

    public void startBaselineCapture()
    {
        logSystem.startBaseline("baselineValues");
        initializeTimestamp();
        empaticaClient.startStreaming();
        startLogging = true;
    }

    public void stopStreaming()
    {
        if (ICATEmpaticaBLEClient.isStreaming)
        {
            empaticaClient.stopStreaming();
            startLogging = false;
        }
    }

    public void setLevel(string LevelID)
    {
        this.logSystem.startLevel(LevelID);
    }

    public void initializeTimestamp()
    {
        currentTimeStamp = new Stopwatch();
        currentTimeStamp.Reset();
        currentTimeStamp.Start();
    }

    public void stopTimestamp()
    {
        currentTimeStamp.Stop();
    }

    public void registerLogger(ObjectLogger log)
    {
        loggingObjectList.Add(log);
    }

    public void debugTest()
    {
        UnityEngine.Debug.Log("I Exist!");
    }

    public void sendSyncSignal()
    {
        foreach(ObjectLogger log in loggingObjectList)
        {
            SyncEvent n = log.syncLogger(EventCode.GlobalUpdateEventCode.GLOBAL_UPDATE_STATUS);
            if(n != null)
            {
                n.setTimestamp(currentTimeInSeconds());
                logSystem.writeSyncToFile(n);
            }
        }
    }

    public void sendAsyncSignal(AsyncEvent e)
    {
        // Set Timestamp on the Log Event.
        e.setTimestamp(currentTimeInSeconds());

        // Write Event into the File.
        logSystem.writeAsyncToFile(e);

        //UnityEngine.Debug.Log(e.createLog());
    }

    public double currentTimeInSeconds()
    {
        return currentTimeStamp.ElapsedMilliseconds / 1000.0;
    }

    public static double currentTimeInSeconds(Stopwatch stopwatch)
    {
        return stopwatch.ElapsedMilliseconds / 1000.0;
    }

    public void startLevelLog(string ExpID, string UserID, string LevelID)
    {
        string[] str = { ExpID, UserID, LevelID };
        sendAsyncSignal(new AsyncEvent(EventCode.AsyncEventCode.LEVEL_START, str));
        logSystem.writeSyncToFile(new SyncEvent(EventCode.SyncEventCode.LEVEL_START, str));
        empaticaClient.startStreaming();
        startLogging = true;
    }


}
