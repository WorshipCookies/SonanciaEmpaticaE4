using UnityEngine;
using System.Collections;

public class AudioSourceChorus : AudioSourceInfo {

    private float dryMix = 0.5f;
    private float delay = 0.4f;
    private float rate = 7f;
    private float depth = 0.4f;
    private float feedback = 1f;

    private float[] lim_dryMix = { 0f, 1f };
    private float[] lim_delay = { 0f, 100f };
    private float[] lim_rate = { 0f, 20f };
    private float[] lim_depth = { 0f, 1f };
    private float[] lim_feedback = { -1f, 1f };

    private int param_num = 5;

    public AudioSourceChorus(string effectName, AudioSource source, int roomID)
        : base(effectName, source, roomID)
    {
        source.outputAudioMixerGroup.audioMixer.SetFloat("chorusDryMix", dryMix);
        source.outputAudioMixerGroup.audioMixer.SetFloat("chorusDelay", delay);
        source.outputAudioMixerGroup.audioMixer.SetFloat("chorusRate", rate);
        source.outputAudioMixerGroup.audioMixer.SetFloat("chorusDepth", depth);
        source.outputAudioMixerGroup.audioMixer.SetFloat("chorusFeedback", feedback);
    }

    public float getDryMix()
    {
        return dryMix;
    }

    public float getDelay()
    {
        return delay;
    }

    public float getRate()
    {
        return rate;
    }

    public float getDepth()
    {
        return depth;
    }

    public float getFeedback()
    {
        return feedback;
    }

    public void setDryMix(float dryMix)
    {
        this.dryMix = dryMix;
        source.outputAudioMixerGroup.audioMixer.SetFloat("chorusDryMix", dryMix);
    }

    public void setDelay(float delay)
    {
        this.delay = delay;
        source.outputAudioMixerGroup.audioMixer.SetFloat("chorusDelay", delay);
    }

    public void setRate(float rate)
    {
        this.rate = rate;
        source.outputAudioMixerGroup.audioMixer.SetFloat("chorusRate", rate);
    }

    public void setDepth(float depth)
    {
        this.depth = depth;
        source.outputAudioMixerGroup.audioMixer.SetFloat("chorusDepth", depth);
    }

    public void setFeedback(float feedback)
    {
        this.feedback = feedback;
        source.outputAudioMixerGroup.audioMixer.SetFloat("chorusFeedback", feedback);
    }

    public override int getParamNum()
    {
        return param_num;
    }

    public override void randomParams()
    {
        setDryMix(myRandom.Next(lim_dryMix[0], lim_dryMix[1]));
        setDelay(myRandom.Next(lim_delay[0], lim_delay[1]));
        setRate(myRandom.Next(lim_rate[0], lim_rate[1]));
        setDepth(myRandom.Next(lim_depth[0], lim_depth[1]));
        setFeedback(myRandom.Next(lim_feedback[0], lim_feedback[1]));
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
        Debug.Log(" ------ Chorus Parameters ------ ");
        Debug.Log(" DryMix = " + dryMix);
        Debug.Log(" Delay = " + delay);
        Debug.Log(" Rate = " + rate);
        Debug.Log(" Depth = " + depth);
        Debug.Log(" Feedback = " + feedback);
    }
}
