using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Managing.Statistic;
using FishNet.Component.Transforming;
using FishNet.Object.Synchronizing;

public class PlayerController : BaseRunner
{
	[SerializeField] new Camera camera;
    FaceTrackingToMovement faceTracking;

	public override void OnStartClient()
    {
		Debug.Log("Player Starting");
		BaseAwake();
		faceTracking = GetComponent<FaceTrackingToMovement>();

		if (Owner.IsLocalClient)
		{
			Camera.main.GetComponent<CameraFollowPlayer>().target = transform;
			SaveData.ReadFromJson();
			SetCharacter(SaveData.player.playerCharacterData, SaveData.player.username);
		}
		else
		{
			faceTracking.enabled = false;
			GetComponent<PlayerController>().enabled = false;
		}
	}

	private void FixedUpdate()
	{
#if !UNITY_EDITOR
		if (faceTracking == null || !faceTracking.detectado || !canMove || !IsOwner)
		{
			return;
		}
		Debug.Log("Player Moving");
		rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);
		rigidBody.velocity += baseSpeed * faceTracking.speed * Mathf.Max(0.1f, speedMultiplier) * transform.forward;
		rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, faceTracking.faceRotation, rotationSpeed);
#else
		if (!canMove || !IsOwner)
		{
			return;
		}
		rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);
		rigidBody.velocity += baseSpeed * Mathf.Max(0.1f, speedMultiplier) * transform.forward;
#endif
		if (Physics.Raycast(transform.position, Vector3.down, out _, runnerHeight * 0.5f + 1f, whatIsGround))
		{
			rigidBody.drag = groundDrag;
		}
		else
		{
			rigidBody.drag = 0;
		}
		BaseFixedUpdate();
	}

	void Update()
	{
		BaseUpdate();
	}
}
