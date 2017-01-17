using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class UserLogSystem {

    
    public string globalExperimentFolder = System.IO.Directory.GetCurrentDirectory() + "\\Experiments";

    private static AsynchronousLog async;
    private static SynchronousLog sync;

    private string ExpID;
    private string UserID;
    private string LevelID;

    private string ExpFolderDestination;
    private string LvlFolderDestination;
    private string baselineFolderDestination;

    private static bool isLevelRunning;
    private static bool threadRunning;

    private static Queue<SyncEvent> syncBuffer;
    private static Queue<AsyncEvent> asyncBuffer;
    private Thread writerThread;

    public UserLogSystem(string ExpID, string UserID)
    {
        this.ExpID = ExpID;
        this.UserID = UserID;
        this.LevelID = "";

        System.IO.Directory.CreateDirectory(globalExperimentFolder); // If there is no Global Directory create one!

        ExpFolderDestination = globalExperimentFolder + "\\" + ExpID + "_" + UserID;
        System.IO.Directory.CreateDirectory(ExpFolderDestination);

        baselineFolderDestination = ExpFolderDestination + "\\baselineValues";
        System.IO.Directory.CreateDirectory(baselineFolderDestination);

        syncBuffer = new Queue<SyncEvent>();
        asyncBuffer = new Queue<AsyncEvent>();

        isLevelRunning = false;
    }

    public void startBaseline(string LevelID)
    {
        this.LevelID = LevelID;
        LvlFolderDestination = ExpFolderDestination + "\\" + LevelID;
        System.IO.Directory.CreateDirectory(LvlFolderDestination);
    }

    public void startLevel(string LevelID)
    {
        this.LevelID = LevelID;
        LvlFolderDestination = ExpFolderDestination + "\\" + LevelID;
        System.IO.Directory.CreateDirectory(LvlFolderDestination);

        // Initialize Logs
        async = new AsynchronousLog(LvlFolderDestination + "\\" + "AsyncLog.csv");
        sync = new SynchronousLog(LvlFolderDestination + "\\" + "SyncLog.csv");

        async.initializeLogging();
        sync.initializeLogging();

        isLevelRunning = true;

        writerThread = new Thread(writerLoggerThread);
        writerThread.Start();
    }

    public void terminateLoggerThread()
    {
        isLevelRunning = false;
        if(writerThread != null)
        {
            Debug.Log("Closing Logging Thread...");
            while (threadRunning)
            {
                Debug.Log("Waiting for Async/Sync Log thread to close..");

                Debug.Log("AsyncBuffer has " + asyncBuffer.Count);
                Debug.Log("SyncBuffer has " + syncBuffer.Count);

                if(asyncBuffer.Count == 0 && syncBuffer.Count == 0)
                {
                    writerThread.Abort();
                    threadRunning = false;
                }

                Thread.Sleep(100);
            }
            writerThread.Join();
            Debug.Log("Thread has Closed Successfully...");
            writerThread = null;
        }
        else
        {
            Debug.Log("Logging Thread Succesfully Terminated...");
        }
    }

    public void writeAsyncToFile(AsyncEvent e)
    {
        if (isLevelRunning)
        {
            //async.log(e);
            asyncBuffer.Enqueue(e);
        }
    }

    public void writeSyncToFile(SyncEvent e)
    {
        if (isLevelRunning)
        {
            //sync.log(e);
            syncBuffer.Enqueue(e);
        }
    }

    public void endLevel()
    {
        isLevelRunning = false;

        // End Level Logging
        //async.closeLogging();
    }

    public string getExpFolderDestination()
    {
        return this.ExpFolderDestination;
    }

    public string getLvlFolderDestination()
    {
        return this.LvlFolderDestination;
    }

    public string getBaselineFolderDestination()
    {
        return this.baselineFolderDestination;
    }

    public void writerLoggerThread()
    {
        threadRunning = true;
        while (isLevelRunning)
        {
            // Check buffers to write
            if (asyncBuffer.Count > 0)
            {
                async.log(asyncBuffer.Dequeue());
            }

            if (syncBuffer.Count > 0)
            {
                sync.log(syncBuffer.Dequeue());
            }
        }

        // Before quitting check buffers for remaining items
        while(asyncBuffer.Count > 0)
        {
            async.log(asyncBuffer.Dequeue());
        }

        while(syncBuffer.Count > 0)
        {
            sync.log(syncBuffer.Dequeue());
        }

        //async.closeLogging();
        //sync.closeLogging();
        threadRunning = false;
    }
}
