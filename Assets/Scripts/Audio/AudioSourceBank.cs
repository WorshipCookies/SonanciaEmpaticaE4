using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProjectMaze.Util;
using ProjectMaze.GeneticAlgorithm;
using System;

public class AudioSourceBank {

    private List<AudioSourceInfo> sourcebank;

    private static string[] availableEffects = { "LowPassSource", "ReverbSource", "EchoSource", "DistortionSource", "ChorusSource", "PitchShiftSource", "HighPassSource", "FlangeSource", "NoEffectSource" };

    public AudioSourceBank()
    {
        this.sourcebank = new List<AudioSourceInfo>();
    }

    public int addSource(string effectName, AudioSource source, int roomID)
    {
        if (effectName == "LowPassSource")
        {
            sourcebank.Add(new AudioSourceLowPass(effectName, source, roomID));
            return sourcebank.Count - 1;
        }
        else if (effectName == "ReverbSource")
        {
            sourcebank.Add(new AudioSourceReverb(effectName, source, roomID));
            return sourcebank.Count - 1;
        }
        else if (effectName == "EchoSource")
        {
            sourcebank.Add(new AudioSourceEcho(effectName, source, roomID));
            return sourcebank.Count - 1;
        }
        else if (effectName == "DistortionSource")
        {
            sourcebank.Add(new AudioSourceDistortion(effectName, source, roomID));
            return sourcebank.Count - 1;
        }
        else if (effectName == "ChorusSource")
        {
            sourcebank.Add(new AudioSourceChorus(effectName, source, roomID));
            return sourcebank.Count - 1;
        }
        else if (effectName == "PitchShiftSource")
        {
            sourcebank.Add(new AudioSourcePitchShift(effectName, source, roomID));
            return sourcebank.Count - 1;
        }
        else if (effectName == "HighPassSource")
        {
            sourcebank.Add(new AudioSourceHighPass(effectName, source, roomID));
            return sourcebank.Count - 1;
        }
        else if (effectName == "FlangeSource")
        {
            sourcebank.Add(new AudioSourceFlange(effectName, source, roomID));
            return sourcebank.Count - 1;
        }
        else if (effectName == "NoEffectSource")
        {
            sourcebank.Add(new AudioSourceNoEffect(effectName, source, roomID));
            return sourcebank.Count - 1;
        }
        return -1;
    }

    //public int addSource(int roomID, LevelObject lvl)
    //{
    //    string effectName = AudioSourceBank.getRandomEffect();
    //    AudioSource source = (AudioSource)GameObject.Instantiate(Resources.Load<AudioSource>("SoundObjects/" + effectName));
    //    source.transform.position = SoundManager.calcRoomCentroids(lvl, roomID);
    //    float[] dist = SoundManager.getDistance(source.transform.position, roomID, lvl);
    //    source.minDistance = dist[0];
    //    source.maxDistance = dist[1];
    //    return addSource(effectName, source, roomID); // returns the index of the source in the bank
    //}

    public int addSource(int roomID, LevelObject lvl)
    {
        // THIS OBTAINS A RANDOM SOURCE EFFECT
        //string effectName = AudioSourceBank.getRandomEffect();
        string effectName = "NoEffectSource";

        AudioSource source = (AudioSource)GameObject.Instantiate(Resources.Load<AudioSource>("SoundObjects/" + effectName));
        VA_AudioSource va = source.gameObject.AddComponent<VA_AudioSource>();
        va.Shape = lvl.getRoomMeshList()[roomID][0].GetComponent<VA_Mesh>();

        va.Volume = true;
        va.VolumeMinDistance = SoundManager.VOLUME_MIN_DISTANCE;
        va.VolumeMaxDistance = SoundManager.VOLUME_MAX_DISTANCE;

        va.Blend = true;
        va.BlendMinDistance = SoundManager.BLEND_MIN_DISTANCE;
        va.BlendMaxDistance = SoundManager.BLEND_MAX_DISTANCE;

        return addSource(effectName, source, roomID);
    }

    public static string getRandomEffect()
    {
        return availableEffects[myRandom.Next(0, availableEffects.Length - 1)];
    }

