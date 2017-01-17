using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class reverbScript : MonoBehaviour {

    public AudioSource source;
    public AudioMixer mixer;

    private float dry = 0.0f;
    private float delay = 0.04f;
    private float reflections = -10000.00f;
    private float room = -10000.00f;

    // Use this for initialization
    void Start()
    {
        source.loop = true;
        source.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
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
            // Dryness Parameter
            if (Input.GetKey(KeyCode.UpArrow))
            {
                dry += 1;
                mixer.SetFloat("reverbDry", dry);
                Debug.Log("Reverb Dry Parameter Changed " + dry);
            }

            else if (Input.GetKey(KeyCode.DownArrow))
            {
                dry -= 1;
                mixer.SetFloat("reverbDry", dry);
                Debug.Log("Reverb Dry Parameter Changed " + dry);
            }

            // Delay Parameter
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                delay -= 1;
                mixer.SetFloat("reverbDelay", delay);
                Debug.Log("Reverb Delay Parameter Changed " + delay);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                delay += 1;
                mixer.SetFloat("reverbDelay", delay);
                Debug.Log("Reverb Delay Parameter Changed " + delay);
            }

            // Room Parameter
            else if (Input.GetKey(KeyCode.A))
            {
                room += 100;
                mixer.SetFloat("reverbRoom", room);
                Debug.Log("Reverb Room Parameter Changed " + room);
            }
            else if (Input.GetKey(KeyCode.Z))
            {
                room -= 100;
                mixer.SetFloat("reverbRoom", room);
                Debug.Log("Reverb Room Parameter Changed " + room);
            }

            // Reflections Paramter
            else if (Input.GetKey(KeyCode.S))
            {
                reflections += 100;
                mixer.SetFloat("reverbReflections", reflections);
                Debug.Log("Reverb Reflections Parameter Changed " + reflections);
            }
            else if (Input.GetKey(KeyCode.X))
            {
                reflections -= 100;
                mixer.SetFloat("reverbReflections", reflections);
                Debug.Log("Reverb Reflections Parameter Changed " + reflections);
            }
        }
    }
}
