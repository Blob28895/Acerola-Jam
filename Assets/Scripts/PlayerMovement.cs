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

	private bool isWallSliding;
	private bool isWallJumping;
	private float wallJumpingDirection;
	private float wallJumpingCounter;
	private Vector2 wallJumpingPower = new Vector2(8f, 16f);
	private float groundEmissionRate;

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
	[SerializeField] private Transform groundCheck;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform wallCheck;
	[SerializeField] private LayerMask wallLayer;
	[SerializeField] private ParticleSystem groundParticles;


	private void Awake()
	{
		groundEmissionRate = groundParticles.emissionRate;
		groundParticles.emissionRate = 0;
	}

	private void Update()
	{
		horizontal = Input.GetAxisRaw("Horizontal");

		if (Input.GetButtonDown("Jump") && IsGrounded())
		{
			rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
		}

		if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
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
		if (!isWallJumping)
		{
			rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
			if (IsGrounded() && rb.velocity != Vector2.zero)
			{
				groundParticles.emissionRate = groundEmissionRate;
			}
			else
			{
				groundParticles.emissionRate = 0;
			}
		}
	}

	private bool IsGrounded()
	{
		return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
	}

	private bool IsWalled()
	{
		return Physics2D.OverlapCircle(wallCheck.position, maxWallDistance, wallLayer);
	}

	private void WallSlide()
	{
		if (IsWalled() && !IsGrounded() && horizontal != 0f)
		{
			isWallSliding = true;
			rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
		}
		else
		{
			isWallSliding = false;
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
}