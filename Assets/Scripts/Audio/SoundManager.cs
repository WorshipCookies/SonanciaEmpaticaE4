using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.Util;
using System;

public class SoundManager {

    public AudioSource lowpass;
    public AudioSource reverb;
    public AudioSource echo;
    public AudioSource distortion;
    public AudioSource chorus;
    public AudioSource flange;
    public AudioSource highpass;
    public AudioSource pitchshift;


    private List<AudioSourceInfo> sourceList;

    private SoundBank clipbank;
    private AudioSourceBank sourcebank;

    public static float VOLUME_MIN_DISTANCE = 0f;
    public static float VOLUME_MAX_DISTANCE = 1f;

    public static float BLEND_MIN_DISTANCE = 0f;
    public static float BLEND_MAX_DISTANCE = 1f;

	// Use this for initialization
    public SoundManager(LevelObject lvl)
    {
        SoundPlayType.PlayType playType = (SoundPlayType.PlayType)LevelBuilder.soundplay;

        clipbank = new SoundBank();
        sourcebank = new AudioSourceBank();

        if (playType == SoundPlayType.PlayType.ALL_ON || playType == SoundPlayType.PlayType.ROOMS_ONLY)
        {
            List<List<int>> subpaths = lvl.getGeneticMap().getAllSubPaths();
            List<MyTuple> mainpath = lvl.getGeneticMap().getAnxietyMap();

            // Read Parameters from Level Builder
            Rank.RankType rankType = (Rank.RankType)LevelBuilder.soundrank;
            SoundAllocationType.Allocation allocationType = (SoundAllocationType.Allocation)LevelBuilder.soundallocation;
            SoundPickingType.PickingType pickType = (SoundPickingType.PickingType)LevelBuilder.soundpick;

            sourcebank.assingSoundFilesToExperiment(clipbank, lvl, rankType, playType, allocationType, pickType);

            //sourcebank.assignSoundFilesToTension(clipbank, lvl);
            sourcebank.playAllSources(); // Now loop all sources in the source bank

            string filename = LevelBuilder.levelName + " - " + Convert.ToString(rankType) + ".csv";
            sourcebank.writeToFileSourceBank(filename);

            //initializeRandomSourceBank(lvl);
            //sourcebank.assignRandomSoundFiles(clipbank);
            //sourcebank.randomizeEffectParameters();
            //initializeSoundPlay();
        }


    }

    // THIS WILL NEED TO CHANGE IN THE FUTURE TO TAKE INTO ACCOUNT THE TENSION CURVE
    public void initializeRandomSourceBank(LevelObject lvl)
    {
        sourcebank = new AudioSourceBank();
        for (int i = 0; i < lvl.getTotalRoomNum(); i++)
        {
            // Position of future Audio Source
            int roomID = i+1; // The real room ID

            string effectName = AudioSourceBank.getRandomEffect();
            AudioSource source = (AudioSource) GameObject.Instantiate(Resources.Load<AudioSource>("SoundObjects/" + effectName));
            source.transform.position = LevelBuilder.calcRoomCentroids(lvl, roomID);
            float[] dist = getDistance(source.transform.position, roomID, lvl);//distanceBetweenDoors(source.transform.position, roomID, lvl);
            source.minDistance = dist[0];
            source.maxDistance = dist[1]; // -(dist[0] * 0.5f);
            sourcebank.addSource(effectName, source, roomID); // returns the index of the source in the bank
        }
    }

    public float[] distanceBetweenDoors(Vector3 centroid, int roomID, LevelObject lvl)
    {
        List<Vector3> door_centroids = new List<Vector3>();
        float max_dist = 0f;
        float min_dist = float.MaxValue;
        foreach (DoorObject d in lvl.getDoors())
        {
            float new_dist = 0f;
            if (d.getRoomID1() == roomID)
            {
                new_dist = calcDistance(centroid, lvl.getTiles(d.getTileID2()).transform.position);
            }
            else if (d.getRoomID2() == roomID)
            {
                new_dist = calcDistance(centroid, lvl.getTiles(d.getTileID1()).transform.position);
            }

            if (new_dist > max_dist)
            {
                max_dist = new_dist;
            }
            
            if (new_dist < min_dist && new_dist > 0)
            {
                min_dist = new_dist;
            }

        }
        float[] f = {min_dist, max_dist};
        return f;
    }

    public static float[] getDistance(Vector3 centroid, int roomID, LevelObject lvl)
    {
        List<int> extremities = lvl.getAllAdjacentTiles(roomID);
        float min_dist = float.MaxValue;
        float max_dist = 0f;
        foreach (int i in extremities)
        {
            float new_dist = calcDistance(centroid, lvl.getTiles(i).transform.position);
            if (new_dist > max_dist)
            {
                max_dist = new_dist;
            }

            if (new_dist < min_dist && new_dist > 0)
            {
                min_dist = new_dist;
            }
        }
        float[] f = { min_dist, max_dist };
        return f;
    }

    public static float calcDistance(Vector3 v1, Vector3 v2)
    {
        return Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.z - v2.z), 2));
    }

    public AudioSourceBank getSourceBank()
    {
        return sourcebank;
    }

    public SoundBank getSoundBank()
    {
        return clipbank;
    }

    public void initializeSoundPlay()
    {
        sourcebank.playAllSources();
    }

    public void muteAllSources()
    {
        sourcebank.muteAllSources();
    }

    public void unmuteAllSources()
    {
        sourcebank.unmuteAllSources();
    }

}
