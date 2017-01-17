using UnityEngine;
using System.Collections;

public class AIPathScript : AIPath {

    private Transform tr;

    /** Minimum velocity for moving */
    public float sleepVelocity = 0.4F;

    /** Speed relative to velocity with which to play animations */
    public float animationSpeed = 0.2F;

    public new void Start()
    {
        tr = GetComponent<Transform>();
        base.Start();
    }

    /** Point for the last spawn of #endOfPathEffect */
    protected Vector3 lastTarget;

    /** Effect which will be instantiated when end of path is reached. */
    private int currPatrol;
    private Transform[] positions;


    /**
     * Called when the end of path has been reached.
     * An effect (#endOfPathEffect) is spawned when this function is called
     * However, since paths are recalculated quite often, we only spawn the effect
     * when the current position is some distance away from the previous spawn-point
     */
    public override void OnTargetReached()
    {
        if (positions[currPatrol].gameObject != null && Vector3.Distance(tr.position, lastTarget) > 1)
        {
            GameObject.Instantiate(positions[currPatrol++].gameObject, tr.position, tr.rotation);
            lastTarget = tr.position;
        }
    }

    public void setTargetPositions(Transform[] t)
    {
        this.positions = t;
        currPatrol = 0;
        lastTarget = positions[currPatrol].position;
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
            if (dir.sqrMagnitude > sleepVelocity * sleepVelocity)
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


        //Animation

        //Calculate the velocity relative to this transform's orientation
        Vector3 relVelocity = tr.InverseTransformDirection(velocity);
        relVelocity.y = 0;

        if (velocity.sqrMagnitude <= sleepVelocity * sleepVelocity)
        {
            //Fade out walking animation
            //anim.Blend("forward", 0, 0.2F);
        }
        else
        {
            //Fade in walking animation
            //anim.Blend("forward", 1, 0.2F);

            //Modify animation speed to match velocity
            //AnimationState state = anim["forward"];

            //float speed = relVelocity.z;
            //state.speed = speed * animationSpeed;
        }
    }

}
