using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerController : BaseRunner
{
	[SerializeField] new Camera camera;
    FaceTrackingToMovement faceTracking;

	public override void OnStartNetwork()
    {
		Debug.Log("Player Starting");
		BaseAwake();
		if (Owner.IsLocalClient)
		{
			faceTracking = GetComponent<FaceTrackingToMovement>();
		}
		else
		{
			Destroy(camera.gameObject);
		}
		if(IsServerInitialized)
		{
			Debug.Log("I'm" + characterData.characterName);
			_characterData.Value = characterData;
		}
	}

	public override void OnStartClient()
	{
		
	}

	private void FixedUpdate()
	{
		if (faceTracking == null || !faceTracking.detectado || !canMove)
		{
			return;
		}

		rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, faceTracking.faceRotation, rotationSpeed);
		rigidBody.AddForce(transform.forward * faceTracking.speed * Mathf.Max(0.1f, speedMultiplier) * baseSpeed, ForceMode.VelocityChange);
	}

	void Update()
	{
		BaseUpdate();
	}
}
