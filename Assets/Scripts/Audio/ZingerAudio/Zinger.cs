using UnityEngine;
using System.Collections;

public class Zinger {

    private int id;
    private Transform zTransform;
    private AudioSource zSource;
    private int roomID;

    private static int idCount = 0;

    public static Zinger createZinger(int roomID, Transform zinger)
    {
        AudioSource zSource = zinger.GetComponent<AudioSource>();
        if(zSource != null)
        {
            return new Zinger(idCount++,roomID,zinger,zSource);
        } else
        {
            return null;
        }
    }

    private Zinger(int id, int roomID, Transform zTransform, AudioSource zSource)
    {
        this.id = id;
        this.roomID = roomID;
        this.zTransform = zTransform;
        this.zSource = zSource;
    }

    public int getID()
    {
        return id;
    }

    public int getRoomID()
    {
        return roomID;
    }

    public AudioSource getAudioSource()
    {
        return zSource;
    }

    public Transform getTransform()
    {
        return zTransform;
    }

    public void attachAudio(AudioClip clip)
    {
        zSource.clip = clip;
    }
}
