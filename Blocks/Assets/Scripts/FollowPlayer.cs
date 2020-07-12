using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	public Player player;

	/// <summary>
	/// Called once per frame.
	/// </summary>
	void Update()
	{
		transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
	}
}
