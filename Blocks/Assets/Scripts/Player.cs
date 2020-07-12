using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private const float ACC_FACTOR = 0.7f;
	private const float MAX_WALK_SPEED = 2.1f;
	private const float BASE_JUMP_ACC = 5f;
	private const float HIGH_JUMP_MIN_SPEED = 1f;
	private const float HIGH_JUMP_MULTIPLIER = 1.5f;
	private const string RESPAWN_POINT_TAG = "Respawn";
	private const string KILL_TRIGGER_TAG = "KillTrigger";

	public LayerMask platformLayerMask;
	public GameObject screenBlackout;

	private BoxCollider2D _collider;
	private Rigidbody2D _rb;

	private void Start()
	{
		_collider = GetComponent<BoxCollider2D>();
		_rb = GetComponent<Rigidbody2D>();

		UpdateCollider();
		StartCoroutine(Respawn());
	}

	private IEnumerator Respawn()
	{
		screenBlackout?.SetActive(true);

		GameObject spawnPoint = GameObject.FindGameObjectWithTag(RESPAWN_POINT_TAG);
		this.transform.position = spawnPoint.transform.position;

		yield return new WaitForSeconds(0.25f);
		screenBlackout?.SetActive(false);
	}

	/// <summary>
	/// Recalculate and update the size of the collider depending on the number of held boxes.
	/// </summary>
	private void UpdateCollider()
	{
		GlowBox[] boxes = GetComponentsInChildren<GlowBox>();
		const int PLAYER_HEIGHT = 64;
		const int BOX_SIZE = 48;
		_collider.size = new Vector2(BOX_SIZE / 100f, (PLAYER_HEIGHT + (boxes.Length * BOX_SIZE)) / 100f);
		_collider.offset = new Vector2(0, _collider.size.y / 2f);
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
		if (!IsOnGroundOrWall() || !Input.GetButtonDown("Jump"))
		{
			return;
		}

		float jumpAcc = BASE_JUMP_ACC;
		if (Mathf.Abs(_rb.velocity.x) > HIGH_JUMP_MIN_SPEED)
		{
			jumpAcc += (Mathf.Abs(_rb.velocity.x) - HIGH_JUMP_MIN_SPEED) * HIGH_JUMP_MULTIPLIER;
		}
		_rb.velocity = new Vector2(_rb.velocity.x, jumpAcc);
	}

	private bool IsOnGroundOrWall()
	{
		const float ALLOWED_SPACE_AROUND = 0.02f;
		RaycastHit2D raycastHit = Physics2D.BoxCast(
			_collider.bounds.center,
			_collider.bounds.size + new Vector3(ALLOWED_SPACE_AROUND, ALLOWED_SPACE_AROUND, 0),
			0,
			Vector2.down,
			ALLOWED_SPACE_AROUND,
			platformLayerMask);
		return raycastHit.collider != null;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag != KILL_TRIGGER_TAG)
		{
			return;
		}
		StartCoroutine(Respawn());
	}
}
