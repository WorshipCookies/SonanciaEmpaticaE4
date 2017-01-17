using UnityEngine;
using System.Collections;

public class AILineOfSight : MonoBehaviour {

    public Vector3 resetPosition = new Vector3(1000f, 1000f, 1000f); // Reset the position to default if the monster loses track of him.

    public float fieldOfViewAngle = 110f;

    public AudioClip patrolSound;
    public AudioClip roarSound;
    public AudioClip stepSound;
    private AudioSource monsterSound;
    private bool soundHasBeenPlayed = false;
    
    public float closefieldOfViewAngle = 360f; // So players do not run past him and the agent stands as if he disappeared.
    public float defaultfieldOfViewAngle = 110f;
    public float highAwarenessDistance = 15f; // Maybe add a time as well??

    public bool playerInSight;
    public Vector3 playerLastSighting;

    private NavMeshAgent nav;
    private SphereCollider col;
    private Animator anim;
    private GameObject player;
    private Vector3 previousSighting;

    private MonsterLogging log;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        col = GetComponent<SphereCollider>();
        anim = GetComponent<Animator>();
        monsterSound = GetComponentInChildren<AudioSource>();
        log = GetComponent<MonsterLogging>();
        player = GameObject.FindGameObjectWithTag("Player");

        // Last Sighting Reset Here -- Once Implemented
        playerLastSighting = resetPosition;
        previousSighting = resetPosition;

    }
	
	// Update is called once per frame
	void Update ()
    {
        anim.SetBool("PlayerInSight", playerInSight); // Update the animation boolean.

        if(!LevelBuilder.isGamePaused)
        {
            if (playerInSight)
            {
                playRoar();
            }
            else
            {
                StartCoroutine(playPatrolSound(UnityEngine.Random.Range(30,40)));
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.name == "FPSPlayer")
        {
            determineFieldofView(other);

            // Do Stuff Here
            playerInSight = false;

            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            if(angle < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                
                Debug.DrawRay(transform.position, direction.normalized, Color.cyan);

                if (Physics.Raycast(transform.position, direction.normalized, out hit, col.radius))
                {
                    if(hit.transform.tag == "Player")
                    {

                        playerInSight = true;

                        // Update Global Sight of the Player HERE
                        playerLastSighting = other.transform.position;

                       // Debug.Log("I SEE YOU!");
                    }
                }

            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.name == "FPSPlayer")
        {
            playerInSight = false;
        }
    }

    float CalculatePathLength(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();

        if (nav.enabled)
        {
            nav.CalculatePath(targetPosition, path);
        }

        Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];
        allWayPoints[0] = transform.position;
        allWayPoints[allWayPoints.Length - 1] = targetPosition;

        for(int i = 0; i < path.corners.Length; i++)
        {
            allWayPoints[i + 1] = path.corners[i];
        }

        float pathLength = 0f;

        for(int i = 0; i < allWayPoints.Length-1; i++)
        {
            pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]); 
        }
        return pathLength;
    }

    private void determineFieldofView(Collider other)
    {
        // Added new Code
        if (playerInSight == true)
        {
            float dist = Mathf.Abs(Vector3.Distance(other.transform.position, transform.position));
            if (dist <= highAwarenessDistance && fieldOfViewAngle == defaultfieldOfViewAngle)
            {
                fieldOfViewAngle = closefieldOfViewAngle;
            }
            else
            {
                fieldOfViewAngle = defaultfieldOfViewAngle;
            }
        }
    }

    private void playRoar()
    {
        if (!monsterSound.isPlaying && playerInSight && !soundHasBeenPlayed)
        {
            monsterSound.PlayOneShot(roarSound);
            soundHasBeenPlayed = true;
        }
        else if(!playerInSight && soundHasBeenPlayed)
        {
            soundHasBeenPlayed = false;
        }
    }

    IEnumerator playPatrolSound(float time)
    {
        yield return new WaitForSeconds(time);

        if (!playerInSight && !monsterSound.isPlaying)
        {
            log.logPatrolSound();
            monsterSound.PlayOneShot(patrolSound);
        }
    }

    public void playFootstep()
    {
        if (!monsterSound.isPlaying)
        {
            monsterSound.PlayOneShot(stepSound);
        }
    }
}
