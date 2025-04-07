using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using TMPro;
using FishNet.Object.Synchronizing;
using System;
using System.Linq;

public class BaseRunner : NetworkBehaviour
{

	protected GameObject characterObject;
	[SerializeField] protected float baseSpeed;
	[SerializeField] protected float rotationSpeed = 1;
	[SerializeField] protected float groundDrag = 5f;
	[SerializeField] protected GameObject trailBoost;
	[SerializeField] protected LayerMask whatIsGround;
	[SerializeField] protected float runnerHeight = 2;
	[SerializeField] TextMeshPro nameTag;
	Coroutine animatorCoroutine;

	public readonly SyncVar<int> id = new SyncVar<int>(-1);

	protected Rigidbody rigidBody;
	protected Animator animator;

	protected float speedMultiplier = 1;
	public bool canMove = true;

	private void Awake()
	{
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.detectCollisions = true;
	}

	protected void BaseAwake()
	{
		animatorCoroutine = StartCoroutine(UpdateAnimatorAndBoostTrail());
	}

	private void OnDestroy()
	{
		StopCoroutine(animatorCoroutine);
	}

	[ServerRpc]
	protected void SetCharacter(CharacterData characterData, string playerName)
	{
				Debug.Log("[CHARLOAD] Server" + characterData.hairColor + " " + characterData.skinColor + " " + characterData.topColor + " " + characterData.bottomColor);

		LoadCharacter(characterData);
		SetNameTag(playerName);
	}

	[ObserversRpc]
	void SetNameTag(string name)
	{
		nameTag.text = name;
	}

	[ObserversRpc]
	void LoadCharacter(CharacterData characterData)
	{
		Debug.Log("Loading: " + characterData.characterName);
		Debug.Log("[CHARLOAD] Observers" + characterData.hairColor + " " + characterData.skinColor + " " + characterData.topColor + " " + characterData.bottomColor);
		Character character = CharacterLoader.GetCharacter(characterData);

		if (character.prefab == null)
		{
			Debug.LogError("Character Data is Null");
			return;
		}

		if (characterObject != null)
			Destroy(characterObject);

		characterObject = Instantiate(character.prefab, transform.position - Vector3.up, Quaternion.identity, transform);

		characterObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

		characterObject.GetComponent<CharacterPrefabColorChanger>().ChangeColors(character.hairColor, character.skinColor, character.topColor, character.bottomColor);
		characterObject.GetComponent<CharacterPrefabColorChanger>().ChangeShoe(ShoeLoader.GetMesh(character.shoes.id), ShoeLoader.getMaterials(character.shoes.materials));
	}

	List<GameObject> GetAllChildrenRecursive(GameObject gameObject)
	{
		List<GameObject> children = new List<GameObject>();
		for(int i = 0; i < gameObject.transform.childCount; i++)
		{
			children.Add(gameObject.transform.GetChild(i).gameObject);
			children.Concat(GetAllChildrenRecursive(gameObject.transform.GetChild(i).gameObject));
		}
		Debug.Log(children);
		return children;
	}

	protected void BaseFixedUpdate()
	{
		//Slow down character speed boost
		if (speedMultiplier > 1f)
		{
			speedMultiplier -= 0.01f;
			speedMultiplier = Mathf.Clamp(speedMultiplier, 1f, 10f);
		}
	}

	[ServerRpc]
	void TrailBoostServerRpc(bool on)
	{
		TrailBoostRpc(on);
	}

	[ObserversRpc]
	void TrailBoostRpc(bool on)
	{
		trailBoost.GetComponent<TrailRenderer>().emitting = on;
		if (on)
		{
			trailBoost.GetComponentInChildren<ParticleSystem>().Play();
		}
		else
		{
			trailBoost.GetComponentInChildren<ParticleSystem>().Stop();
		}
	}

	protected void BaseUpdate()
	{
		if (animator == null)
		{
			animator = GetComponentInChildren<Animator>();
		}
	}

	IEnumerator UpdateAnimatorAndBoostTrail()
	{
		while (IsOwner)
		{
			SetAnimatorParametersServerRpc(rigidBody.velocity.magnitude > 0.3f, !Physics.Raycast(transform.position, Vector3.down, out _, runnerHeight * 0.5f + 1f, whatIsGround), new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z).magnitude / 10);
			TrailBoostServerRpc(speedMultiplier > 1f);
			yield return new WaitForSeconds(0.5f);
		}
	}

	[ServerRpc]
	void SetAnimatorParametersServerRpc(bool running, bool falling, float speed)
	{
		SetAnimatorParametersObserversRpc(running, falling, speed);
	}

	[ObserversRpc]
	void SetAnimatorParametersObserversRpc(bool running, bool falling, float speed)
	{
		if (animator == null)
		{
			animator = GetComponentInChildren<Animator>();
		}
		if (animator == null)
		{
			return;
		}
		animator.SetBool("isRunning", running);
		animator.SetBool("isFalling", falling);
		animator.SetFloat("playerSpeed", speed);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag is "Goal")
		{
			GoalReached();
		}
		if(other.GetComponent<SpeedBoost>() != null)
		{
			speedMultiplier = other.GetComponent<SpeedBoost>().speedBoost;
			other.GetComponent<SpeedBoost>().FadeAndRespawn();
		}
	}

	void GoalReached()
	{
		if(!IsOwner)
			return;

		FreezeServerRpc();
		rigidBody.detectCollisions = false;
		rigidBody.isKinematic = true;
		rigidBody.velocity = Vector3.zero;
		GetComponent<Collider>().enabled = false;
		FindFirstObjectByType<GameManager>().GoalReached(id.Value);
	}

	[ServerRpc]
	void FreezeServerRpc()
	{
		Freeze();
	}

	[ObserversRpc]
	public void Freeze()
	{
		if (!IsOwner)
			return;
		//rigidBody.constraints = RigidbodyConstraints.FreezeAll;
		canMove = false;
		rigidBody.velocity = Vector3.zero;
	}

	[ObserversRpc(ExcludeServer = false, ExcludeOwner = false)]
	public void UnFreeze()
	{
		if (!IsOwner)
			return;
		//rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		Debug.Log("I'm Unfreezing");
		canMove = true;
	}

	public void SetId(int i)
	{
		id.Value = i;
		id.DirtyAll();
	}

	public int GetId()
	{
		return id.Value;
	}

	[ServerRpc (RequireOwnership = false)]
	void SetIdServerRpc(int i)
	{
		
	}
}
