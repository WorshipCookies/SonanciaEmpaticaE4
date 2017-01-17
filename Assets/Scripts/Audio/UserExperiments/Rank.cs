using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class Rank {

    public string globalRankFolder = System.IO.Directory.GetCurrentDirectory() + "\\GlobalRankData";

    public enum RankType
    {
        PARTICIPANT_TENSION_GLOBALRANK,
        PARTICIPANT_AROUSAL_GLOBALRANK,
        PARTICIPANT_VALENCE_GLOBALRANK,
        PREDICTIVE_TENSION_TOTAL_GLOBALRANK,
        PREDICTIVE_AROUSAL_TOTAL_GLOBALRANK,
        PREDICTIVE_VALENCE_TOTAL_GLOBALRANK,
        PREDICTIVE_TENSION_SOUNDONLY_GLOBALRANK,
        PREDICTIVE_AROUSAL_SOUNDONLY_GLOBALRANK,
        PREDICTIVE_VALENCE_SOUNDONLY_GLOBALRANK,
        RANDOM_GLOBALRANK
    }

    private Dictionary<int, double> rankData;

    public Rank(RankType type)
    {
        rankData = new Dictionary<int, double>();

        switch (type)
        {
            case RankType.PARTICIPANT_TENSION_GLOBALRANK:
                loadRankData("participantTensionGlobalRank.csv");
                break;
            case RankType.PARTICIPANT_AROUSAL_GLOBALRANK:
                loadRankData("participantArousalGlobalRank.csv");
                break;
            case RankType.PARTICIPANT_VALENCE_GLOBALRANK:
                loadRankData("participantValenceGlobalRank.csv");
                break;
            case RankType.PREDICTIVE_TENSION_SOUNDONLY_GLOBALRANK:
                loadRankData("TensionRankSVM.csv");
                break;
            case RankType.RANDOM_GLOBALRANK:
                loadRankData("randomTensionModel.csv");
                break;
            // ADD MORE in case
            default:
                Debug.Log("Type Does not Exist");
                break;
        }
    }

    public Rank(RankType type, SoundBank clipbank) : this(type)
    {
        assignRankValues(clipbank);
    }

    public void loadRankData(string path)
    {
        string fullPath = globalRankFolder + "\\" + path;

        using (StreamReader sr = new StreamReader(fullPath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] s = line.Split(',');
                rankData.Add(Convert.ToInt32(s[0]), Convert.ToDouble(s[s.Length - 1])); // Keep Sound ID and its Rank Data.
            }
        }
    }

    public double getValue(int id)
    {
        return rankData[id];
    }

    // Assigning Rank Values according to what was set.
    public void assignRankValues(SoundBank clipbank)
    {
        foreach(int i in rankData.Keys)
        {
            clipbank.getFileInfo(i).setRankValue(rankData[i]);
        }
    }

    public Dictionary<int, double> getOrderedRanking()
    {
        Dictionary<int, double> aux = rankData.OrderByDescending(i => i.Value).ToDictionary(i => i.Key, i => i.Value);
        return aux;
    }

    public Dictionary<int, double> getOrderedRankingDescending()
    {
        Dictionary<int, double> aux = rankData.OrderBy(i => i.Value).ToDictionary(i => i.Key, i => i.Value);
        return aux;
    }

    public List<int> getOrderIDOnly()
    {
        Dictionary<int,double> aux = rankData.OrderByDescending(i => i.Value).ToDictionary(i => i.Key, i => i.Value);
        return new List<int>(aux.Keys);
    }

    public List<int> getOrderIDOnlyDescending()
    {
        Dictionary<int, double> aux = rankData.OrderBy(i => i.Value).ToDictionary(i => i.Key, i => i.Value);
        return new List<int>(aux.Keys);
    }
}
