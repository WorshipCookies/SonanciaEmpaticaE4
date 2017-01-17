using UnityEngine;
using System.Collections;
using System;

public class AudioSourceNoEffect : AudioSourceInfo {

    private float volume = 0f;
    private float MAX_VOL = 20f;
    private float MIN_VOL = -20;
    private int param_num = 1;

    public AudioSourceNoEffect(string effectName, AudioSource source, int roomID)
        : base(effectName, source, roomID)
    {
        this.volume = 0f;
    }

    public override int getParamNum()
    {
        return param_num;
    }

    public void setVol(float volume)
    {
        this.volume = volume;
        source.outputAudioMixerGroup.audioMixer.SetFloat("noEffectVol", volume);
    }

    public override void printParameters()
    {
        Debug.Log(" ------ No Effect Parameters ------ ");
        Debug.Log(" Volume = " + volume);
    }

    public override void randomParams()
    {
        setVol(myRandom.Next(MIN_VOL, MAX_VOL));
    }

    public override void setPresetParam(int i)
    {
        throw new NotImplementedException();
    }

    public override int maxPresetNum()
    {
        throw new NotImplementedException();
    }

    public override int currentPreset()
    {
        throw new NotImplementedException();
    }
}