    public void assignRandomSoundFiles(SoundBank clipbank)
    {
        foreach(AudioSourceInfo a in sourcebank)
        {
            a.source.clip = Resources.Load<AudioClip>(clipbank.getRandom());
        }
    }

    public void assignSoundFileToSource(int idSource, int idClip, SoundBank clipbank)
    {
        sourcebank[idSource].source.clip = Resources.Load<AudioClip>(clipbank.getFile(idClip));
    }

    public void assignSoundFilesToTension(SoundBank clipbank, LevelObject lvl)
    {
        // First Phase of the audio selection -- Roulette Wheel Selection Method
        List<SoundFileInfo> pickedFiles = rouletteClipSelection(clipbank, lvl.getTotalRoomNum());

        // Order the Files according to the tension curve
        List<MyTuple> roomPath = new List<MyTuple>(lvl.getGeneticMap().getAnxietyMap());
        // Distribute the files according to intensity and path.
        while (roomPath.Count > 0)
        {
            int max_index = getMaxIntensityValue(roomPath);
            MyTuple max_tuple = roomPath[max_index];

            int max_audio_index = getMinIntensityAudioFile(pickedFiles);
            SoundFileInfo max_file = pickedFiles[max_audio_index];

            assignSoundFileToSource(addSource( max_tuple.getID() + 1, lvl ), max_file.getSoundFileID(), clipbank); // Creates a Source with addSource, and then associates the clip to that source.
            
            roomPath.RemoveAt(max_index);
            pickedFiles.RemoveAt(max_audio_index);
        }

        // Distribute the remaining sound files to the rest of the level.
        foreach(int i in lvl.roomsOutsidePath())
        {
            int rand_select = MyRandom.getRandom().random().Next(pickedFiles.Count);
            assignSoundFileToSource(addSource(i, lvl), pickedFiles[rand_select].getSoundFileID(), clipbank);
            pickedFiles.RemoveAt(rand_select);
        }


        // Once this is done we'll randomize the Effect Parameters.
        randomizeEffectParameters();
    }

    /**
     *  ---- THIS SECTION OF CODE HAS BEEN UPDATED AND PERTAINS TO THE NEW UPDATES ON THE AUDIO SYSTEM ----
     *  
     *  THIS NEW VERSION WORKS WITH GLOBAL RANK FROM USER EXPERIMENTS AND PREDICTIVE RANK DATA OBTAINED 
     *  FROM RANK SVMS
     *  
     * */

    public void assingSoundFilesToExperiment(SoundBank clipbank, LevelObject lvl, int rType,
        int playType, int aType, int pickType)
    {
        assingSoundFilesToExperiment(clipbank, lvl, (Rank.RankType)rType, (SoundPlayType.PlayType)playType,
            (SoundAllocationType.Allocation)aType, (SoundPickingType.PickingType)pickType);
    }

    public void assingSoundFilesToExperiment(SoundBank clipbank, LevelObject lvl, Rank.RankType rType, 
        SoundPlayType.PlayType playType, SoundAllocationType.Allocation aType, 
        SoundPickingType.PickingType pickType)
    {
        // DO STUFF HERE
        // Create the global rank data and assign them to clipbank
        Rank r = new Rank(rType, clipbank);

        // Pick Sounds
        SoundPickingType pickingType = new SoundPickingType(clipbank, pickType, r, lvl);

        // Allocate Sounds
        SoundAllocationType allocationType = new SoundAllocationType(aType, pickingType, clipbank, lvl);

        // Assign the Actual Files to the Source.
        for(int i = 0; i < lvl.getTotalRoomNum(); i++)
        {
            assignSoundFileToSource(addSource(i+1, lvl), allocationType.getSoundAllocation()[i].getSoundFileID(), clipbank);
        }
    }

    /**
     * ---- END OF THE NEW CODE AUDIO SYSTEM. ----
     * */

    public AudioSourceInfo getSourceByIndex(int index)
    {
        return sourcebank[index];
    }

    public void randomizeEffectParameters()
    {
        foreach (AudioSourceInfo a in sourcebank)
        {
            a.randomParams();
        }
    }

