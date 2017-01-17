using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour {

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == LevelBuilder.playerID)
        {
            Debug.Log("Current Tile is " + name);
        }
    }
}
