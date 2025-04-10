using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to handle the speed boost power-up in the game.
/// </summary>
public class SpeedBoost : NetworkBehaviour
{
    public float speedBoost = 2;
    public Material matFade;
    public List<MeshRenderer> mrBoost;
    public bool isTemporary = true;

    [SerializeField] private List<Material> defaultMaterials;
    private Collider coll;


    private bool isRespawning = false;
    private float timer = 0f;
    private float timerMax = 200f;

    public override void OnStartNetwork()
	{
        coll = GetComponent<Collider>();

        defaultMaterials.Capacity = mrBoost.Count;
        for (int i=0; i< mrBoost.Count; i++)
        {
            defaultMaterials.Add(mrBoost[i].material);
        }

    }

    private void Update()
    {
        if(!IsServerInitialized) return;

		if (isRespawning)
        {
            if (timer < timerMax)
            {
                timer += 1f;
            }
            else
            {
                Respawn();
				timer = 0f;
				isRespawning = false;
			}
        }
        
    }

    [ObserversRpc]
    void Respawn()
    {
		for (int i = 0; i < mrBoost.Count; i++)
		{
			mrBoost[i].material = defaultMaterials[i];
		}
		coll.enabled = true;
	}

    [ServerRpc(RequireOwnership = false)]
	public void FadeAndRespawn()
    {
        if (!isTemporary) return;
        Despawn();
		isRespawning = true;
    }

    [ObserversRpc]
    void Despawn()
    {
		for (int i = 0; i < mrBoost.Count; i++)
		{
			mrBoost[i].material = matFade;
		}
		coll.enabled = false;
	}
}
