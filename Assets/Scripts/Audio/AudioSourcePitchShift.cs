using UnityEngine;
using System.Collections;

public class AudioSourcePitchShift : AudioSourceInfo {

    private float pitch = 1f;
    private float[] lim_pitch = { 0.5f, 2.0f };

    private int param_num = 1;

    public AudioSourcePitchShift(string effectName, AudioSource source, int roomID)
        : base(effectName, source, roomID)
    {
        source.outputAudioMixerGroup.audioMixer.SetFloat("pitchPitch", pitch);
    }

    public float getPitchValue()
    {
        return pitch;
    }

    public void setPitchValue(float pitch)
    {
        this.pitch = pitch;
        source.outputAudioMixerGroup.audioMixer.SetFloat("pitchPitch", pitch);
    }

    public override int getParamNum()
    {
        return param_num;
    }

    public override void randomParams()
    {
        setPitchValue(myRandom.Next(lim_pitch[0], lim_pitch[1]));
    }

    public override void setPresetParam(int i)
    {
        throw new System.NotImplementedException();
    }

    public override int maxPresetNum()
    {
        throw new System.NotImplementedException();
    }

    public override int currentPreset()
    {
        throw new System.NotImplementedException();
    }

    public override void printParameters()
    {
        Debug.Log(" ------ Pitch Shift Parameters ------ ");
        Debug.Log(" Pitch = " + pitch);
    }

    
}