    public void playAllSources()
    {
        foreach (AudioSourceInfo a in sourcebank)
        {
            a.getSource().Play();
        }
    }

    public void muteAllSources()
    {
        foreach(AudioSourceInfo a in sourcebank)
        {
            a.getSource().mute = true;
        }
    }

    public void unmuteAllSources()
    {
        foreach(AudioSourceInfo a in sourcebank)
        {
            a.getSource().mute = false;
        }
    }

    public int getMaxIntensityValue(List<MyTuple> values)
    {
        if (values.Count > 0)
        {
            MyTuple max = values[0];
            int max_index = 0;
            for (int i = 0; i < values.Count; i++)
            {
                if (max.getValue() <= values[i].getValue())
                {
                    max = values[i];
                    max_index = i;
                }
            }
            return max_index;
        }
        else
        {
            return -1;
        }
    }

    public int getMinIntensityAudioFile(List<SoundFileInfo> shuffler)
    {
        if (shuffler.Count > 0)
        {
            SoundFileInfo max = shuffler[0];
            int max_index = 0;
            for (int i = 0; i < shuffler.Count; i++)
            {
                if (max.getTensionValue() >= shuffler[i].getTensionValue())
                {
                    max = shuffler[i];
                    max_index = i;
                }
            }
            return max_index;
        }
        else
        {
            return -1;
        }
    }

    // Roulette Wheel Selection Method Function.
    public List<SoundFileInfo> rouletteClipSelection(SoundBank clipbank, int totalClips)
    {
        List<SoundFileInfo> pickedClips = new List<SoundFileInfo>();
        Dictionary<int, List<SoundFileInfo>> clips = clipbank.soundClipsByInstrumentID();

        // Creating the roulette. Consists of the Instrument ID and the probability of being chosen -- Initialized at 1.
        List<MyTuple> instRoulette = new List<MyTuple>();
        foreach (int id in clips.Keys)
        {
            instRoulette.Add(new MyTuple(id, 1.0));
        }

        // The Clip Selection Cycle
        for (int i = 0; i < totalClips; i++)
        {
            // Total available to spin
            double totalValue = 0;
            foreach (MyTuple tup in instRoulette)
            {
                totalValue += tup.getValue();
            }

            double spinValue = myRandom.Next(0f, (float)totalValue);
          
            // initialization
            int current_index = 0;
            double rouletteSpinner = instRoulette[current_index].getValue();
            
            // Spin the Roulette Wheel
            while (rouletteSpinner < spinValue)
            {
                current_index++;
                rouletteSpinner += instRoulette[current_index].getValue();
            }

            // Instrument Picked, now pick a Clip randomly!
            int chosenID = instRoulette[current_index].getID();

            if (!clips.ContainsKey(chosenID))
            {
                Debug.Log("Bug Found!");
            }

            int randomClip = myRandom.Next(0, clips[chosenID].Count-1);

            pickedClips.Add(clips[chosenID][randomClip]); // Add clip to the chosen list
            
            // Remove it from the List of available clips. If ID has no more available clips.
            clips[chosenID].RemoveAt(randomClip);
            

            if (clips[chosenID].Count <= 0)
            {
                clips.Remove(chosenID);
                instRoulette.RemoveAt(current_index); // Bug Fix Added -- Hopefully it works!
            }
            // To promote variability give a higher chance to instrument ID's not previously chosen. Cut probability of chosen instruments by half!
            else
            {
                double newValue = instRoulette[current_index].getValue() / 2;
                instRoulette[current_index].setValue(newValue);
            }

            // End of Spin! -- Start anew :)
        }
        return pickedClips;
    }

    public void writeToFileSourceBank(string fileName)
    {
        string str = "";
        foreach(AudioSourceInfo s in sourcebank)
        {

            //str += s.getRoomId() + "," + s.get
            string clipName = s.getSource().clip.name;
            clipName = clipName.Split('.')[0];
            string clipID = clipName.Split('_')[4];
            str += Convert.ToString(s.getRoomId()) + "," + clipID + "," + clipName + "\n";
        }
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, true))
        {
            file.WriteLine(str);
        }

    }

}
