using UnityEngine;
using System.Collections;

public class AudioSourceEcho : AudioSourceInfo  {

    private float echoDelay = 500f;
    private float echoDecay = 50f;
    private float dryMix = 100f;
    private float wetMix = 100f;

    // Parameter Interval Limits -- Index 0 = Min; Index 1 = Max.
    private float[] lim_echoDelay = { 1f, 500f };
    private float[] lim_echoDecay = { 0f, 0.5f };
    private float[] lim_dryMix = { 0f, 1f };
    private float[] lim_wetMix = { 0f, 1f };

    private int param_num = 4;

    public AudioSourceEcho(string effectName, AudioSource source, int roomID)
        : base(effectName, source, roomID)
    {
        source.outputAudioMixerGroup.audioMixer.SetFloat("echoDelay", echoDelay);
        source.outputAudioMixerGroup.audioMixer.SetFloat("echoDecay", echoDecay);
        source.outputAudioMixerGroup.audioMixer.SetFloat("echoWetMix", wetMix);
        source.outputAudioMixerGroup.audioMixer.SetFloat("echoDryMix", dryMix);
    }

    public float getDelay()
    {
        return echoDelay;
    }

    public float getDecay()
    {
        return echoDecay;
    }

    public float getDryMix()
    {
        return dryMix;
    }

    public float getWetMix()
    {
        return wetMix;
    }

    public override int getParamNum()
    {
        return param_num;
    }

    public void setDelay(float delay)
    {
        this.echoDelay = delay;
        source.outputAudioMixerGroup.audioMixer.SetFloat("echoDelay", echoDelay);
    }

    public void setDecay(float decay)
    {
        this.echoDecay = decay;
        source.outputAudioMixerGroup.audioMixer.SetFloat("echoDecay", echoDecay);
    }

    public void setWetMix(float wetMix)
    {
        this.wetMix = wetMix;
        source.outputAudioMixerGroup.audioMixer.SetFloat("echoWetMix", wetMix);
    }

    public void setDryMix(float dryMix)
    {
        this.dryMix = dryMix;
        source.outputAudioMixerGroup.audioMixer.SetFloat("echoDryMix", dryMix);
    }

    // This is a test method in order to experiment the possibility of changing the parameters of the mixer during runtime.
    public override void randomParams()
    {
        // Randomly change all parameters in this audio source
        setDelay(myRandom.Next(lim_echoDelay[0], lim_echoDelay[1]));
        setDecay(myRandom.Next(lim_echoDecay[0], lim_echoDecay[1]));
        setDryMix(myRandom.Next(lim_dryMix[0], lim_dryMix[1]));
        setWetMix(myRandom.Next(lim_wetMix[0], lim_wetMix[1]));
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
        Debug.Log(" ------ Echo Parameters ------ ");
        Debug.Log(" Delay Value = " + echoDelay);
        Debug.Log(" Decay Value = " + echoDecay);
        Debug.Log(" Dry Mix Value = " + dryMix);
        Debug.Log(" Wet Mix Value = " + wetMix);
    }
}
