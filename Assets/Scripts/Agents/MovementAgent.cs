using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MovementAgent : Agent
{
    [Header("Senses")]
    public Transform enemy;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    public float closestDistance;

    const float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private Rigidbody2D rb;
	private bool facingRight = true;  // For determining which way the player is currently facing.
	private Vector3 myVelocity = Vector3.zero;

    [Header("Motion")]
    [SerializeField] private float maxJumpHeight = 4f;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float movementSmoothing = .05f;

    [Header("Episode")]
    public List<Vector3> startPosition;
    public EpisodeController epCntl;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer sprite;
    public Color winColor;
    public SpriteRenderer background;

    [Header("Reward")]
    public float timeSinceGoal;
    public float additionalReward = 40f;

    [Header("Spawning")]
    public float minSpawnDist = 5f;
    public Transform bottomLeftBound;
    public Transform topRightBound;

    //ML Stuff
    public override void OnEpisodeBegin()
    {   
        transform.localPosition = FindSpawnLocation(enemy, minSpawnDist);
        timeSinceGoal = 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(enemy.transform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Collect actions
        float moveX = actions.ContinuousActions[0];
        //float jumpStrength = Mathf.Abs(actions.ContinuousActions[1]);
        float moveY = actions.ContinuousActions[1];

        //Now move
        //MoveX(moveX);
        //Jump(jumpStrength);
        //MoveY(moveY);
        transform.position += new Vector3(moveX, moveY, 0) * Time.deltaTime * moveSpeed;
        
        //iterate timesincegoal
        timeSinceGoal += Time.deltaTime;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
        
    }


    //Platforming Stuff
    public virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        epCntl.AddAgent(this);
    }

    private void MoveX(float dir){
        Vector3 targetVelocity = new Vector2(dir * moveSpeed, rb.velocity.y);
        // And then smoothing it out and applying it to the character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref myVelocity, movementSmoothing);
        //transform.position += new Vector3(dir * moveSpeed * Time.deltaTime, rb.velocity.y,0);

        // If the input is moving the player right and the player is facing left...
        if (dir > 0 && !facingRight)
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (dir < 0 && facingRight)
        {
            // ... flip the player.
            Flip();
        } 
    }

    private void MoveY(float dir){
        Vector3 targetVelocity = new Vector2(rb.velocity.x, dir * moveSpeed);
        // And then smoothing it out and applying it to the character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref myVelocity, movementSmoothing);
    }

    private void Jump(float strength){
        if(isGrounded()){
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(-2.0f * Physics2D.gravity.y * (maxJumpHeight*strength)));
            //rb.velocity = new Vector2(rb.velocity.x, 10f * strength);
        }
    }

    private bool isGrounded(){
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, groundMask);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				return true;
			}
		}
        return false;
    }

    private void Flip()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		sprite.flipX = !sprite.flipX;
	}

    public float CalculateAdditionalReward(){
        return additionalReward/timeSinceGoal;
    }

    public Vector3 FindSpawnLocation(Transform objectToAvoid, float minDist){
        while(true){
            Vector3 newPos = new Vector3(Random.Range(bottomLeftBound.localPosition.x,topRightBound.localPosition.x),Random.Range(bottomLeftBound.localPosition.y,topRightBound.localPosition.y),0);
            if( Vector3.Distance(objectToAvoid.localPosition,newPos) >= minDist && Physics2D.OverlapCircle(newPos, 0.8f, groundMask)==null){
                return newPos;
            }
        }
    }

}
