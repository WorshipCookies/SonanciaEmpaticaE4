using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AudioSourceReverb : AudioSourceInfo {

    private float dry = 0.0f;
    private float delay = 0.04f;
    private float reflections = -10000.00f;
    private float room = -10000.00f;

    // Parameter Interval Limits -- Index 0 = Min; Index 1 = Max.
    private float[] lim_dry = { -10000f, 0f };
    private float[] lim_room = { -10000f, 0f };
    private float[] lim_reflections = { -10000f, 1000f };
    private float[] lim_delay = { 0f, 0.1f };


    public int param_num = 4;

    public AudioSourceReverb(string effectName, AudioSource source, int roomID)
        : base(effectName, source, roomID)
    {
        source.outputAudioMixerGroup.audioMixer.SetFloat("reverbDry", dry);
        source.outputAudioMixerGroup.audioMixer.SetFloat("reverbDelay", delay);
        source.outputAudioMixerGroup.audioMixer.SetFloat("reverbRoom", room);
        source.outputAudioMixerGroup.audioMixer.SetFloat("reverbReflections", reflections);
    }


    public float getDryValue()
    {
        return dry;
    }

    public float getDelayValue()
    {
        return delay;
    }

    public float getReflections()
    {
        return reflections;
    }

    public float getRoomValue()
    {
        return room;
    }

    public void setDry(float dry)
    {
        this.dry = dry;
        source.outputAudioMixerGroup.audioMixer.SetFloat("reverbDry", dry);
    }

    public void setDelay(float delay)
    {
        this.delay = delay;
        source.outputAudioMixerGroup.audioMixer.SetFloat("reverbDelay", delay);
    }

    public void setReflections(float reflections)
    {
        this.reflections = reflections;
        source.outputAudioMixerGroup.audioMixer.SetFloat("reverbRoom", room);
    }

    public void setRoomValue(float room)
    {
        this.room = room;
        source.outputAudioMixerGroup.audioMixer.SetFloat("reverbReflections", reflections);
    }

    public override int getParamNum()
    {
        return param_num;
    }

    // This is a test method in order to experiment the possibility of changing the parameters of the mixer during runtime.
    public override void randomParams()
    {
        // Randomly change all parameters in this audio source
        setDry(myRandom.Next(lim_dry[0], lim_dry[1]));
        setRoomValue(myRandom.Next(lim_room[0], lim_room[1]));
        setReflections(myRandom.Next(lim_reflections[0], lim_reflections[1]));
        setDelay(myRandom.Next(lim_delay[0], lim_delay[1]));
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
        Debug.Log(" ------ Reverb Parameters ------ ");
        Debug.Log(" Dry Value = " + dry);
        Debug.Log(" Delay Value = " + delay);
        Debug.Log(" Reflections Value = " + reflections);
        Debug.Log(" Room Value = " + room);
    }
}
