using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class PlayerMovement : MonoBehaviour
{
	private float horizontal;
	private bool isFacingRight = true;
	private bool canMove = true;

	[Header("Movement Variables")]
	[Tooltip("Constant speed the player moves")]
	[SerializeField] private float speed = 8f;
	[Tooltip("Amount of force to be applied to your vertical velocity when jumping")]
	[SerializeField] private float jumpingPower = 16f;
	[Tooltip("How many times the player is allowed to jump midair, the grounded jump is not included here")]
	[SerializeField] private int midairJumps = 0;
	[Tooltip("How Fast the player's jump will stop rising when they release spacebar. Higher numbers mean a faster slow, 0 results in a divide by 0, and decimals result in the player increasing in force")]
	[SerializeField] private float jumpReleaseSlow = 2f;
	[Tooltip("Amount of time after falling off a ledge that youre allowed to jump")]
	[SerializeField] private float coyoteTime = 0.1f;
	[Tooltip("Applied Velocity of dashing")]
	[SerializeField] private float dashingSpeed = 16f;
	[Tooltip("How long the dash lasts")]
	[SerializeField] private float dashDuration = 0.5f;
	[Tooltip("Time between dashes")]
	[SerializeField] private float dashCooldown = 1f;
	[Tooltip("Multiplier of how much faster the dash speed is when dash jumping")]
	[SerializeField] private float jumpDashMultiplier = 1.25f;

	private bool isWallSliding;
	private bool isWallJumping;
	private float wallJumpingDirection;
	private float wallJumpingCounter;
	private float groundEmissionRate;
	private float wallEmissionRate;
	private bool groundedLast = false;
	private float coyoteJumpTimer = 0f;
	private bool isGrounded = false;
	private float dashCounter = 0f;
	private float dashCooldownTimer = 0f;
	private bool jumpDashing = false;
	private bool jumpReleased = false;
	private int midairJumpsAvailable;
	

	[Header("Wall Jumping Variables")]
	[Tooltip("Speed the player slides down the wall")]
	[SerializeField] private float wallSlidingSpeed = 2f;
	[Tooltip("How long you can wait to press jump after leaving the wall. Like coyote time for wall jumping")]
	[SerializeField] private float wallJumpingTime = 0.2f;
	[Tooltip("How long the direction of a wall jump is applied")]
	[SerializeField] private float wallJumpingDuration = 0.4f;
	[Tooltip("Max distance that the Wall check can be from a wall to allow a wall jump")]
	[SerializeField] private float maxWallDistance = 0.2f;
	[Tooltip("This Vector controls the angle and power that the player comes off the wall with")]
	[SerializeField]private Vector2 wallJumpingPower = new Vector2(8f, 16f);


	[Header("References")]
	[SerializeField] private Rigidbody2D rb;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform wallCheck;
	[SerializeField] private LayerMask wallLayer;
	[SerializeField] private ParticleSystem groundParticles;
	[SerializeField] private ParticleSystem wallParticles;
	[SerializeField] private Collider2D playerCollider;
	[SerializeField] private Animator animator;


	private void Awake()
	{
		groundEmissionRate = groundParticles.emissionRate;
		groundParticles.emissionRate = 0;
		wallEmissionRate = wallParticles.emissionRate;
		wallParticles.emissionRate = 0;
	}

	private void Update()
	{
		if (!canMove) { return; }
		groundedLast = isGrounded;
		boxCast();
		if (!isGrounded && groundedLast)
		{ //on walking off a ledge
			coyoteJumpTimer = coyoteTime;
		}
		if(isGrounded && !groundedLast)
		{//On landing
			dashCounter = 0f; //End the dash if one is happening
		}
		if(Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && dashCooldownTimer <= 0 && dashCounter  <= 0)
		{
			StartDash();
		}
		
		
		horizontal = Input.GetAxisRaw("Horizontal");
		

		if (Input.GetButtonDown("Jump") && canJump())
		{
			//Debug.Log("Jump");
			rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
			jumpReleased = false;

			if (!isGrounded && !isWallSliding)
			{
				midairJumpsAvailable -= 1;
			}
			if (dashCounter > 0f)
			{
				jumpDashing = true;
				dashCounter = dashDuration;//Let the player jump while dashing to continue their dash
			}
		}
		if(Input.GetButtonUp("Jump") && rb.velocity.y > 0.01f) //User 0.01 instead of 0 cause unity rigid bodies are weird
		{ //If statement within the update to make sure we get the player's input
			jumpReleased = true;
		}


		WallSlide();
		WallJump();

		if (!isWallJumping)
		{
			Flip();
		}
		animator.SetFloat("yVelocity", rb.velocity.y);
		animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
		animator.SetBool("Grounded", isGrounded);

	}


	private void FixedUpdate()
	{
		coyoteCountdown();
		dashCountdown();

		//Ground particles Code
		if (isGrounded && rb.velocity != Vector2.zero)
		{
			groundParticles.emissionRate = groundEmissionRate;
		}
		else
		{
			groundParticles.emissionRate = 0;
		}

		if (!canMove) { return; }

		if (jumpReleased && rb.velocity.y > 0.01f)
		{//Actual jump release slow within FixedUpdate to make sure that the slow is consistent across different frame rates
			rb.velocity = new Vector2 (rb.velocity.x, rb.velocity.y / jumpReleaseSlow);
			Debug.Log(midairJumpsAvailable + ":slowing");
		}

		if (!isWallJumping)
		{
			if (dashCounter > 0f)
			{ //code for dashing
				animator.speed = dashingSpeed / speed;
				if(jumpDashing)
				{
					rb.velocity = new Vector2(horizontal * dashingSpeed * jumpDashMultiplier, rb.velocity.y);
				}
				else
				{
					rb.velocity = new Vector2(horizontal * dashingSpeed, rb.velocity.y);
				}
			}
			else
			{//Regular movement code
				animator.speed = 1f;
				rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
			}


		}
	}


	private bool IsWalled()
	{
		return Physics2D.OverlapCircle(wallCheck.position, maxWallDistance, wallLayer);
	}

	private void WallSlide()
	{
		if (IsWalled() && !isGrounded && horizontal != 0f)
		{
			jumpReleased = false; //We have to reset the players jump release if they are on a wall
			isWallSliding = true;
			rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
			wallParticles.emissionRate = wallEmissionRate;
			animator.SetBool("WallSliding", isWallSliding);
		}
		else
		{
			isWallSliding = false;
			wallParticles.emissionRate = 0;
			animator.SetBool("WallSliding", isWallSliding);

		}
	}

	private void WallJump()
	{
		if (isWallSliding)
		{
			isWallJumping = false;
			wallJumpingDirection = -transform.localScale.x; //direction we are facing
			wallJumpingCounter = wallJumpingTime;
			dashCounter = 0f; // for good measure
			CancelInvoke(nameof(StopWallJumping));
		}
		else
		{
			wallJumpingCounter -= Time.deltaTime; //Allows the equivalent of coyote time for wall jumping
		}

		if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
		{
			isWallJumping = true;
			rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
			wallJumpingCounter = 0f;

			if (transform.localScale.x != wallJumpingDirection)
			{
				isFacingRight = !isFacingRight;
				Vector3 localScale = transform.localScale;
				localScale.x *= -1f;
				transform.localScale = localScale;
			}

			Invoke(nameof(StopWallJumping), wallJumpingDuration);
		}
	}

	private void StopWallJumping()
	{
		isWallJumping = false;
	}

	private void Flip()
	{
		if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
		{
			isFacingRight = !isFacingRight;
			Vector3 localScale = transform.localScale;
			localScale.x *= -1f;
			transform.localScale = localScale;
			dashCounter = 0f;
		}
		
		
	}

	private void boxCast()
	{
		// positions box cast at bottom center of player collider
		Vector3 colliderCenter = playerCollider.bounds.center;
		Vector2 colliderCenter2D = new Vector2(colliderCenter.x, colliderCenter.y);

		Vector3 playerColliderSize = playerCollider.bounds.size;
		Vector2 playerColliderSize2D = new Vector2(playerColliderSize.x, playerColliderSize.y);


		RaycastHit2D raycastHit = Physics2D.BoxCast(colliderCenter2D, playerColliderSize2D / 2, 0f, Vector2.down, 1f, groundLayer);
		
		

		if (raycastHit.collider == null) { isGrounded = false; }
		else { 
			isGrounded = true;
			jumpReleased = false; //reset this variable for the next jump
			midairJumpsAvailable = midairJumps;
		}
	}

	private void coyoteCountdown()
	{
        if (coyoteJumpTimer > 0)
        {
			coyoteJumpTimer -= Time.deltaTime;
        }
    }

	private void dashCountdown()
	{
		if (dashCounter > 0)
		{
			dashCounter -= Time.deltaTime;
		}
		else
		{
			jumpDashing = false;
		}
		if(dashCooldownTimer > 0)
		{
			dashCooldownTimer -= Time.deltaTime;
		}
	}
	private void StartDash()
	{
		dashCooldownTimer = dashCooldown;
		dashCounter = dashDuration;
	}

	public bool isDashing()
	{
		return (dashCounter > 0);
	}

	private bool canJump()
	{
		return midairJumps == 0 && (((isGrounded || midairJumpsAvailable > 0) || coyoteJumpTimer > 0) && rb.velocity.y < 0.1f ) || //do the ground velocity check only without double jumps
			   midairJumps > 0 && ((isGrounded || midairJumpsAvailable > 0) || coyoteJumpTimer > 0); //if you have double jumps let this happen
	}
	public void setMidairJumps(int x)
	{
		midairJumps = x;
	}
	public void setCanMove(bool b)
	{
		canMove = b;
		rb.velocity = Vector3.zero;
		animator.SetFloat("yVelocity", rb.velocity.y);
		animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
		animator.SetBool("Grounded", isGrounded);

	}

	//This function is exclusive to "Where?"
	public void growWings()
	{
		midairJumps = 1;
		midairJumpsAvailable = 1;
		PlayerProgression.canDoubleJump = true;
	}
}