using UnityEngine;
using System.Collections;
using System;

public class SoundLogging : MonoBehaviour, ObjectLogger {

    private EventDataBase globalData;
    private PlayerLogging playerLog;

    // Use this for initialization
    void Start()
    {
        // Load External Classes (Outside of the Monster Object)
        globalData = FindObjectOfType(typeof(EventDataBase)) as EventDataBase;
        playerLog = FindObjectOfType(typeof(PlayerLogging)) as PlayerLogging;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void asyncLogger(AsyncEvent e)
    {
        globalData.sendAsyncSignal(e);
    }

    public void registerLoggerToEventLogger()
    {
        globalData.registerLogger(this);
    }

    // For this Object a Sync Logger is Unecessary.
    public SyncEvent syncLogger(EventCode.GlobalUpdateEventCode code)
    {
        return null; // NOT IMPLEMENTED YET!
    }

    // Log when a 3D Sound is triggered -- Format -- [TIMESTAMP, EVENTCODE, SOUNDNAME, ROOMID, TILEID]
    public void logSoundCollision()
    {
        if (EventDataBase.startLogging)
        {
            string clipName = GetComponent<AudioSource>().clip.name;
            string[] str = { clipName, Convert.ToString(playerLog.getCurrentRoom()),
            Convert.ToString(playerLog.getCurrentTile()) };
            asyncLogger(new AsyncEvent(EventCode.AsyncEventCode.SOUND_TRIGGERED, str));
        }
    }
}
