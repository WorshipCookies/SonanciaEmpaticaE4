using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour {

    private Vector3 spawnPosition;

	// Use this for initialization
	void Awake () {
        spawnPosition = transform.position;
	}
	
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            respawnPlayerStart();
        }
    }

    // this function will probably be bigger -- we need to add a reset etc. maybe a GUI 
	public void respawnPlayerStart()
    {
        transform.position = spawnPosition;
    }
}
