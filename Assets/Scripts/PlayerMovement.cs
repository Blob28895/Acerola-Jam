using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
	private float horizontal;
	private bool isFacingRight = true;

	[Header("Movement Variables")]
	[Tooltip("Constant speed the player moves")]
	[SerializeField] private float speed = 8f;
	[Tooltip("Amount of force to be applied to your vertical velocity when jumping")]
	[SerializeField] private float jumpingPower = 16f;
	[Tooltip("Amount of time after falling off a ledge that youre allowed to jump")]
	[SerializeField] private float coyoteTime = 0.1f;

	private bool isWallSliding;
	private bool isWallJumping;
	private float wallJumpingDirection;
	private float wallJumpingCounter;
	private Vector2 wallJumpingPower = new Vector2(8f, 16f);
	private float groundEmissionRate;
	private float wallEmissionRate;
	private bool groundedLast = false;
	private float coyoteJumpTimer = 0f;
	private bool isGrounded = false;

	[Header("Wall Jumping Variables")]
	[Tooltip("Speed the player slides down the wall")]
	[SerializeField] private float wallSlidingSpeed = 2f;
	[Tooltip("How long you can wait to press jump after leaving the wall. Like coyote time for wall jumping")]
	[SerializeField] private float wallJumpingTime = 0.2f;
	[Tooltip("How long the direction of a wall jump is applied")]
	[SerializeField] private float wallJumpingDuration = 0.4f;
	[Tooltip("Max distance that the Wall check can be from a wall to allow a wall jump")]
	[SerializeField] private float maxWallDistance = 0.2f;


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

		groundedLast = isGrounded;
		boxCast();
		if (!isGrounded && groundedLast)
		{
			coyoteJumpTimer = coyoteTime;
		}

		horizontal = Input.GetAxisRaw("Horizontal");

		if (Input.GetButtonDown("Jump") && (isGrounded || coyoteJumpTimer > 0))
		{
			rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
		}

		if (Input.GetButtonUp("Jump") && isGrounded)
		{
			rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
		}

		WallSlide();
		WallJump();

		if (!isWallJumping)
		{
			Flip();
		}
	}


	private void FixedUpdate()
	{
		coyoteCountdown();

		if (!isWallJumping)
		{
			rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
			if (isGrounded && rb.velocity != Vector2.zero)
			{
				Debug.Log(rb.velocity);
				groundParticles.emissionRate = groundEmissionRate;
			}
			else
			{
				groundParticles.emissionRate = 0;
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
		else { isGrounded = true; }
	}

	private void coyoteCountdown()
	{
        if (coyoteJumpTimer > 0)
        {
			coyoteJumpTimer -= Time.deltaTime;
        }
    }
}