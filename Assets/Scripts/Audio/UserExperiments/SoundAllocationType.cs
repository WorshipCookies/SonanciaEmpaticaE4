using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProjectMaze.Util;
using System.Linq;
using System;

public class SoundAllocationType {

	public enum Allocation
    {
        HIGHTENSION_HIGHRANK, // Highest Tension Room gets the Highest chosen Tense Sound
        HIGHTENSION_LOWRANK, // Highest Tension Room get the Lowest chosen Tense Sound
        GRANULAR,
        RANDOM // Randomly allocate sounds
    }

    private Dictionary<int, SoundFileInfo> allocatedSound;

    public SoundAllocationType(Allocation type, SoundPickingType pType, SoundBank clipbank, LevelObject lvl)
    {
        allocatedSound = new Dictionary<int, SoundFileInfo>();
        switch (type)
        {
            case Allocation.HIGHTENSION_HIGHRANK:
                highTensionHighRank(pType, clipbank, lvl);
                break;
            case Allocation.HIGHTENSION_LOWRANK:
                highTensionLowRank(pType, clipbank, lvl);
                break;
            case Allocation.GRANULAR:
                granularTension(pType, clipbank, lvl);
                break;
            case Allocation.RANDOM:
                randomTension(pType, clipbank, lvl);
                break;
        }
    }

    private void highTensionHighRank(SoundPickingType pType, SoundBank clipbank, LevelObject lvl)
    {
        List<SoundFileInfo> picked = new List<SoundFileInfo>(pType.getPickedClips());

        Dictionary<int, MyTuple> roomTensions = SoundPickingType.returnRoomTensions(clipbank, lvl);

        Dictionary <int, MyTuple> aux = roomTensions.OrderByDescending(i => i.Value).ToDictionary(i => i.Key, i => i.Value);
        List<MyTuple> tupleList = new List<MyTuple>(aux.Values);

        picked = picked.OrderByDescending(x => x.getRankValue()).ToList();

        for(int i = 0; i < picked.Count; i++)
        {
            allocatedSound.Add(tupleList[i].getID(), picked[i]);
        }

    }

    private void highTensionLowRank(SoundPickingType pType, SoundBank clipbank, LevelObject lvl)
    {
        List<SoundFileInfo> picked = new List<SoundFileInfo>(pType.getPickedClips());

        Dictionary<int, MyTuple> roomTensions = SoundPickingType.returnRoomTensions(clipbank, lvl);
        Dictionary<int, MyTuple> aux = roomTensions.OrderBy(i => i.Value).ToDictionary(i => i.Key, i => i.Value);
        List<MyTuple> tupleList = new List<MyTuple>(aux.Values);

        picked = picked.OrderByDescending(x => x.getRankValue()).ToList();

        for (int i = 0; i < picked.Count; i++)
        {
            allocatedSound.Add(tupleList[i].getID(), picked[i]);
        }
    }

    private void granularTension(SoundPickingType pType, SoundBank clipbank, LevelObject lvl)
    {
        List<SoundFileInfo> picked = new List<SoundFileInfo>(pType.getPickedClips());
        Dictionary<int, MyTuple> roomTensions = SoundPickingType.returnRoomTensions(clipbank, lvl);

        while(picked.Count > 0)
        {
            double currVal = double.MaxValue;
            int pickedRoom = -1;
            int pickedClip = -1;
            for (int j = 0; j < picked.Count; j++)
            {
                foreach (int i in roomTensions.Keys)
                {
                    if (!allocatedSound.ContainsKey(i))
                    {
                        double dist = Math.Abs(picked[j].getRankValue() - roomTensions[i].getValue());
                        if (dist < currVal)
                        {
                            currVal = dist;
                            pickedRoom = roomTensions[i].getID();
                            pickedClip = j;
                        }
                    }
                }
            }
            if (pickedRoom >= 0)
            {
                allocatedSound.Add(pickedRoom, picked[pickedClip]);
                picked.RemoveAt(pickedClip);
            }
            else
                Debug.Log("ERROR in Granular Sound Allocation Type");
        }
    }

    private void randomTension(SoundPickingType pType, SoundBank clipbank, LevelObject lvl)
    {
        List<SoundFileInfo> picked = new List<SoundFileInfo>(pType.getPickedClips());

        int currRoom = 0;
        while(picked.Count > 0)
        {
            int rand = UnityEngine.Random.Range(0, picked.Count);
            allocatedSound.Add(currRoom, picked[rand]);

            picked.RemoveAt(rand);
            currRoom++;
        }

    }

    public Dictionary<int, SoundFileInfo> getSoundAllocation()
    {
        return allocatedSound;
    }
}
