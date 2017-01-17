using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System;

public class EmpaticaStreamLogger {

    private string mainPath;

    private string gsrLogPath;
    private string accLogPath;
    private string bvpLogPath;
    private string tmpLogPath;
    private string ibiLogPath;

    private Thread writterThread;
    public static bool isWriting;
    public static bool stopThread;
    public static bool threadIsRunning;

    public EmpaticaStreamLogger(string mainPath)
    {
        this.mainPath = mainPath;
        this.gsrLogPath = mainPath + "\\gsrLog.csv";
        this.accLogPath = mainPath + "\\accLog.csv";
        this.bvpLogPath = mainPath + "\\bvpLog.csv";
        this.tmpLogPath = mainPath + "\\tmpLog.csv";
        this.ibiLogPath = mainPath + "\\ibiLog.csv";

        isWriting = false;
        stopThread = false;
        threadIsRunning = false;
    }

    public void startLoggerThread()
    {
        writterThread = new Thread(checkQueueAndWrite);
        writterThread.Start();
    }

    /** 
     * This is the threading function will constantly check the queue 
     * for new responses and write them into a file.
     **/
    public void checkQueueAndWrite()
    {
        threadIsRunning = true;
        isWriting = true;
        while (isWriting)
        {
            while(ICATEmpaticaBLEClient.streamResponse.Count > 0)
            {
                parserStreamResponse(ICATEmpaticaBLEClient.streamResponse.Dequeue());
            }
            if (stopThread)
            {
                isWriting = false;
            }
            else
            {
                Thread.Sleep(10);
            }
        }

        // Last Check! Write the remaining information into a file before quitting
        if(ICATEmpaticaBLEClient.streamResponse.Count > 0)
        {
            while (ICATEmpaticaBLEClient.streamResponse.Count > 0)
            {
                parserStreamResponse(ICATEmpaticaBLEClient.streamResponse.Dequeue());
            }
        }
        writterThread = null;
        threadIsRunning = false;
    }

    public void parserStreamResponse(string response)
    {
        string[] parsedResponse = response.Split(';');
        string[] parsedValues = parsedResponse[1].Split(null);

        // Log GSR
        if (parsedResponse[0] == "E4_Gsr")
        {
            // Write to GSR File
            double newReference = Convert.ToDouble(parsedValues[1].Replace(',', '.'));
            double realTimeValue = ICATEmpaticaBLEClient.gsrTime.updateTime(Convert.ToDouble(parsedValues[3]),newReference);
            writeToFile(parsedResponse[1] + " " + realTimeValue, gsrLogPath);
        }
        // Log ACC
        else if (parsedResponse[0] == "E4_Acc")
        {
            // Write to ACC File
            double newReference = Convert.ToDouble(parsedValues[1].Replace(',', '.'));
            double realTimeValue = ICATEmpaticaBLEClient.accTime.updateTime(Convert.ToDouble(parsedValues[3]), newReference);
            writeToFile(parsedResponse[1] + " " + realTimeValue, accLogPath);
        }
        // Log BVP
        else if (parsedResponse[0] == "E4_Bvp")
        {
            // Write to BVP File
            double newReference = Convert.ToDouble(parsedValues[1].Replace(',', '.'));
            double realTimeValue = ICATEmpaticaBLEClient.bvpTime.updateTime(Convert.ToDouble(parsedValues[3]), newReference);
            writeToFile(parsedResponse[1] + " " + realTimeValue, bvpLogPath);
        }
        // Log IBI
        else if (parsedResponse[0] == "E4_Hr")
        {
            // Write to IBI File
            double newReference = Convert.ToDouble(parsedValues[1].Replace(',', '.'));
            double realTimeValue = ICATEmpaticaBLEClient.ibiTime.updateTime(Convert.ToDouble(parsedValues[3]), newReference);
            writeToFile(parsedResponse[1] + " " + realTimeValue, ibiLogPath);   
        }
        // Log ST (or TEMP)
        else if (parsedResponse[0] == "E4_Temperature")
        {
            // Write to ST File
            double newReference = Convert.ToDouble(parsedValues[1].Replace(',', '.'));
            double realTimeValue = ICATEmpaticaBLEClient.tmpTime.updateTime(Convert.ToDouble(parsedValues[3]), newReference);
            writeToFile(parsedResponse[1] + " " + realTimeValue, tmpLogPath);
        }
        // Log Session Break
        else if(parsedResponse[0] == "End_Session")
        {
            writeToFile(parsedResponse[1], gsrLogPath);
            writeToFile(parsedResponse[1], accLogPath);
            writeToFile(parsedResponse[1], bvpLogPath);
            writeToFile(parsedResponse[1], ibiLogPath);
            writeToFile(parsedResponse[1], tmpLogPath);

            // Close writing
            isWriting = false;
        }
    }

    public void writeToFile(string log, string filepath)
    {
        using (StreamWriter sw = File.AppendText(filepath))
        {
            sw.WriteLine(log);
        }
    }
}
