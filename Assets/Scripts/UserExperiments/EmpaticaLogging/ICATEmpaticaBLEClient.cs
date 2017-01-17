/* -- ICAT's Empatica Bluetooth Low Energy(BLE) Comm Client -- *
 * ----------------------------------------------------------- *
 * 1. On launch, it tries to connect to the localhost/port20 
 * 	  (You have to change it to your own ip/port combination).
 * 2. Enter the Device ID and connect to device.
 * 3. Select the data streams to log and hit "Log Data"
 * 4. Hit Ctrl+Shift+Z to disconnect at anytime.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Diagnostics;

using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using System.Threading;

public class ICATEmpaticaBLEClient : MonoBehaviour {	
	//variables	
	private TCPConnection myTCP;	
	private string streamSelected;
	public string msgToServer;
	public string connectToServer;
	
    // Switch this so we can stream to the file we want!
	private string savefilename = "name" + DateTime.UtcNow.ToString("dd_mm_yyyy_hh_mm_ss") + ".txt";

	//flag to indicate device conection status
	private bool deviceConnected = false;

	//flag to indicate if data to be logged to file
	private bool logToFile = false;

    public static string EmpaticaDeviceID = "89BBA7";//"7DBBA7"; // Hardcoded but can be changed.

    // Keep a reference if the stream is active in case we want to stop it mid game or something.
    private bool gsrActive = false;
    private bool accActive = false;
    private bool bvpActive = false;
    private bool ibiActive = false;
    private bool stActive = false;

    private bool subscribeToAll = false;
    private bool waitingForResponse = false;
    private EmpaticaStreamType.StreamType toChange;
    private Text empaticafeedbackText;
    private Text empaticaGSRStream;

    // The Stream Starts Paused so we can control when to start it
    private static bool pauseStream = true; 
    public static bool streamIsPaused = false;

    public bool empaticaReady = false;

    private string pathGSR;
    private string pathACC;
    private string pathBVP;
    private string pathIBI;
    private string pathST;

    private EventDataBase dataBase;

    private Thread socketStreamming;
    public static Queue<string> streamResponse;

    public static Queue<string> debugResponse = new Queue<string>();

    public static bool isStreaming;
    public static bool threadRunning;

    // Time Systems
    public EmpaticaStreamLogger loggerSystem;
    public static Stopwatch stopwatch;

    public static EmpaticaTimeConverter gsrTime;
    public static EmpaticaTimeConverter accTime;
    public static EmpaticaTimeConverter bvpTime;
    public static EmpaticaTimeConverter tmpTime;
    public static EmpaticaTimeConverter ibiTime;

    void Awake() {
        //add a copy of TCPConnection to this game object		
        myTCP = new TCPConnection();

        //dataBase = null;	
	}
	
	void Start () {

        empaticafeedbackText = GameObject.Find("Empatica_Feedback").GetComponent<Text>();
        empaticaGSRStream = GameObject.Find("Empatica_GSR").GetComponent<Text>();

        //DisplayTimerProperties ();
        streamSelected = String.Empty; // Important need to Initialize the Stream Before using it!

		if (myTCP.socketReady == false) {			
			Debug.Log("Attempting to connect..");
			//Establish TCP connection to server
			myTCP.setupSocket();
		}

        if (empaticafeedbackText != null)
        {
            empaticafeedbackText.text += "Connected To Server\n";
        }

        streamResponse = new Queue<string>();

        // Initialize the TimeConverters
        
    }

	void Update ()
    {		
		//keep checking the server for messages, if a message is received from server, 
		//it gets logged in the Debug console (see function below)
        if(!isStreaming)
		    SocketResponse();

        //if (!waitingForResponse && deviceConnected && pauseStream)
        //{
        //    sendPauseMsg();
        //}

        if (!isStreaming)
        {
            if (!waitingForResponse && deviceConnected)
            {
                //if (pauseStream)
                //{
                //    sendPauseMsg();
                //}
                //else 
                if (subscribeToAll)
                {
                    if (!gsrActive)
                    {
                        connectToStream(EmpaticaStreamType.StreamType.Galvanic_Skin_Response);
                    }
                    else if (!accActive)
                    {
                        connectToStream(EmpaticaStreamType.StreamType.Accelerometer);
                    }
                    else if (!bvpActive)
                    {
                        connectToStream(EmpaticaStreamType.StreamType.Blood_Volume_Pulse);
                    }
                    else if (!ibiActive)
                    {
                        connectToStream(EmpaticaStreamType.StreamType.Inter_Beat_Interval);
                    }
                    else if (!stActive)
                    {
                        connectToStream(EmpaticaStreamType.StreamType.Skin_Temperature);
                    }
                    Thread.Sleep(100);
                }
            }

            if (gsrActive && accActive && bvpActive && ibiActive && stActive)
            {
                empaticaReady = true;
            }
        }

        //if(debugResponse.Count > 0)
        //{
        //    while(debugResponse.Count > 0)
        //    {
        //        Debug.Log(debugResponse.Dequeue());
        //    }
        //}
	}

    void OnApplicationQuit()
    {
        if(socketStreamming != null)
        {
            Debug.Log("Stopping Thread...");
            isStreaming = false;
            while (threadRunning)
            {
                Debug.Log("Waiting for thread to close...");
            }
            //while (socketStreamming.IsAlive)
            //{
            //    Debug.Log("Thread still closing.. Brute Forcing...");
            //    socketStreamming.Abort();
            //}
            //socketStreamming = null;
            Debug.Log("Thread Has Stopped!");
        }

        if (EmpaticaStreamLogger.threadIsRunning)
        {
            Debug.Log("Stopping Writter Thread...");
            EmpaticaStreamLogger.stopThread = true;
            while (EmpaticaStreamLogger.threadIsRunning)
            {
                Debug.Log("Waiting for writer to finish writing...");
                Thread.Sleep(100);
            }
            Debug.Log("Writter thread has finished..");
        }

        closeClientAndDisconnect();
    }

    public void connectToDevice()
    {
        if (myTCP.socketReady == true && deviceConnected == false)
        {
            SendToServer("device_connect " + EmpaticaDeviceID);
            Debug.Log("Connected to Empatica. Press Ctrl+Shift+Z to disconnect Empatica at any time");
        }

        if (empaticafeedbackText != null)
        {
            empaticafeedbackText.text += "Sucessfully Connected to Device " + EmpaticaDeviceID + "\n";
        }

        subscribeToAllStreams();
    }

    public void subscribeToStream(EmpaticaStreamType.StreamType type)
    {
        toChange = type;
    }

    private void connectToStream(EmpaticaStreamType.StreamType type)
    {
        if (myTCP.socketReady == true && deviceConnected == true && logToFile == false)
        {
            // Subscribe to the GSR Stream
            if (type == EmpaticaStreamType.StreamType.Galvanic_Skin_Response)
            {
                if (gsrActive)
                {
                    SendToServer("device_subscribe gsr OFF");
                }
                else
                {
                    SendToServer("device_subscribe gsr ON");
                    streamSelected += "GSR ";
                }
                //gsrActive = !gsrActive;
            }
            // Subscribe to to the ACC Stream
            if (type == EmpaticaStreamType.StreamType.Accelerometer)
            {
                if (accActive)
                {
                    SendToServer("device_subscribe acc OFF");
                }
                else
                {
                    SendToServer("device_subscribe acc ON");
                    streamSelected += "ACC ";
                }
                //accActive = !accActive;
            }
            // Subscribe to the BVP Stream
            if (type == EmpaticaStreamType.StreamType.Blood_Volume_Pulse)
            {
                if (bvpActive)
                {
                    SendToServer("device_subscribe bvp OFF");
                }
                else
                {
                    SendToServer("device_subscribe bvp ON");
                    streamSelected += "BVP ";
                }
                //bvpActive = !bvpActive;
            }
            if (type == EmpaticaStreamType.StreamType.Inter_Beat_Interval)
            {
                if (ibiActive)
                {
                    SendToServer("device_subscribe ibi OFF");
                }
                else
                {
                    SendToServer("device_subscribe ibi ON");
                    streamSelected += "IBI ";
                }
                //ibiActive = !ibiActive;
            }
            if (type == EmpaticaStreamType.StreamType.Skin_Temperature)
            {
                if (stActive)
                {
                    SendToServer("device_subscribe tmp OFF");
                }
                else
                {
                    SendToServer("device_subscribe tmp ON");
                    streamSelected += "TMP ";
                }
                //stActive = !stActive;
            }
        }
    }

    // Subscribe to All Streams
    public void subscribeToAllStreams()
    {
        subscribeToAll = true;
    }

    public void pauseTheStream()
    {
        pauseStream = true;
    }

    public void startStreaming()
    {
        if(dataBase == null)
        {
            stopwatch = new Stopwatch();
            EmpaticaTimeConverter.gameTime = stopwatch;
            initializeTimeConverters();
            stopwatch.Start();
            loggerSystem = new EmpaticaStreamLogger("D:\\Google Drive\\Unity Projects\\SonificationV11_EmpaticaIntegration\\Experiments");
            
        }
        else
        {
            stopwatch = this.dataBase.currentTimeStamp;
            EmpaticaTimeConverter.gameTime = stopwatch;
            initializeTimeConverters();
            loggerSystem = new EmpaticaStreamLogger(this.dataBase.logSystem.getLvlFolderDestination());
        }

        loggerSystem.startLoggerThread();

        //sendPauseMsg();
        //pauseStream = true;
        socketStreamming = new Thread(SocketStream);
        isStreaming = true;
        socketStreamming.Start();
    }

    public void stopStreaming()
    {
        isStreaming = false;
    }

    public void sendPauseMsg()
    {
        if (!streamIsPaused)
        {
            Debug.Log("Pausing!");
            SendToServer("pause ON");
        }
        else
        {
            Debug.Log("UnPausing!");
            SendToServer("pause OFF");
        }
        pauseStream = false;
    }

    //void OnGUI() {		
        //	//if connection has not been made, display button to connect		
        //	if (myTCP.socketReady == false) {			
        //		if (GUILayout.Button ("Connect")) {	
        //			Debug.Log("Attempting to connect..");
        //			//Establish TCP connection to server
        //			myTCP.setupSocket();
        //		}
        //	}

        //	//once TCP connection has been made, connect to Empatica device
        //	if (myTCP.socketReady == true && deviceConnected == false){
        //		if (GUILayout.Button ("Device List")) {	
        //			// ask to pupulate device list
        //			SendToServer("device_list");
        //		}
        //		connectToServer = GUILayout.TextField(connectToServer);
        //		if (GUILayout.Button ("Connect to Device", GUILayout.Height(30))) {				
        //			SendToServer("device_connect " + connectToServer);
        //			Debug.Log("Connected to Empatica. Press Ctrl+Shift+Z to disconnect Empatica at any time");
        //		}
        //	}

        //	//once device has been connected, choose which streams to select and start logging	
        //	if (myTCP.socketReady == true && deviceConnected == true && logToFile == false) {			
        //		msgToServer = GUILayout.TextField(msgToServer);
        //		if (GUILayout.Button ("Write to server", GUILayout.Height(30))) {				
        //			SendToServer(msgToServer);
        //		}

        //		//Buttons for selecting data streams
        //		streamSelected = GUILayout.TextField(streamSelected);
        //		if (GUILayout.Button ("Galvanic Skin Response")){
        //			SendToServer("device_subscribe gsr ON");
        //			streamSelected += "GSR ";
        //		}
        //		if (GUILayout.Button ("Accelerometer")){
        //			SendToServer("device_subscribe acc ON");
        //			streamSelected += "ACC ";
        //		}
        //		if (GUILayout.Button ("Blood Volume Pulse")){
        //			SendToServer("device_subscribe bvp ON");
        //			streamSelected += "BVP ";
        //		}
        //		if (GUILayout.Button ("Inter Beat Interval")){
        //			SendToServer("device_subscribe ibi ON");
        //			streamSelected += "IBI ";
        //		}
        //		if (GUILayout.Button ("Skin Temperature")){
        //			SendToServer("device_subscribe tmp ON");
        //			streamSelected += "TMP ";
        //		}

        //		//Button for logging data to file
        //		if (GUILayout.Button("Log Data")){
        //			logToFile = true;
        //			Debug.Log("Started Logging data. Press Ctrl+Shift+Z to disconnect Empatica at any time");
        //		}
        //	}

        //	//button combination for disconnecting
        //	Event e = Event.current;
        //	if (myTCP.socketReady == true && deviceConnected == true) {
        //		if (e.type == EventType.KeyDown && e.control && e.shift && e.keyCode == KeyCode.Z){
        //			Debug.Log("Disconnecting Device and TCP connection...");
        //			//disconnect Empatica
        //			SendToServer("device_disconnect");
        //			//disconnect TCP
        //			myTCP.closeSocket();

        //			//reset all flags
        //			deviceConnected = false;
        //			logToFile = false;
        //			streamSelected = "";
        //		}
        //	}
        //}

        //socket reading script	

    void SocketResponse() {		
		string serverSays = myTCP.readSocket();	
		if (serverSays != "") {		
			if (myTCP.socketReady == true && deviceConnected == true && logToFile == true){
				//streamwriter for writing to file
				using(StreamWriter sw = File.AppendText(savefilename)){
					sw.WriteLine(serverSays);
				}
			}
            else
            {
                // Check the Server Response Here
				//Debug.Log("[SERVER]" + serverSays);
				string serverConnectOK = @"R device_connect OK";

                //Check if server response was device_connect OK
                waitingForResponse = false;
                if (string.CompareOrdinal(Regex.Replace(serverConnectOK, @"\s", ""), Regex.Replace(serverSays.Substring(0, serverConnectOK.Length), @"\s", "")) == 0) {
                    deviceConnected = true;
                    //empaticafeedbackText.text = serverSays;
                }
                else
                {
                    string[] serverSaySplit = serverSays.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                    foreach(string s in serverSaySplit)
                    {
                        string[] parsedResponse = s.Split(null);
                        if (parsedResponse[0] == "R")
                        {
                            if (serverSaySplit[0].Split(null)[1] == "device_subscribe")
                            {
                                //Debug.Log("[SERVER]" + serverSays);
                                parseServerSubscribeResponse(serverSaySplit);
                                if (dataBase == null)
                                {
                                    //empaticafeedbackText.text = serverSays;
                                }
                            }
                            // Pause Response
                            else if (parsedResponse[1] == "pause")
                            {
                                if (parsedResponse[2] == "ON")
                                {
                                    streamIsPaused = true;
                                }
                                else
                                {
                                    streamIsPaused = false;
                                }
                                //if (dataBase == null)
                                //{
                                //    //empaticafeedbackText.text = serverSays;
                                //}
                            }
                        }
                        else
                        {
                            string[] parseLine = s.Split(null);
                            if (parseLine[0] == "E4_Gsr")
                            {
                                empaticaGSRStream.text = parseLine[0] + " = " + parseLine[2];
                                DebugGraph.Log("rawValues", Convert.ToDouble(parseLine[2]));
                            }
                        }
                    }

                    //string[] parsedResponse = serverSaySplit[0].Split(null);
                    //if (parsedResponse[0] == "R")
                    //{
                    //    if (serverSaySplit[0].Split(null)[1] == "device_subscribe")
                    //    {
                    //        //Debug.Log("[SERVER]" + serverSays);
                    //        parseServerSubscribeResponse(serverSaySplit);
                    //        if(dataBase == null)
                    //        {
                    //            //empaticafeedbackText.text = serverSays;
                    //        }
                    //    }
                    //    // Pause Response
                    //    else if (parsedResponse[1] == "pause")
                    //    {
                    //        if(parsedResponse[2] == "ON")
                    //        {
                    //            streamIsPaused = true;
                    //        }
                    //        else
                    //        {
                    //            streamIsPaused = false;
                    //        }
                    //        //if (dataBase == null)
                    //        //{
                    //        //    //empaticafeedbackText.text = serverSays;
                    //        //}
                    //    }
                    //}
                    //else
                    //{
                    //    foreach (string s in serverSaySplit)
                    //    {
                            
                    //    }
                    //}
                }
            }
		} 
	}

    void SocketStream()
    {
        ICATEmpaticaBLEClient.threadRunning = true;
        while (isStreaming)
        {
            string serverSays = myTCP.readSocket();
            if (serverSays != "")
            {
                string[] serverSaySplit = serverSays.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                // Remember responses can come with multiple data points! 
                // So we need to parse them accordingly.
                foreach (string s in serverSaySplit)
                {
                    string[] parsedResponse = s.Split(null);
                    if (parsedResponse[0] == "R")
                    {
                        if (parsedResponse[1] == "pause")
                        {
                            if (parsedResponse[2] == "ON")
                            {
                                streamIsPaused = true;
                            }
                            else
                            {
                                streamIsPaused = false;
                            }
                        }
                    }
                    else if (ICATEmpaticaBLEClient.isStreaming && EmpaticaStreamType.isAStreamResponse(parsedResponse[0]))//EventDataBase.startLogging)
                    {
                        streamResponse.Enqueue(parsedResponse[0] + ";" + s + " " + EventDataBase.currentTimeInSeconds(stopwatch));
                    }
                    //else
                    //{
                    //    debugResponse.Enqueue(s);
                    //}
                }
            }
        }

        streamResponse.Enqueue("End_Session;" + "END_SESSION " + EventDataBase.currentTimeInSeconds(stopwatch));
        //sendPauseMsg();
        //ICATEmpaticaBLEClient.pauseStream = true;
        ICATEmpaticaBLEClient.threadRunning = false;
        socketStreamming = null;
    }

    public void closeClientAndDisconnect()
    {
        SendToServer("device_disconnect" + Environment.NewLine);
        myTCP.closeSocket();
    }

	//send message to the server	
	public void SendToServer(string str) {
        waitingForResponse = true;		 
		myTCP.writeSocket(str);		
		Debug.Log ("[CLIENT] " + str);		
	}

	//Method To check Stopwatch properties
	void DisplayTimerProperties()
	{
		// Display the timer frequency and resolution.
		if (Stopwatch.IsHighResolution){
			Debug.Log("Operations timed using the system's high-resolution performance counter.");
		}else{
			Debug.Log("Operations timed using the DateTime class.");
		}
		
		long frequency = Stopwatch.Frequency;
		Debug.Log(string.Format("Timer frequency in ticks per second = {0}",frequency));
		long nanosecPerTick = (1000L*1000L*1000L) / frequency;
		Debug.Log(string.Format("Timer is accurate within {0} nanoseconds",nanosecPerTick));
	}

    void waitForSocketReady()
    {
        while (!myTCP.socketReady)
        {
            // Sleep
        }
    }

    public void parseServerSubscribeResponse(string[] serverMsg)
    {
        // GSR Msg
        if(serverMsg[0] == EmpaticaServerResponseDataBase.GSR_ON)
        {
            gsrActive = true;

            if(empaticafeedbackText != null)
            {
                empaticafeedbackText.text += "GSR Active\n";
            }

        }
        if(serverMsg[0] == EmpaticaServerResponseDataBase.GSR_OFF)
        {
            gsrActive = false;
        }

        // ACC Msg
        if (serverMsg[0] == EmpaticaServerResponseDataBase.ACC_ON)
        {
            accActive = true;

            if (empaticafeedbackText != null)
            {
                empaticafeedbackText.text += "ACC Active\n";
            }
        }
        if (serverMsg[0] == EmpaticaServerResponseDataBase.ACC_OFF)
        {
            accActive = false;
        }

        // BVP Msg Parser
        if(serverMsg[0] == EmpaticaServerResponseDataBase.BVP_ON)
        {
            bvpActive = true;

            if (empaticafeedbackText != null)
            {
                empaticafeedbackText.text += "BVP Active\n";
            }
        }
        if (serverMsg[0] == EmpaticaServerResponseDataBase.BVP_OFF)
        {
            bvpActive = false;
        }

        // IBI Msg
        if(serverMsg[0] == EmpaticaServerResponseDataBase.IBI_ON)
        {
            ibiActive = true;

            if (empaticafeedbackText != null)
            {
                empaticafeedbackText.text += "IBI Active\n";
            }
        }
        if(serverMsg[0] == EmpaticaServerResponseDataBase.IBI_OFF)
        {
            ibiActive = false;
        }

        // ST Msg
        if(serverMsg[0] == EmpaticaServerResponseDataBase.ST_ON)
        {
            stActive = true;

            if (empaticafeedbackText != null)
            {
                empaticafeedbackText.text += "TMP Active\n";
            }
        }
        if(serverMsg[0] == EmpaticaServerResponseDataBase.ST_OFF)
        {
            stActive = false;
        }
    }
    
    public void setEventDataBase(EventDataBase dataBase)
    {
        this.dataBase = dataBase;
    }

    public void initializeLogFiles(string path)
    {
        this.pathGSR = path + "\\gsrStream.csv";
        this.pathACC = path + "\\accStream.csv";
        this.pathBVP = path + "\\bvpStream.csv";
        this.pathIBI = path + "\\ibiStream.csv";
        this.pathST = path + "\\tempStream.csv"; 
    }

    public void writeToFile(string log, string filepath)
    {
        log = log + ", " + dataBase.currentTimeInSeconds();
        using (StreamWriter sw = File.AppendText(filepath))
        {
            sw.WriteLine(log);
        }
    }

    public static void initializeTimeConverters()
    {
        gsrTime = new EmpaticaTimeConverter();
        accTime = new EmpaticaTimeConverter();
        bvpTime = new EmpaticaTimeConverter();
        ibiTime = new EmpaticaTimeConverter();
        tmpTime = new EmpaticaTimeConverter();
    }
}

