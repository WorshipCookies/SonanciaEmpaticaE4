using UnityEngine;
using System.Collections;

public class AudioSourceDistortion : AudioSourceInfo {

    private float distortionLevel = 0.5f;

    // Parameter Interval Limits -- Index 0 = Min; Index 1 = Max.
    private float[] lim_distortionLevel = { 0f, 1f };

    private int param_num = 1;

    public AudioSourceDistortion(string effectName, AudioSource source, int roomID)
        : base(effectName, source, roomID)
    {
        source.outputAudioMixerGroup.audioMixer.SetFloat("distortionLevel", distortionLevel);
    }

    public float getDistortionLevel()
    {
        return distortionLevel;
    }

    public override int getParamNum()
    {
        return param_num;
    }

    public void setDistortionLevel(float distortionLevel)
    {
        this.distortionLevel = distortionLevel;
        source.outputAudioMixerGroup.audioMixer.SetFloat("distortionLevel", distortionLevel);
    }

    // This is a test method in order to experiment the possibility of changing the parameters of the mixer during runtime.
    public override void randomParams()
    {
        // Randomly change all parameters in this audio source
        setDistortionLevel(myRandom.Next(lim_distortionLevel[0], lim_distortionLevel[1]));
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
        Debug.Log(" ------ Distortion Parameters ------ ");
        Debug.Log(" Distortion Level Value = " + distortionLevel);
    }
}
