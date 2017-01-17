using UnityEngine;
using System.Collections;
using Pathfinding;

public class AIEnemy : MonoBehaviour {

    public float patrolSpeed = 2f;                          // The nav mesh agent's speed when patrolling.
    public float chaseSpeed = 5f;                           // The nav mesh agent's speed when chasing.
    public float chaseWaitTime = 5f;                        // The amount of time to wait when the last sighting is reached.
    public float patrolWaitTime = 1f;                       // The amount of time to wait when the patrol way point is reached.
    public Transform[] patrolWayPoints;                     // An array of transforms for the patrol route.


    private AILineOfSight enemySight;                          // Reference to the EnemySight script.
    private NavMeshAgent nav;                                 // Reference to the nav mesh agent.
    private Transform player;
    private Animator anim;
    
    // Reference to the player's transform.
    //private PlayerHealth playerHealth;                      // Reference to the PlayerHealth script.
    //private LastPlayerSighting lastPlayerSighting;          // Reference to the last global sighting of the player.
    private float chaseTimer;                               // A timer for the chaseWaitTime.
    private float patrolTimer;                              // A timer for the patrolWaitTime.
    private int wayPointIndex;                              // A counter for the way point array.


    private bool stopped = false;
    private bool chasing = false;

    private Seeker seeker;
    private Transform currPos;

    void Awake()
    {
        this.nav = GetComponent<NavMeshAgent>();
        this.player = GameObject.FindGameObjectWithTag("Player").transform;
        this.enemySight = GetComponent<AILineOfSight>();
        this.anim = GetComponent<Animator>();
        this.seeker = GetComponent<Seeker>();
        this.currPos = GetComponent<Transform>();
    }

    void Update()
    {
        // If the player is in sight and is alive...
        if (enemySight.playerInSight)
            Chasing();

        // If the player has been sighted and isn't dead...
        else if (enemySight.playerLastSighting != enemySight.resetPosition)
            // ... chase.
            Chasing();

        // Otherwise...
        else
            // ... patrol.
            Patrolling();
    }

    void Chasing()
    {
        // Create a vector from the enemy to the last sighting of the player.
        Vector3 sightingDeltaPos = enemySight.playerLastSighting - transform.position;

        // If the the last personal sighting of the player is not close...
        if (sightingDeltaPos.sqrMagnitude > 4f)
        {
            // ... set the destination for the NavMeshAgent to the last personal sighting of the player.
            nav.destination = enemySight.playerLastSighting;
            if (stopped && !chasing)
            {
                anim.Play("IdleToRun");
            } else if(!stopped && !chasing)
            {
                anim.Play("WalkToRun");
            }
            chasing = true;
        }
            

        // Set the appropriate speed for the NavMeshAgent.
        nav.speed = chaseSpeed;

        // If near the last personal sighting...
        if (nav.remainingDistance < nav.stoppingDistance)
        {

            if (chasing)
            {
                anim.Play("RunToIdle");
                chasing = false;
            }

            // ... increment the timer.
            chaseTimer += Time.deltaTime;

            // If the timer exceeds the wait time...
            if (chaseTimer >= chaseWaitTime)
            {
                // ... reset last global sighting, the last personal sighting and the timer.
                enemySight.playerLastSighting = enemySight.resetPosition;
                chaseTimer = 0f;
            }
        }
        else
        {
            // If not near the last sighting personal sighting of the player, reset the timer.
            chaseTimer = 0f;
            
        }
            
    }

    void Patrolling()
    {
        // Set an appropriate speed for the NavMeshAgent.
        nav.speed = patrolSpeed;

        // If near the next waypoint or there is no destination...
        if (nav.destination == enemySight.resetPosition || nav.remainingDistance < nav.stoppingDistance)
        {
            // ... increment the timer.
            patrolTimer += Time.deltaTime;
            
            // If the timer exceeds the wait time...
            if (patrolTimer >= patrolWaitTime)
            {
                stopped = false;
                anim.Play("Walk");
                // ... increment the wayPointIndex.
                if (wayPointIndex == patrolWayPoints.Length - 1)
                {
                    wayPointIndex = 0;
                }
                else
                {
                    wayPointIndex++;
                }
                // Reset the timer.
                patrolTimer = 0;
            }
            else
            {
                if (!stopped)
                {
                    anim.Play("WalkToIdle");
                    stopped = true;
                }
                    
            }
        }
        else
        {
            // If not near a destination, reset the timer.
            patrolTimer = 0;
        }
            

        // Make Sure this doesn't crash! 
        if(patrolWayPoints.Length > 0)
        {
            if (!anim.GetBool("Walking") && patrolTimer == 0)
            {
                anim.Play("Walk");
                stopped = false;
            }
               

            // Set the destination to the patrolWayPoint.
            //nav.SetDestination(patrolWayPoints[wayPointIndex].position);
            Path p = seeker.StartPath(currPos.position, patrolWayPoints[wayPointIndex].position);
            
            //nav.destination = patrolWayPoints[wayPointIndex].position;
        }
    }

    public void setWaypoints(Transform[] patrolWayPoints)
    {
        this.patrolWayPoints = patrolWayPoints;
    }



}
