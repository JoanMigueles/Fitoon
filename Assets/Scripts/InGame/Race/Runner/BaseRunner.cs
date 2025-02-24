using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class BaseRunner : NetworkBehaviour
{
	[SerializeField] protected float baseSpeed;
	[SerializeField] protected float rotationSpeed = .5f;
	[SerializeField] protected GameObject trailBoost;
	[SerializeField] protected LayerMask whatIsGround;
	[SerializeField] protected float runnerHeight = 2;

	int id;

	protected Rigidbody rigidBody;
	Animator animator;

	protected float speedMultiplier = 1;
	protected bool canMove = true;
	protected void BaseAwake()
	{
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.detectCollisions = true;
		Freeze();
	}
	protected void BaseUpdate()
	{
		if(animator != null) 
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
		animator.SetBool("isRunning", rigidBody.velocity.magnitude > 0.3f);
		animator.SetBool("isFalling", Physics.Raycast(transform.position, Vector3.down, out _, runnerHeight * 0.5f + 1f, whatIsGround));
		animator.SetFloat("playerSpeed", 0.3f + new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z).magnitude / 10);
	}
	public void LoadCharacter(Character character)
	{
		if(character.prefab == null)
		{
			Debug.LogError("Character Data is Null");
			return;
		}
		
		Instantiate(character.prefab, transform);

		List<GameObject> hair = new List<GameObject>();
		List<GameObject> skin = new List<GameObject>();
		List<GameObject> top = new List<GameObject>();
		List<GameObject> bottom = new List<GameObject>();
		GameObject shoes = null;

		for (int i = 0; i < transform.childCount; i++)
		{
			if (transform.GetChild(i).tag is "Hair")
				hair.Add(transform.GetChild(i).gameObject);
			else if (transform.GetChild(i).tag is "Skin")
				skin.Add(transform.GetChild(i).gameObject);
			else if (transform.GetChild(i).tag is "Top")
				top.Add(transform.GetChild(i).gameObject);
			else if (transform.GetChild(i).tag is "Bottom")
				bottom.Add(transform.GetChild(i).gameObject);
			else if (transform.GetChild(i).tag is "Shoes")
				shoes = transform.GetChild(i).gameObject;
		}
		foreach (GameObject gameObject in hair)
		{
			gameObject.GetComponent<SkinnedMeshRenderer>().material.color = character.hairColor;
		}
		foreach (GameObject gameObject in skin)
		{
			gameObject.GetComponent<SkinnedMeshRenderer>().material.color = character.skinColor;
		}
		foreach (GameObject gameObject in top)
		{
			gameObject.GetComponent<SkinnedMeshRenderer>().material.color = character.topColor;
		}
		foreach (GameObject gameObject in bottom)
		{
			gameObject.GetComponent<SkinnedMeshRenderer>().material.color = character.bottomColor;
		}
		if(shoes != null)
		{
			shoes.GetComponent<SkinnedMeshRenderer>().sharedMesh = character.shoes.mesh;
		}
		animator = GetComponentInChildren<Animator>();
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
		GameManager.GoalReached(this);
	}

	public void Boost(float amount, float duration)
	{
		StartCoroutine(BoostCoroutine(amount, duration));
	}
	public void Freeze()
	{
		rigidBody.constraints = RigidbodyConstraints.FreezeAll;
		canMove = false;
	}
	public void UnFreeze()
	{
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
