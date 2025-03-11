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
	protected readonly SyncVar<CharacterData> _characterData = new SyncVar<CharacterData>();
	public CharacterData characterData;
	readonly SyncVar<bool> isFalling = new SyncVar<bool>();
	readonly SyncVar<bool> isRunning = new SyncVar<bool>();
	readonly SyncVar<float> playerSpeed = new SyncVar<float>();

	private GameObject characterObject;
	[SerializeField] protected float baseSpeed;
	[SerializeField] protected float rotationSpeed = .5f;
	[SerializeField] protected GameObject trailBoost;
	[SerializeField] protected LayerMask whatIsGround;
	[SerializeField] protected float runnerHeight = 2;
	[SerializeField] TextMeshProUGUI nameTag;

	int id;

	protected Rigidbody rigidBody;
	Animator animator;

	protected float speedMultiplier = 1;
	protected bool canMove = true;
	void Start()
	{
		if (!IsServerInitialized)
		{
			LoadCharacter(_characterData.Value);
		}
	}
	protected void BaseAwake()
	{
		_characterData.OnChange += LoadCharacter;

		rigidBody = GetComponent<Rigidbody>();
		rigidBody.detectCollisions = true;
		isFalling.OnChange += IsFallingOnChange;
		isRunning.OnChange += IsRunningOnChange;
		playerSpeed.OnChange += PlayerSpeedOnChange;
		//Freeze();
	}

	private void PlayerSpeedOnChange(float prev, float next, bool asServer)
	{
		if(animator != null)
			animator.SetFloat("playerSpeed", next);
	}

	private void IsRunningOnChange(bool prev, bool next, bool asServer)
	{
		if (animator != null)
			animator.SetBool("isRunning", next);
	}

	private void IsFallingOnChange(bool prev, bool next, bool asServer)
	{
		if (animator != null)
			animator.SetBool("isFalling", next);
	}

	public void LoadCharacter(CharacterData prev, CharacterData next, bool asServer)
	{
		//Debug.Log("Loading Character");
		Debug.Log("Loading: " + next.characterName);
		Character character = CharacterLoader.GetCharacter(next);

		if (character.prefab == null)
		{
			Debug.LogError("Character Data is Null");
			return;
		}

		if (characterObject != null)
			Destroy(characterObject);

		characterObject = Instantiate(character.prefab, transform);
		if(IsOwner)
			Debug.Log("Instantiated: " + characterObject.name);
		characterObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

		characterObject.GetComponent<CharacterPrefabColorChanger>().ChangeColors(character.hairColor, character.skinColor, character.topColor, character.bottomColor);
		characterObject.GetComponent<CharacterPrefabColorChanger>().ChangeShoe(ShoeLoader.GetMesh(character.shoes.id), ShoeLoader.getMaterials(character.shoes.materials));

		animator = GetComponentInChildren<Animator>();
	}
	public void LoadCharacter(CharacterData next)
	{
		//Debug.Log("Loading Character");
		Debug.Log("Loading: " + next.characterName);
		Character character = CharacterLoader.GetCharacter(next);

		if (character.prefab == null)
		{
			Debug.LogError("Character Data is Null");
			return;
		}

		if (characterObject != null)
			Destroy(characterObject);

		characterObject = Instantiate(character.prefab, transform);
		characterObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

		characterObject.GetComponent<CharacterPrefabColorChanger>().ChangeColors(character.hairColor, character.skinColor, character.topColor, character.bottomColor);
		characterObject.GetComponent<CharacterPrefabColorChanger>().ChangeShoe(ShoeLoader.GetMesh(character.shoes.id), ShoeLoader.getMaterials(character.shoes.materials));

		animator = GetComponentInChildren<Animator>();
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

	private void OnDestroy()
	{
		Debug.Log("ME CAGO EN TODO"); Debug.Log("ME CAGO EN TODO"); Debug.Log("ME CAGO EN TODO"); Debug.Log("ME CAGO EN TODO"); Debug.Log("ME CAGO EN TODO"); Debug.Log("ME CAGO EN TODO"); Debug.Log("ME CAGO EN TODO"); Debug.Log("ME CAGO EN TODO"); Debug.Log("ME CAGO EN TODO"); Debug.Log("ME CAGO EN TODO"); Debug.Log("ME CAGO EN TODO"); Debug.Log("ME CAGO EN TODO"); Debug.Log("ME CAGO EN TODO"); Debug.Log("ME CAGO EN TODO"); Debug.Log("ME CAGO EN TODO");
	}

	protected void BaseUpdate()
	{
		if (animator != null && IsServerInitialized) 
		{
			UpdateAnimator();
		}
		UpdateBoostTrail();
	}
	void UpdateBoostTrail()
	{
		if (speedMultiplier > 1)
		{
			trailBoost.GetComponent<TrailRenderer>().emitting = true;
			trailBoost.GetComponentInChildren<ParticleSystem>().Play();
		}
		else
		{
			trailBoost.GetComponent<TrailRenderer>().emitting = false;
			trailBoost.GetComponentInChildren<ParticleSystem>().Stop();
		}
	}
	void UpdateAnimator()
	{
		isRunning.Value = rigidBody.velocity.magnitude > 0.3f;
		isFalling.Value = !Physics.Raycast(transform.position, Vector3.down, out _, runnerHeight * 0.5f + 1f, whatIsGround);
		playerSpeed.Value = 0.3f + new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z).magnitude / 10;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag is "Goal")
		{
			GoalReached();
		}
	}

	void GoalReached()
	{
		Freeze();
		rigidBody.detectCollisions = false;
		GameManager.Instance.GoalReached(this);
	}

	public void Boost(float amount, float duration)
	{
		StartCoroutine(BoostCoroutine(amount, duration));
	}

	[ObserversRpc]
	public void Freeze()
	{
		if (!IsOwner)
			return;
		rigidBody.constraints = RigidbodyConstraints.FreezeAll;
		canMove = false;
	}

	[ObserversRpc]
	public void UnFreeze()
	{
		if (!IsOwner)
			return;
		rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		canMove = true;
	}

	public void SetId(int i)
	{
		id = i;
	}

	public int GetId()
	{
		return id;
	}

	IEnumerator BoostCoroutine(float amount, float duration)
	{
		speedMultiplier += amount;
		float decay = duration / Time.fixedDeltaTime;
		float counter = 0;
		while(counter < amount)
		{
			yield return new WaitForFixedUpdate();
			counter += decay;
			speedMultiplier -= decay;
		}
	}
}
