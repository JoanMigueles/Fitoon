using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotRunner : BaseRunner
{
    float moveV;
    float moveH;

	private void Awake()
	{
		BaseAwake();
	}
	void FixedUpdate()
    {
		if (!canMove)
		{
			return;
		}

		rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, Quaternion.Euler(0, moveH, 0), rotationSpeed);
		rigidBody.AddForce(transform.forward * moveV * Mathf.Max(0.1f, speedMultiplier) * baseSpeed, ForceMode.VelocityChange);
	}

	public void SetMovement(float moveV, float moveH)
	{
		if (canMove)
		{
			this.moveV = moveV;
			this.moveH = moveH;
		}
	}
}
