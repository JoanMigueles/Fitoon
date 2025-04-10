using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

[Obsolete]
public class PlayerCameraHolder : NetworkBehaviour
{
    public GameObject cam;
    public Vector3 offset;
    public float timeToReachTarget = 1f;

    private Vector3 startPos;
    private float relativeHeight;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        /*if (IsOwner) {
            cam.SetActive(true);
            Camera.main.enabled = false;
        }
        startPos = cam.transform.position;
        relativeHeight = cam.transform.localPosition.y;*/
    }


    void FixedUpdate()
    {
        //cam.transform.position = startPos + transform.position + offset;

        Vector3 smoothPos = Vector3.SmoothDamp(cam.transform.position, transform.position + offset + new Vector3(0, relativeHeight, 0) + (transform.forward * startPos.z), ref velocity, timeToReachTarget);
        cam.transform.position = smoothPos;
        cam.transform.LookAt(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z));
    }
}
