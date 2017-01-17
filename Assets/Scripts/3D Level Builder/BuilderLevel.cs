using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class BuilderLevel : MonoBehaviour {

    public GameObject Player;

    // Use this for initialization
    void Start()
    {
        new MeshBuilder(100, 50, 1);

        //Player = (GameObject)Instantiate(Player, new Vector3(2, 0, 2), Quaternion.identity);
    }

}
