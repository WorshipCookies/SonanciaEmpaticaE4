using UnityEngine;
using System.Collections;

public class ObjectiveScript : MonoBehaviour
{
    
    private GameObject player;
    private PlayerUI playerUI;
    
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerUI = player.GetComponent<PlayerUI>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "FPSPlayer")
        {
            playerUI.setPlayerObjective(true);
        }
    }


    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.name == "FPSPlayer")
        {
            playerUI.setPlayerObjective(false);
        }
    }
}