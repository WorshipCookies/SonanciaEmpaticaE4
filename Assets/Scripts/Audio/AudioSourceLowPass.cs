using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AudioSourceLowPass : AudioSourceInfo {

    private float cutoff = 500f;
    private float qvalue = 1f;

    // Parameter Interval Limits -- Index 0 = Min; Index 1 = Max.
    private float[] lim_cutoff = { 10f, 7000f };
    private float[] lim_qvalue = { 1f, 10f };

    private int param_num = 2;

    public AudioSourceLowPass(string effectName, AudioSource source, int roomID)
        : base(effectName, source, roomID)
    {
        source.outputAudioMixerGroup.audioMixer.SetFloat("lowPassCutOff", cutoff);
        source.outputAudioMixerGroup.audioMixer.SetFloat("lowPassQ", qvalue);
    }

    public float getCutOff()
    {
        return cutoff;
    }

    public float getQValue()
    {
        return qvalue;
    }

    public void setCutOff(float cutoff)
    {
        this.cutoff = cutoff;
        source.outputAudioMixerGroup.audioMixer.SetFloat("lowPassCutOff", cutoff);
    }

    public void setQValue(float qvalue)
    {
        this.qvalue = qvalue;
        source.outputAudioMixerGroup.audioMixer.SetFloat("lowPassQ", qvalue);
    }

    public override int getParamNum()
    {
        return param_num;
    }

    // This is a test method in order to experiment the possibility of changing the parameters of the mixer during runtime.
    public override void randomParams()
    {
        // Randomly change all parameters in this audio source
        setCutOff(myRandom.Next(lim_cutoff[0], lim_cutoff[1]));
        setQValue(myRandom.Next(lim_qvalue[0], lim_qvalue[1]));
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
        Debug.Log(" ------ Low Pass Parameters ------ ");
        Debug.Log(" CutOff = " + cutoff);
        Debug.Log(" QVlaue = " + qvalue);
    }
}
