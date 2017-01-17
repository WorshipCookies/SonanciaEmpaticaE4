using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public abstract class AudioSourceInfo {

    public AudioSource source;
    public string effectName;
    public int roomID;

    public AudioSourceInfo(string effectName, AudioSource source, int roomID)
    {
        this.effectName = effectName;
        this.source = source;
        this.roomID = roomID;
    }

    public string getEffectName()
    {
        return effectName;
    }

    public AudioSource getSource()
    {
        return source;
    }

    public int getRoomId()
    {
        return roomID;
    }

    public abstract int getParamNum();

    public abstract void randomParams();

    public abstract void setPresetParam(int i);

    public abstract int maxPresetNum();

    public abstract int currentPreset();

    public abstract void printParameters();
}
