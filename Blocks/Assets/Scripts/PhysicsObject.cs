using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
	protected const float MIN_MOVE_DISTANCE = 0.001f;

	public float gravityModifier = 1f;
	protected Vector2 _velocity;
	protected Rigidbody2D _rb;

	private void OnEnable()
	{
		_rb = GetComponent<Rigidbody2D>();
	}

	/// <summary>
	/// Called before the first frame update.
	/// </summary>
	private void Start()
	{
		
	}

	// Update is called once per frame  
	void Update()
	{
	}

	private void FixedUpdate()
	{
		// Move in response to gravity.
		_velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
		Move(Vector2.up * _velocity.y * Time.deltaTime);
	}

	protected void Move(Vector2 movement)
	{
		if (movement.magnitude < MIN_MOVE_DISTANCE)
		{
			return;
		}
		_rb.position += movement;
	}
}
