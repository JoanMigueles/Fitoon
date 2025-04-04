using FishNet.Object;
using UnityEngine;

public class CannonShip : NetworkBehaviour
{
    public float timeInterval = 1f;

    private Shoot cannon;
    private float time;

	// Start is called before the first frame update
	public override void OnStartNetwork()
    {
        if(!IsServerInitialized)
        {
			enabled = false;
			return;
		}
		cannon = transform.GetChild(0).GetChild(0).GetComponent<Shoot>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time > timeInterval) {
            cannon.ShootProjectile();

            time = 0;
        }
    }
}
