using UnityEngine;
using System.Collections;

public class SoundRank {

    private static Rank soundranks = null;

	public static Rank getSoundRanks()
    {
        if (soundranks == null)
        {
            Debug.Log("Error: SoundRank has not been initialized.");
        } 
        return soundranks;
        
    }

    public static Rank initializeRanks(Rank.RankType r)
    {
        soundranks = new Rank(r);
        return soundranks;
    }

}
