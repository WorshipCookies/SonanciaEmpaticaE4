using UnityEngine;
using System.Collections;

public class AIEnemyAttack : MonoBehaviour {
    
    private GameObject player;
    private AILineOfSight playerSight;

    public float attackRange;
    public bool inRange;

    public bool isAttacking;


    // Use this for initialization
    public void Awake()
    {
        playerSight = GetComponent<AILineOfSight>();
        player = GameObject.FindGameObjectWithTag("Player");

        inRange = false;
        isAttacking = false;
    }

    
	// Update is called once per frame
	public void Update ()
    {
        inRange = calcInRange();
	}


    private bool calcInRange()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        return dist < attackRange ? true : false; // If in Range return true, else return false
    }

}
