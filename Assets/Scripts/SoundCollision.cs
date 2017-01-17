using UnityEngine;
using System.Collections;

public class SoundCollision : MonoBehaviour {

    private bool notPlayed = true;
    private SoundLogging soundLog;

    void Start()
    {
        soundLog = gameObject.AddComponent<SoundLogging>();
    }

    void OnTriggerEnter(Collider col)
    {
        // Debug.Log("Trigger!!!!!!!! + " + col.gameObject.name); Phil -- Commented it out!
        if (col.gameObject.name == "FPSPlayer" && notPlayed)
        {
            AudioSource aS = GetComponent<AudioSource>();
            aS.PlayOneShot(aS.clip);
            soundLog.logSoundCollision();
            notPlayed = false;
        }
    }
}
