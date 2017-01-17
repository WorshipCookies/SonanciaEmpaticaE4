using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SoundBank {

    private Dictionary<int,SoundFileInfo> soundBank;
    private string abspath = "Assets\\Resources\\SoundClips\\";
    private string path = "SoundClips/";

    public SoundBank()
    {
        string[] files = Directory.GetFiles(abspath, "*.wav");

        //int id_counter = 0;
        soundBank = new Dictionary<int, SoundFileInfo>();
        
        foreach (string i in files)
        {
            Debug.Log(i);
            // Old Version.
            //soundBank.Add(id_counter, new SoundFileInfo(id_counter, i));
            //id_counter++;

            SoundFileInfo sf = new SoundFileInfo(i);
            soundBank.Add(sf.getRankValueID(), sf);
        }
    }

    public string getFile(int id)
    {
        return path + soundBank[id].getFileName();
    }

    public string getRandom()
    {
        return path + soundBank[Random.Range(0, soundBank.Count-1)].getFileName();
    }

    // Creates Dictionary object where the Instrument ID points to all available clips associated to that instrument ID --- Usable for the Roulette Function.
    public Dictionary<int, List<SoundFileInfo>> soundClipsByInstrumentID()
    {
        Dictionary<int, List<SoundFileInfo>> instID_clips = new Dictionary<int, List<SoundFileInfo>>();
        foreach (SoundFileInfo sf in soundBank.Values)
        {
            if (instID_clips.ContainsKey(sf.getInstrumentID()))
            {
                instID_clips[sf.getInstrumentID()].Add(sf); // If this ID already exists, add it to the list of clips associated to this instrument ID
            }
            else
            {
                List<SoundFileInfo> newList = new List<SoundFileInfo>(); // If the ID doesn't exist, then create a new list with that instrument ID.
                newList.Add(sf);
                instID_clips.Add(sf.getInstrumentID(), newList);
            }
        }
        return instID_clips;
    } 

    public SoundFileInfo getFileInfo(int id)
    {
        return soundBank[id];
    }

}
