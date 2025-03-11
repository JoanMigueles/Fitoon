using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotRunner : BaseRunner
{
    float moveV;
    float moveH;

	void FixedUpdate()
    {
		if (!canMove)
		{
			return;
		}

		rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, Quaternion.Euler(0, moveH, 0), rotationSpeed);
		rigidBody.AddForce(transform.forward * moveV * Mathf.Max(0.1f, speedMultiplier) * baseSpeed, ForceMode.VelocityChange);
	}

	public override void OnStartNetwork()
	{
		BaseAwake();
		if (IsServerInitialized)
		{
			Debug.Log("im on da server!");
			if (characterData == null)
				characterData = CharacterLoader.CreateRandomCharacterData();

			_characterData.Value = characterData;
		}
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
