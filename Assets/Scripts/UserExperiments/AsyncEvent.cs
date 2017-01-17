using UnityEngine;
using System.Collections;

public class AsyncEvent {

    private EventCode.AsyncEventCode async_event;
    private double timestamp;

    private string[] data;

    public AsyncEvent(EventCode.AsyncEventCode async_event, string[] data)
    {
        this.async_event = async_event;
        this.data = data;
    }

    public void setTimestamp(double timestamp)
    {
        this.timestamp = timestamp;
    }

    public string createLog()
    {
        string log = "";
        log += timestamp + "," + async_event;
        foreach(string s in data)
        {
            log += "," + s;
        }
        return log;
    }

}
