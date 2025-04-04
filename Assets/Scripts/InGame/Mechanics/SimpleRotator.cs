using FishNet.Component.Transforming;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
public class SimpleRotator : NetworkBehaviour
{
    public bool reset;

    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 30f;
    public bool randomizeDirection = false;
    public bool randomizeSpeed = false;
    public float randomSpeed = 5f;

    private int rndDir;
    private float rndSpd;
    private Vector3 pos;
    private Quaternion rot;

    private void Start()
    {
        if(!IsServerInitialized)
        {
			enabled = false;
			return;
		}
        pos = transform.position;
        rot = transform.rotation;

        if (randomizeDirection) rndDir = (int) ((Random.Range(0, 2) - 0.5) * 2);
        if (randomizeSpeed) rndSpd = Random.Range(-randomSpeed, randomSpeed);
    }

    private void Update()
    {
        if (reset)
        {
            Reset();
            reset = false;
        }

        //if (randomizeDirection) transform.Rotate(rotationAxis * (rotationSpeed + rndSpd * System.Convert.ToInt32(randomizeSpeed)) * rndDir * Time.deltaTime);
        //else transform.Rotate(rotationAxis * (rotationSpeed + rndSpd * System.Convert.ToInt32(randomizeSpeed)) * Time.deltaTime);

        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }

    public void Reset()
    {
        transform.SetPositionAndRotation(pos, rot);
    }

}
