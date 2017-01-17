using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProjectMaze.Util;

public class ZingerDictionary {

    private Dictionary<int, List<Zinger>> zingerDictionary;
    private List<ZingerClip> clipList;

    private List<ZingerClip> inUse;

    private string abspath = "Assets\\Resources\\SoundStinger\\";
    public static string path = "SoundStinger/";

    public ZingerDictionary()
    {
        string[] files = Directory.GetFiles(abspath, "*.wav");
        zingerDictionary = new Dictionary<int, List<Zinger>>();

        clipList = new List<ZingerClip>();
        inUse = new List<ZingerClip>();

        foreach (string i in files)
        {
            string[] parsed = i.Split('\\');
            string fileName = parsed[parsed.Length - 1].Split('.')[0];
            Debug.Log(path + fileName);
            clipList.Add(new ZingerClip(path + fileName));
        }

    }

    public void addZinger(int roomID, Zinger z)
    {
        if(clipList.Count < 1)
        {
            reloadClips();
        }

        if (zingerDictionary.ContainsKey(roomID))
        {
            z.attachAudio(getRandomClip().getClip());
            zingerDictionary[roomID].Add(z);
        }
        else
        {
            zingerDictionary[roomID] = new List<Zinger>();
            z.attachAudio(getRandomClip().getClip());
            zingerDictionary[roomID].Add(z);
        }
    }

    public bool deleteZinger(int roomID, int zingID)
    {
        if(zingerDictionary.ContainsKey(roomID) && zingerDictionary[roomID].Count > 0)
        {
            for(int i = 0; i < zingerDictionary[roomID].Count; i++)
            {
                if(zingerDictionary[roomID][i].getID() == zingID)
                {
                    zingerDictionary[roomID].RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    public List<Zinger> getZingersOfRoom(int roomID)
    {
        if (zingerDictionary.ContainsKey(roomID))
        {
            return zingerDictionary[roomID];
        }
        else
        {
            return null;
        }
    }

    public Zinger getZingerByID(int zingID)
    {
        foreach(int id in zingerDictionary.Keys)
        {
            foreach(Zinger z in zingerDictionary[id])
            {
                if(z.getID() == zingID)
                {
                    return z;
                }
            }
        }
        return null;
    }

    public void reloadClips()
    {
        clipList = new List<ZingerClip>(inUse);
        inUse = new List<ZingerClip>();
    }

    public ZingerClip getRandomClip()
    {
        int choice = MyRandom.getRandom().random().Next(clipList.Count);
        ZingerClip chosen = clipList[choice];
        inUse.Add(chosen);
        clipList.RemoveAt(choice);
        return chosen;
    }

    public bool keyExists(int roomID)
    {
        return zingerDictionary.ContainsKey(roomID);
    }

}
