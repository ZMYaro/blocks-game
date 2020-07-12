using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GlowBox : MonoBehaviour
{
	public const int SIZE = 48;

	private BoxCollider2D _collider;
	private Rigidbody2D _rb;

	/// <summary>
	/// Called before the first frame update.
	/// </summary>
	public void Start()
	{
		_collider = GetComponent<BoxCollider2D>();
		_rb = GetComponent<Rigidbody2D>();

		_collider.enabled = false;
		_rb.bodyType = RigidbodyType2D.Kinematic;
	}

	public void Release()
	{
		Release(new Vector2(0, 0));
	}

	public void Release(Vector2 velocity)
	{
		// Remove the box from its parent.
		transform.parent = null;
		transform.position = transform.position + (Vector3)(velocity * 0.5f);

		// Enable it as its own physics object.
		_rb.bodyType = RigidbodyType2D.Dynamic;
		_rb.velocity = velocity;
		_collider.enabled = true;
	}
}
