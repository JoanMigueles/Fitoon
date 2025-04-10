using FishNet.Component.Transforming;
using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to handle the projectile behavior.
/// </summary>
[RequireComponent(typeof(NetworkTransform))]
public class Projectile : NetworkBehaviour
{
    public List<string> explodeOnTags = new List<string>();
    public GameObject explosionPrefab;

	public override void OnStartNetwork()
	{
		if(!IsServerInitialized)
        {
			enabled = false;
		}
	}

    [ObserversRpc]
	public void Explode()
    {
        // TODO: generar sistema de partículas
        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
        Destroy(explosion, 0.6f);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (explodeOnTags.Contains(other.gameObject.tag)) {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Jumpad>().Bounce(collision.gameObject);
        if (explodeOnTags.Contains(collision.gameObject.tag)) {
            Explode();
        }
    }
}
