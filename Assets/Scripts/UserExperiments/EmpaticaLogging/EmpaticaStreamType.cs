using UnityEngine;
using System.Collections;

public class EmpaticaStreamType {

    /**
     * Types of Stream available from the Empatica Device.
     **/
	public enum StreamType
    {
        Galvanic_Skin_Response,
        Accelerometer,
        Blood_Volume_Pulse,
        Inter_Beat_Interval,
        Skin_Temperature
    }

    public static string[] serverStreamTypes = { "E4_Gsr", "E4_Acc", "E4_Bvp", "E4_Hr", "E4_Temperature" };

    public static bool isAStreamResponse(string msg)
    {
        foreach(string s in serverStreamTypes)
        {
            if (msg == s)
                return true;
        }
        return false;
    }
}
