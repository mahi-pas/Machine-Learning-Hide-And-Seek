using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PlatformingAgent : Agent
{
    [Header("Senses")]
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    const float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private Rigidbody2D rb;
	private bool facingRight = true;  // For determining which way the player is currently facing.
	private Vector3 myVelocity = Vector3.zero;

    [Header("Motion")]
    [SerializeField] private float maxJumpHeight;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float movementSmoothing = .05f;

    //ML Stuff
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Collect actions
        float moveX = actions.ContinuousActions[0];
        float jumpStrength = Mathf.Abs(actions.ContinuousActions[1]);

        //Now move
        MoveX(moveX);
        Jump(jumpStrength);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        if(Input.GetButtonDown("Jump")){
            continuousActions[1] = 1f;
        }
        
    }


    //Platforming Stuff
    public virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void MoveX(float dir){
        Vector3 targetVelocity = new Vector2(dir * moveSpeed, rb.velocity.y);
        // And then smoothing it out and applying it to the character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref myVelocity, movementSmoothing);

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

    private void Jump(float strength){
        if(isGrounded()){
            rb.AddForce(new Vector2(0f, Mathf.Sqrt(-2.0f * Physics2D.gravity.y * maxJumpHeight) * strength));
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

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

}
