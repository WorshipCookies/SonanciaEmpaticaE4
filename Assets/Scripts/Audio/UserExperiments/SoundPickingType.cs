using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProjectMaze.Visual;
using ProjectMaze.Util;
using ProjectMaze.GeneticAlgorithm;
using System;

public class SoundPickingType {

	public enum PickingType
    {
        EQUIDISTANT,
        GRANULAR,
        HIGH_RANK,
        LOW_RANK
    }

    private List<SoundFileInfo> pickedClips; // Highest to Lowest Rank

    public SoundPickingType(SoundBank clipbank, PickingType pType, Rank r, LevelObject lvl)
    {
        switch (pType)
        {
            case PickingType.EQUIDISTANT:
                equidistantPicking(clipbank, r, lvl);
                break;
            case PickingType.GRANULAR:
                granularPicking(clipbank, r, lvl);
                break;
            case PickingType.HIGH_RANK:
                highRankingPicking(clipbank, r, lvl);
                break;
            case PickingType.LOW_RANK:
                lowerRankingPicking(clipbank, r, lvl);
                break;
            default:
                Debug.Log("Error, picking type does not exist!");
                break;
        }
    }

    public void equidistantPicking(SoundBank clipbank, Rank r, LevelObject lvl)
    {

        // Pick Sounds First In Equidistant Fashion
        List<int> orderedRanks = r.getOrderIDOnly();
        pickedClips = new List<SoundFileInfo>();
        int numOfSounds = lvl.getTotalRoomNum();

        int equidistance = orderedRanks.Count / numOfSounds;

        // To avoid rouding errors. Make sure equidistance is never to high
        while ((equidistance * numOfSounds) > orderedRanks.Count - 1)
        {
            equidistance--;
        }

        // Pick Sounds based on the equidistance
        for(int i = 0; i < numOfSounds; i++)
        {
            int pickedID = orderedRanks[i * equidistance];
            pickedClips.Add(clipbank.getFileInfo(pickedID));
        }
    }

    public void highRankingPicking(SoundBank clipbank, Rank r, LevelObject lvl)
    {
        // Pick highest ranked sounds.
        List<int> orderedRanks = r.getOrderIDOnly();
        pickedClips = new List<SoundFileInfo>();
        int numOfSounds = lvl.getTotalRoomNum();

        for(int i = 0; i < numOfSounds; i++)
        {
            pickedClips.Add(clipbank.getFileInfo(orderedRanks[i]));
        }
    }

    public void lowerRankingPicking(SoundBank clipbank, Rank r, LevelObject lvl)
    {
        // Pick lowest ranked sounds.
        List<int> orderedRanks = r.getOrderIDOnlyDescending();
        pickedClips = new List<SoundFileInfo>();
        int numOfSounds = lvl.getTotalRoomNum();

        for (int i = 0; i < numOfSounds; i++)
        {
            pickedClips.Add(clipbank.getFileInfo(orderedRanks[i]));
        }
    }

    public void granularPicking(SoundBank clipbank, Rank r, LevelObject lvl)
    {
        pickedClips = new List<SoundFileInfo>();

        Dictionary<int, MyTuple> tensionVal = returnRoomTensions(clipbank, lvl);

        // Pick Sounds based on the distance of tension and the Global Ranking.
        List<int> pickedSoundID = new List<int>();
        List<int> pickedRoomID = new List<int>();
        while(pickedSoundID.Count < tensionVal.Count)
        {
            double minDist = double.MaxValue;
            int chosenID = -1;
            int chosenRoom = -1;
            foreach (int roomId in tensionVal.Keys)
            {
                if (!pickedRoomID.Contains(roomId))
                {
                    foreach (int soundID in r.getOrderedRanking().Keys)
                    {
                        if (!pickedSoundID.Contains(soundID))
                        {
                            double dist = Math.Abs(tensionVal[roomId].getValue() - r.getOrderedRanking()[soundID]);
                            if (dist < minDist)
                            {
                                minDist = dist;
                                chosenID = soundID;
                                chosenRoom = roomId;
                            }
                        }
                    }
                }
            }
            if (chosenID >= 0 && chosenRoom >= 0)
            {
                pickedSoundID.Add(chosenID);
                pickedRoomID.Add(chosenRoom);
            }
            else
            {
                Debug.Log("There was a bug in the granular picking algorithm!");
            }
        }
        // Got Sound IDs place them in the picked list
        foreach(int i in pickedSoundID)
        {
            pickedClips.Add(clipbank.getFileInfo(i));
        }

    }

