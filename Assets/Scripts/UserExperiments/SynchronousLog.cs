using UnityEngine;
using System.Collections;
using System.IO;

public class SynchronousLog {

    public string logPath;
    public bool isLogging;

    public SynchronousLog(string path)
    {
        isLogging = false;
        this.logPath = path;
        initializeLogging();
    }

    // Initialize Path
    public void initializeLogging()
    {
        // this.w = File.AppendText(logPath);
        isLogging = true;
    }

    public void log(SyncEvent e)
    {
        if (isLogging)
        {
            using (StreamWriter sw = File.AppendText(logPath))
            {
                sw.WriteLine(e.createLog());
            }
        }
    }

    public void closeLogging()
    {
        isLogging = false;
    }

}
