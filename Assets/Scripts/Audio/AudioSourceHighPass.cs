using UnityEngine;
using System.Collections;

public class AudioSourceHighPass : AudioSourceInfo {

    private float cutoff = 500f;
    private float qvalue = 1f;

    // Parameter Interval Limits -- Index 0 = Min; Index 1 = Max.
    private float[] lim_cutoff = { 10f, 7000f };
    private float[] lim_qvalue = { 1f, 10f };

    private int param_num = 2;

    public AudioSourceHighPass(string effectName, AudioSource source, int roomID)
        : base(effectName, source, roomID)
    {
        source.outputAudioMixerGroup.audioMixer.SetFloat("highPassCutOff", cutoff);
        source.outputAudioMixerGroup.audioMixer.SetFloat("highPassQ", qvalue);
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
        source.outputAudioMixerGroup.audioMixer.SetFloat("highPassCutOff", cutoff);
    }

    public void setQValue(float qvalue)
    {
        this.qvalue = qvalue;
        source.outputAudioMixerGroup.audioMixer.SetFloat("highPassQ", qvalue);
    }

    public override int getParamNum()
    {
        return param_num;
    }

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
        Debug.Log(" ------ High Pass Parameters ------ ");
        Debug.Log(" CutOff = " + cutoff);
        Debug.Log(" QVlaue = " + qvalue);
    }
}
