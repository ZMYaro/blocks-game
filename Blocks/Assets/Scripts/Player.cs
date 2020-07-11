using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private const float ACC_FACTOR = 0.7f;
	private const float MAX_WALK_SPEED = 2.1f;
	private const float BASE_JUMP_ACC = 5f;
	private const float HIGH_JUMP_MIN_SPEED = 1f;

	public LayerMask platformLayerMask;

	private BoxCollider2D _collider;
	private Rigidbody2D _rb;

	private void Start()
	{
		_collider = GetComponent<BoxCollider2D>();
		_rb = GetComponent<Rigidbody2D>();
	}

	/// <summary>
	/// Called once per frame.
	/// </summary>
	private void Update()
	{
		HandleHorizMovement();
		HandleJump();
	}

	private void HandleHorizMovement()
	{
		// Accelerate in response to player input.
		float moveInput = Input.GetAxisRaw("Horizontal");
		Vector2 newVelocity = _rb.velocity + new Vector2(moveInput * ACC_FACTOR, 0);
		// Limit maximum speed.
		newVelocity.x = Mathf.Clamp(newVelocity.x, -MAX_WALK_SPEED, MAX_WALK_SPEED);
		// Update velocity.
		_rb.velocity = newVelocity;
		//_rb.AddForce(movement);
		//_rb.velocity += acceleration;
		transform.localScale = new Vector2(
			_rb.velocity.x < -0.1f ? -1 :
				_rb.velocity.x > 0.1f ? 1 : transform.localScale.x,
			1);
	}

	private void HandleJump()
	{
		if (!IsOnGround() || !Input.GetButtonDown("Jump"))
		{
			return;
		}

		float jumpAcc = BASE_JUMP_ACC;
		if (Mathf.Abs(_rb.velocity.x) > HIGH_JUMP_MIN_SPEED)
		{
			jumpAcc += Mathf.Abs(_rb.velocity.x) - HIGH_JUMP_MIN_SPEED;
		}
		_rb.velocity = new Vector2(_rb.velocity.x, jumpAcc);
	}

	private bool IsOnGround()
	{
		const float ALLOWED_SPACE_BELOW = 0.02f;
		RaycastHit2D raycastHit = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0, Vector2.down, ALLOWED_SPACE_BELOW, platformLayerMask);
		return raycastHit.collider != null;
	}
}
