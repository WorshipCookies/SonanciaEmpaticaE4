using UnityEngine;
using System.Collections;

public class SyncEvent {

    private EventCode.SyncEventCode syncCode;
    private double timestamp;

    private string[] data;

    public SyncEvent(EventCode.SyncEventCode syncCode, string[] data)
    {
        this.syncCode = syncCode;
        this.data = data;
    }

    public void setTimestamp(double timestamp)
    {
        this.timestamp = timestamp;
    }

    public string createLog()
    {
        string log = "";
        log += timestamp + "," + syncCode;
        foreach (string s in data)
        {
            log += "," + s;
        }
        return log;
    }
}
