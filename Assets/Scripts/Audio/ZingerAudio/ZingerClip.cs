using UnityEngine;
using System.Collections;

public class ZingerClip {

    private string fileName;
    private AudioClip clip;
    
    public ZingerClip(string fileName)
    {
        clip = Resources.Load<AudioClip>(fileName);
        this.fileName = fileName;
        
    }

    public string getFileName()
    {
        return fileName;
    }

    public AudioClip getClip()
    {
        return clip;
    }

}
