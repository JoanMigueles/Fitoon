using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;

public class BotRunner : BaseRunner
{
    float moveV;
    float moveH;

	void FixedUpdate()
    {
		if (!canMove || !IsServerInitialized)
		{
			return;
		}

		//-------------------------------------------------------------------------------
		//Esto no tiene mucho sentido, pero es la forma en la que han entrenado los bots
		rigidBody.AddForce(transform.forward * baseSpeed * 10f, ForceMode.Force);

		//Rotate Player based on Horizontal input
		Vector3 rotation = new Vector3(0, moveH * rotationSpeed, 0);
		Vector3 currentRotation = transform.rotation.eulerAngles;
		Vector3 limitedRotation = RotationLimited(currentRotation);
		rigidBody.MoveRotation(Quaternion.Euler(limitedRotation + rotation));

		Vector3 moveDirection = transform.forward * moveV + transform.right * moveH;
		if (moveV != 0) rigidBody.AddForce(moveDirection.normalized * baseSpeed * 10f * Mathf.Max(0.1f, speedMultiplier), ForceMode.Force);
		//-------------------------------------------------------------------------------



		/* Si en algún momento se vuelven a entrenar los bots recomiendo usar esto, o algo parecido a esto
		rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, Quaternion.Euler(0, moveH, 0), rotationSpeed);
		rigidBody.AddForce(transform.forward * moveV * Mathf.Max(0.1f, speedMultiplier) * baseSpeed, ForceMode.VelocityChange);
		*/
		BaseFixedUpdate();
	}

	private void Update()
	{
		BaseUpdate();
		if (!IsServerInitialized || !canMove)
		{
			return;
		}

		RaycastHit hit;
		bool grounded = Physics.Raycast(transform.position, Vector3.down, out hit, 2 * 0.5f + 3f, whatIsGround);

		//Limit velocity
		Vector3 flatVel = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);

		if (flatVel.magnitude > baseSpeed)
		{
			Vector3 limitedVel = flatVel.normalized * baseSpeed;
			rigidBody.velocity = new Vector3(limitedVel.x, rigidBody.velocity.y, limitedVel.z);
		}

		//Handle drag


		if (grounded)
		{
			rigidBody.drag = groundDrag;
		}

		else if (!grounded)
		{
			rigidBody.drag = 0;
		}
	}

	public override void OnStartNetwork()
	{
		BaseAwake();
		if (IsServerInitialized)
		{
			Debug.Log("im on da server!");
			SetCharacter(CharacterLoader.CreateRandomCharacterData(), "");
		}
		else
		{
			GetComponent<BotRunner>().enabled = false;
			GetComponent<RunnerAgent>().enabled = false;
			GetComponent<DecisionRequester>().enabled = false;
			GetComponent<BehaviorParameters>().enabled = false;
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

	private Vector3 RotationLimited(Vector3 rotation)
	{
		if (rotation.y > 90 && rotation.y < 270)
		{
			if (rotation.y < 180)
			{
				rotation.y = 90; // Ajustar a 90 si está entre 90 y 180
			}
			else
			{
				rotation.y = 270; // Ajustar a 270 si está entre 180 y 270
			}
		}
		return rotation;
	}
}
