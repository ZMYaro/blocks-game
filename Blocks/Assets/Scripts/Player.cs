using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private const int HEIGHT = 64;
	private const float ACC_FACTOR = 0.7f;
	private const float MAX_WALK_SPEED = 2.1f;
	private const float BASE_JUMP_ACC = 5f;
	private const float HIGH_JUMP_MIN_SPEED = 1f;
	private const float HIGH_JUMP_MULTIPLIER = 1.5f;
	private const float MIN_THROW_SPEED = 1f;
	private const float THROW_SPEED_MULTIPLIER = 1f;
	private const string RESPAWN_POINT_TAG = "Respawn";
	private const string KILL_TRIGGER_TAG = "KillTrigger";

	public LayerMask platformLayerMask;
	/// <summary>The carried boxes, from bottom to top</summary>
	public List<GlowBox> boxes;
	/// <summary>Object that turns the screen black when enabled</summary>
	public GameObject screenBlackout;

	private BoxCollider2D _collider;
	private Rigidbody2D _rb;

	/// <summary>
	/// Called before the first frame update.
	/// </summary>
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
		_collider.size = new Vector2(GlowBox.SIZE / 100f, (HEIGHT + (boxes.Count * GlowBox.SIZE)) / 100f);
		_collider.offset = new Vector2(0, _collider.size.y / 2f);
	}

	/// <summary>
	/// Called once per frame.
	/// </summary>
	private void Update()
	{
		HandleHorizMovement();
		HandleJump();
		HandleThrow();
	}

	/// <summary>
	/// Check whether there was an input to move and handle it.
	/// </summary>
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

	/// <summary>
	/// Check whether there was an input to jump and handle it.
	/// </summary>
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

	/// <summary>
	/// Check whether there was an input to throw a box and handle it.
	/// </summary>
	private void HandleThrow()
	{
		string colorTag =
			Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1) ? "Red" :
			Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2) ? "Green" :
			Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3) ? "Yellow" :
			Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4) ? "Cyan" :
			Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5) ? "Purple" : null;
		
		if (colorTag == null)
		{
			return;
		}
		ThrowBox(colorTag);
	}

	private void ThrowBox(string colorTag)
	{
		GlowBox thrownBox = null;
		foreach (GlowBox box in boxes)
		{
			if (box.tag == colorTag)
			{
				// Throw in the direction you are moving.
				Vector2 throwVelocity = _rb.velocity;
				throwVelocity *= THROW_SPEED_MULTIPLIER;
				// If standing still, throw whichever direction the player is facing.
				if (throwVelocity.magnitude < MIN_THROW_SPEED)
				{
					throwVelocity.x = transform.localScale.x < 0 ? -MIN_THROW_SPEED : MIN_THROW_SPEED;
				}
				box.Release(throwVelocity);
				// Note the box that was found so it can be removed and subsequent ones can shift down.
				thrownBox = box;
			}
			else if (thrownBox != null)
			{
				// Shift subsequent boxes down.
				box.transform.position = new Vector2(
					box.transform.position.x,
					box.transform.position.y - (GlowBox.SIZE / 100f));
			}
		}
		// Remove the box from the boxes list and recalculate the player's collider size.
		boxes.Remove(thrownBox);
		UpdateCollider();
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