    public List<SoundFileInfo> getPickedClips()
    {
        return pickedClips;
    }

    /// <summary>
    /// Returns the tension value of each room, including alternative paths.
    /// </summary>
    /// <param name="clipbank">The Soundbank being used.</param>
    /// <param name="lvl">The LevelObject that was generated (or Loaded).</param>
    /// <returns>MyTuple dictionary with each tension value of each room.</returns>
    public static Dictionary<int,MyTuple> returnRoomTensions(SoundBank clipbank, LevelObject lvl)
    {
        List<List<Room>> pPaths = lvl.getAllPossiblePaths();

        // Get Tension Values for each path
        // Initialize the tension 
        Dictionary<int, MyTuple> tensionOfPathsComplete = new Dictionary<int, MyTuple>();
        for (int i = 0; i < lvl.getTotalRoomNum(); i++)
        {
            tensionOfPathsComplete.Add(i, new MyTuple(i, -1.0));
        }

        // Calculate Tension for each of the paths (keep the longest path which is the main path).
        List<List<MyTuple>> eachPathTension = new List<List<MyTuple>>();
        foreach (List<Room> path in pPaths)
        {
            List<MyTuple> tPath = new List<MyTuple>();
            foreach (Room room in path)
            {
                // Add Tension Values to the Path
                tPath.Add(new MyTuple(room.getID(),
                    AnxietySuspenseFitness.getTensionValueOfRoom(room.getID(), lvl.getGeneticMap())));
            }
            // Refine the tension values (based on the previous rooms etc.)
            AnxietySuspenseFitness.refineAnxietyMap(tPath);
            eachPathTension.Add(tPath);
        }

        // Fill the Anxiety Map first -- This is the critical path so it takes priority 
        // no matter how many paths exist.
        foreach (MyTuple t in lvl.getGeneticMap().getAnxietyMap())
        {
            tensionOfPathsComplete[t.getID()].setValue(t.getValue());
        }

        // Fill the rest with the remainder paths. Priority is given to the shortest paths 
        // as these tend to be the most common alternative paths.
        foreach (List<MyTuple> path in eachPathTension)
        {
            foreach (MyTuple t in path)
            {
                if (tensionOfPathsComplete[t.getID()].getValue() < 0)
                {
                    tensionOfPathsComplete[t.getID()].setValue(t.getValue());
                }
            }
        }

        // In case there are rooms that do not have a path set these to 0.0 to avoid errors.
        foreach (int i in tensionOfPathsComplete.Keys)
        {
            if (tensionOfPathsComplete[i].getValue() < 0)
            {
                tensionOfPathsComplete[i].setValue(0);
            }
        }
        return tensionOfPathsComplete;
    }

    public static void normalizeTension(Dictionary<int, MyTuple> tensionVal)
    {
        double minValue = 0.0;
        double maxValue = 0.0;

        // Get Maximum value for normalization
        foreach (int i in tensionVal.Keys)
        {
            if (tensionVal[i].getValue() > maxValue)
            {
                maxValue = tensionVal[i].getValue();
            }
        }

        foreach (int i in tensionVal.Keys)
        {
            double x = tensionVal[i].getValue();
            double normVal = (2 * (x-minValue) / (maxValue-minValue) ) - 1;
            tensionVal[i].setValue(normVal);
        }
    }


}
