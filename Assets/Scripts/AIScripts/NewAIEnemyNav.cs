using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Seeker))]

public class NewAIEnemyNav : AIPath
{
    /** Animation component.
     * Should hold animations "awake" and "forward"
     */
    public Animation anim;

    public float patrolSpeed = 2f;                          // The nav mesh agent's speed when patrolling.
    public float chaseSpeed = 5f;                           // The nav mesh agent's speed when chasing.
    public float chaseWaitTime = 5f;                        // The amount of time to wait when the last sighting is reached.
    public float patrolWaitTime = 1f;                       // The amount of time to wait when the patrol way point is reached.
    public Transform[] patrolWayPoints;                     // An array of transforms for the patrol route.


    /** Effect which will be instantiated when end of path is reached.
		 * \see OnTargetReached */
    public GameObject endOfPathEffect;

    // Use this for initialization
    public new void Start()
    {
        base.Start();
    }

    /** Point for the last spawn of #endOfPathEffect */
    protected Vector3 lastTarget;

    public override void OnTargetReached()
    {
        if (endOfPathEffect != null && Vector3.Distance(tr.position, lastTarget) > 1)
        {
            GameObject.Instantiate(endOfPathEffect, tr.position, tr.rotation);
            lastTarget = tr.position;
        }
    }

    public override Vector3 GetFeetPosition()
    {
        return tr.position;
    }

    protected new void Update()
    {
        //Get velocity in world-space
        Vector3 velocity;

        if (canMove)
        {
            //Calculate desired velocity
            Vector3 dir = CalculateVelocity(GetFeetPosition());

            //Rotate towards targetDirection (filled in by CalculateVelocity)
            RotateTowards(targetDirection);

            dir.y = 0;
            if (dir.sqrMagnitude > patrolSpeed * patrolSpeed)
            {
                //If the velocity is large enough, move
            }
            else
            {
                //Otherwise, just stand still (this ensures gravity is applied)
                dir = Vector3.zero;
            }

            if (rvoController != null)
            {
                rvoController.Move(dir);
                velocity = rvoController.velocity;
            }
            else
            if (controller != null)
            {
                controller.SimpleMove(dir);
                velocity = controller.velocity;
            }
            else
            {
                Debug.LogWarning("No NavmeshController or CharacterController attached to GameObject");
                velocity = Vector3.zero;
            }
        }
        else
        {
            velocity = Vector3.zero;
        }
    }

    public void setPatrols(Transform[] patrolWayPoints)
    {
        this.patrolWayPoints = patrolWayPoints;
    }

}
