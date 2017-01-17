using UnityEngine;
using System.Collections;

public class SoundTypeConfig {

    private SoundBank clipbank;
    private AudioSourceBank sourcebank;

    public SoundTypeConfig(LevelObject lvl, int rtype, int playType, int aType, int pickType)
    {
        
    }

    public SoundTypeConfig(LevelObject lvl, Rank.RankType rtype, SoundPlayType.PlayType playType, 
        SoundAllocationType.Allocation aType, SoundPickingType.PickingType pickType)
    {
        clipbank = new SoundBank();
        sourcebank = new AudioSourceBank();


    }


}
