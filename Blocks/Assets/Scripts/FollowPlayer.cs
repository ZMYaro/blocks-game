using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	public Player player;
	public Vector2 offset;

	/// <summary>
	/// Called once per frame.
	/// </summary>
	void Update()
	{
		transform.position = new Vector3(
			player.transform.position.x + offset.x,
			player.transform.position.y + offset.y,
			transform.position.z);
	}
}
