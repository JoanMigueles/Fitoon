using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class CannonBarrage : NetworkBehaviour
{
    public float timeInterval = 1f;
    public int projectileAmount = 2;

    private Shoot[] cannons;
    private float time;

	public override void OnStartNetwork()
	{
        if(!IsServerInitialized)
        {
			enabled = false;
            return;
		}
		time = 0;
        cannons = GetComponentsInChildren<Shoot>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time > timeInterval) {
            // Spawn two random cannons
            List<int> availableCannons = new List<int>();
            for (int i = 0; i < cannons.Length; i++) {
                availableCannons.Add(i);
            }

            for (int i = 0; i < projectileAmount || availableCannons.Count == 0; i++) {
                int randomAvailableCannonIndex = Random.Range(0, availableCannons.Count);
                int randomCannonIndex = availableCannons[randomAvailableCannonIndex];
                availableCannons.RemoveAt(randomAvailableCannonIndex);
                cannons[randomCannonIndex].ShootProjectile();
            }

            time = 0;
        }
    }
}
