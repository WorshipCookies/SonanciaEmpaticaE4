using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LogisticLoader {

    private static List<string> EXP_LEVELS = new List<string>();
    public static int EXP_ID = -1;
    public static int USR_ID = -1;

    private static int CURRENT_LEVEL = 0;
    private static bool EXP_HAS_FINISHED = false;

    private static string levelPath = System.IO.Directory.GetCurrentDirectory() + "\\ExperimentLevels";

    public static void setParam(List<string> s, int expid, int usrid)
    {
        EXP_LEVELS = s;
        EXP_ID = expid;
        USR_ID = usrid;
    }

    /// <summary>
    /// Loads the next level in the Queue. For reference this is how a level is defined:
    /// [0]: The level to be loaded.
    /// [1]: Rank Model to be Used.
    /// [2]: Sound Picking Algorithm Used.
    /// [3]: Sound Allocation Algorithm Used.
    /// [4]: Sound Playing Type.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="lvlName"></param>
    /// <param name="soundrank"></param>
    /// <param name="soundpick"></param>
    /// <param name="soundallocation"></param>
    /// <param name="soundplay"></param>
    public static void getNextLevel(out string path, out string lvlName, out int soundrank, 
        out int soundpick, out int soundallocation, out int soundplay)
    {
        if (EXP_HAS_FINISHED)
        {
            path = null;
            soundrank = -1;
            soundpick = -1;
            soundallocation = -1;
            soundplay = -1;
            lvlName = null;
        }
        else
        {
            string s = EXP_LEVELS[CURRENT_LEVEL];
            string file = s.Split('.')[0];
            path = levelPath + "\\" + file + ".snc";
            string[] soundparam = s.Split('.');
            soundrank = Convert.ToInt32(soundparam[1]);
            soundpick = Convert.ToInt32(soundparam[2]);
            soundallocation = Convert.ToInt32(soundparam[3]);
            soundplay = Convert.ToInt32(soundparam[4]);
            lvlName = s;
            hasFinished();
        }
    }

    private static void hasFinished()
    {
        CURRENT_LEVEL++;
        if(CURRENT_LEVEL >= EXP_LEVELS.Count)
        {
            EXP_HAS_FINISHED = true;
        }
    }

    public static bool HASFINISHED()
    {
        return EXP_HAS_FINISHED;
    }
    
}
