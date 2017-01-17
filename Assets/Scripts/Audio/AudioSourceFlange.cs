using UnityEngine;
using System.Collections;

public class AudioSourceFlange : AudioSourceInfo {

    private float dryMix = 0.45f;
    private float wetMix = 0.55f;
    private float depth = 1f;
    private float rate = 0.1f;

    private float[] lim_dryMix = { 0f, 1f };
    private float[] lim_wetMix = { 0f, 1f };
    private float[] lim_depth = { 0f, 1f };
    private float[] lim_rate = { 0f, 20f };

    private int param_num = 4;

    public AudioSourceFlange(string effectName, AudioSource source, int roomID)
        : base(effectName, source, roomID)
    {
        source.outputAudioMixerGroup.audioMixer.SetFloat("flangeDryMix", dryMix);
        source.outputAudioMixerGroup.audioMixer.SetFloat("flangeWetMix", wetMix);
        source.outputAudioMixerGroup.audioMixer.SetFloat("flangeDepth", depth);
        source.outputAudioMixerGroup.audioMixer.SetFloat("flangeRate", rate);
    }

    public float getDryMix()
    {
        return dryMix;
    }

    public float getWetMix()
    {
        return wetMix;
    }

    public float getDepth()
    {
        return depth;
    }

    public float getRate()
    {
        return rate;
    }

    public void setDryMix(float dryMix)
    {
        this.dryMix = dryMix;
        source.outputAudioMixerGroup.audioMixer.SetFloat("flangeDryMix", dryMix);
    }

    public void setWetMix(float wetMix)
    {
        this.wetMix = wetMix;
        source.outputAudioMixerGroup.audioMixer.SetFloat("flangeWetMix", wetMix);
    }

    public void setDepth(float depth)
    {
        this.depth = depth;
        source.outputAudioMixerGroup.audioMixer.SetFloat("flangeDepth", depth);
    }

    public void setRate(float rate)
    {
        this.rate = rate;
        source.outputAudioMixerGroup.audioMixer.SetFloat("flangeRate", rate);
    }

    public override int getParamNum()
    {
        return param_num;
    }

    public override void randomParams()
    {
        setDryMix(myRandom.Next(lim_dryMix[0], lim_dryMix[1]));
        setWetMix(myRandom.Next(lim_wetMix[0], lim_wetMix[1]));
        setDepth(myRandom.Next(lim_depth[0], lim_depth[1]));
        setRate(myRandom.Next(lim_rate[0], lim_rate[1]));
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
        Debug.Log(" ------ Flange Parameters ------ ");
        Debug.Log(" DryMix = " + dryMix);
        Debug.Log(" WetMix = " + wetMix);
        Debug.Log(" Depth = " + depth);
        Debug.Log(" Rate = " + rate); 
    }
}
