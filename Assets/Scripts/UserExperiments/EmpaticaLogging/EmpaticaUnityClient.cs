using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System;

public class EmpaticaUnityClient : MonoBehaviour {

    // The Stream Starts Paused so we can control when to start it
    private bool pauseStream = false;
    private bool streamIsPaused = false;

    // Keep a reference if the stream is active in case we want to stop it mid game or something.
    private bool gsrActive = false;
    private bool accActive = false;
    private bool bvpActive = false;
    private bool ibiActive = false;
    private bool stActive = false;

    private bool subscribeToAll = false;

    //flag to indicate device conection status
    private bool deviceConnected = false;

    public static Queue<string> serverRequests;
    public static Queue<string> serverResponse;

    public static Queue<string> serverStreamResponse; // For Logging the Stream

    public static bool threadRunning;
    public static bool closeThread;

    private Thread clientSocket;
    // Use this for initialization
    void Start () {
        serverRequests = new Queue<string>();
        serverResponse = new Queue<string>();
        threadRunning = true;
        closeThread = false;

        clientSocket = new Thread(AsynchronousClient.StartClient);
        //clientSocket.IsBackground = true;
        clientSocket.Start();
	}
	
	// Update is called once per frame
	void Update () {
        if (serverResponse.Count > 0)
        {
            handleReceivingMessages(serverResponse.Dequeue());
        }
	}

    void OnApplicationQuit()
    {
        Debug.Log("Stopping Thread...");
        closeThread = true;
        while(threadRunning)
        {
            Debug.Log("Waiting for thread to close...");
        }
        while (clientSocket.IsAlive)
        {
            Debug.Log("Thread still closing.. Brute Forcing...");
            clientSocket.Abort();
        }
        clientSocket = null;
        Debug.Log("Thread Has Stopped!");
    }

    public void sendConnectDeviceMsg()
    {
        serverRequests.Enqueue("device_connect 89BBA7");
    }

    public void sendMsgToServer(string msg)
    {
        serverRequests.Enqueue("pause ON");
        serverRequests.Enqueue("device_subscribe gsr ON");
        serverRequests.Enqueue("device_subscribe acc ON");
        //serverRequests.Enqueue("device_subscribe ibi ON");
        //serverRequests.Enqueue("device_subscribe tmp ON");
    }

    public void handleReceivingMessages(string serverSays)
    {
        string[] serverSaySplit = serverSays.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

        string[] parsedResponse = serverSaySplit[0].Split(null);
        if (parsedResponse[0] == "R")
        {
            if (serverSays == "R device_connect OK")
            {
                // Device has successfully been connected
                deviceConnected = true;
            }
            else if (serverSaySplit[0].Split(null)[1] == "device_subscribe")
            {
                //Debug.Log("[SERVER]" + serverSays);
                parseServerSubscribeResponse(serverSaySplit);
            }
            // Pause Response
            else if (parsedResponse[1] == "pause")
            {
                pauseStream = false;
                streamIsPaused = !streamIsPaused;
            }
        }
        //// If its a stream log it
        //else
        //{
        //    // Log GSR
        //    if (parsedResponse[0] == "E4_Gsr")
        //    {
        //        Debug.Log("[SERVER]" + serverSaySplit[0]);
        //        //writeToFile(serverSaySplit[0], pathGSR);
        //    }
        //    // Log ACC
        //    else if (parsedResponse[0] == "E4_Acc")
        //    {
        //        Debug.Log("[SERVER]" + serverSaySplit[0]);
        //        //writeToFile(serverSaySplit[0], pathACC);
        //    }
        //    // Log BVP
        //    else if (parsedResponse[0] == "E4_Bvp")
        //    {
        //        Debug.Log("[SERVER]" + serverSaySplit[0]);
        //        //writeToFile(serverSaySplit[0], pathBVP);
        //    }
        //    // Log IBI
        //    else if (parsedResponse[0] == "E4_Hr")
        //    {
        //        Debug.Log("[SERVER]" + serverSaySplit[0]);
        //        //writeToFile(serverSaySplit[0], pathIBI);
        //    }
        //    // Log ST (or TEMP)
        //    else if (parsedResponse[0] == "E4_Temperature")
        //    {
        //        Debug.Log("[SERVER]" + serverSaySplit[0]);
        //        //writeToFile(serverSaySplit[0], pathST);
        //    }
        //    else
        //    {
        //        Debug.Log(parsedResponse[0] + ", " + serverSaySplit[0]);
        //    }
        //}
    }

    public void parseServerSubscribeResponse(string[] serverMsg)
    {
        // GSR Msg
        if (serverMsg[0] == EmpaticaServerResponseDataBase.GSR_ON)
        {
            gsrActive = true;
        }
        if (serverMsg[0] == EmpaticaServerResponseDataBase.GSR_OFF)
        {
            gsrActive = false;
        }

        // ACC Msg
        if (serverMsg[0] == EmpaticaServerResponseDataBase.ACC_ON)
        {
            accActive = true;
        }
        if (serverMsg[0] == EmpaticaServerResponseDataBase.ACC_OFF)
        {
            accActive = false;
        }

        // BVP Msg Parser
        if (serverMsg[0] == EmpaticaServerResponseDataBase.BVP_ON)
        {
            bvpActive = true;
        }
        if (serverMsg[0] == EmpaticaServerResponseDataBase.BVP_OFF)
        {
            bvpActive = false;
        }

        // IBI Msg
        if (serverMsg[0] == EmpaticaServerResponseDataBase.IBI_ON)
        {
            ibiActive = true;
        }
        if (serverMsg[0] == EmpaticaServerResponseDataBase.IBI_OFF)
        {
            ibiActive = false;
        }

        // ST Msg
        if (serverMsg[0] == EmpaticaServerResponseDataBase.ST_ON)
        {
            stActive = true;
        }
        if (serverMsg[0] == EmpaticaServerResponseDataBase.ST_OFF)
        {
            stActive = false;
        }
    }
}
