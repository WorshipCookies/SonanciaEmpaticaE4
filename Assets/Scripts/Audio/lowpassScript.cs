using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class lowpassScript : MonoBehaviour {

    public AudioSource source;
    public AudioMixer mixer;

    private float cutoff = 500f;
    private float qvalue = 1f;

	// Use this for initialization
	void Start () {
        source.Stop();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (source.isPlaying)
            {
                source.Stop();
                Debug.Log("Reverb Play Has Stopped!");
            }
            else
            {
                source.Play();
                Debug.Log("Reverb Play Has Started!");
            }

        }
        if (source.isPlaying)
        {
            // Cut Off Parameter
            if (Input.GetKey(KeyCode.Keypad8))
            {
                cutoff += 1;
                mixer.SetFloat("lowPassCutOff", cutoff);
                Debug.Log("Cut off value changed " + cutoff);
            }
            else if (Input.GetKey(KeyCode.Keypad5))
            {
                cutoff -= 1;
                mixer.SetFloat("lowPassCutOff", cutoff);
                Debug.Log("Cut off value changed " + cutoff);
            }

            // Q Value Parameter
            else if (Input.GetKey(KeyCode.Keypad7))
            {
                qvalue += 1;
                mixer.SetFloat("lowPassQ", qvalue);
                Debug.Log("Q Value changed " + qvalue);
            }
            else if (Input.GetKey(KeyCode.Keypad4))
            {
                qvalue -= 1;
                mixer.SetFloat("lowPassQ", qvalue);
                Debug.Log("Q Value changed " + qvalue);
            }
        }
	}
}
