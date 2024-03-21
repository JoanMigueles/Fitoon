using UnityEngine;

public class Fan : MonoBehaviour
{
    public bool isInverted = false;

    public float windForce = 10f;       // Fuerza vertical

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {

                //if (other.GetComponent<PlayerController>() != null) other.GetComponent<PlayerController>().LockMovement(true);
                //if (other.GetComponent<BotController>() != null) other.GetComponent<BotController>().LockMovement(true);

                //playerRigidbody.velocity = Vector3.zero;
                //playerRigidbody.angularVelocity = Vector3.zero;

                Vector3 jumpDirection;

                if (isInverted)
                {
                    jumpDirection = -transform.up * windForce; //Calcular la direcci�n combinada hacia el Player y hacia arriba
                }
                else
                {
                    playerRigidbody.rotation = Quaternion.identity;
                    jumpDirection = transform.up * windForce; //Calcular la direcci�n combinada hacia adelante y hacia arriba
                }

                // Aplicar la fuerza combinada
                playerRigidbody.AddForce(jumpDirection, ForceMode.Impulse);
            }
        }
    }
}
