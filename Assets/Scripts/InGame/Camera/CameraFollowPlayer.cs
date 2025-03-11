using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform target; // Reference to the player object
	[SerializeField] private Vector3 offset;   // Offset from the player
	[SerializeField] private Vector3 rotationOffset;
	[SerializeField] private float smoothSpeed = 0.125f; // The smoothness of camera movement

    //Culling problem
    private CommandBuffer commandBuffer;

   /* void Start()
    {
        commandBuffer = new CommandBuffer();
        commandBuffer.SetInvertCulling(false);
        Camera.main.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, commandBuffer);
        Invoke("BugCulling", 1f);
    }

    void BugCulling()
    {
        Camera.main.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, commandBuffer);
    }
    void OnDestroy()
    {
        if (Camera.main != null && commandBuffer != null)
        {
            Camera.main.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, commandBuffer);
        }
    }
   */
    void LateUpdate()
    {
        

        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;


            // Calculate the desired look direction of the camera
            Vector3 lookDirection = (target.position + rotationOffset) - transform.position;

            // Make the camera look at the player
            Quaternion desiredRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothSpeed);
            transform.rotation = smoothedRotation;
        }
    }
}
