using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Translator : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public Axis movementAxis = Axis.Y;  // Eje de movimiento por defecto

    public float speed = 1f;
    public float height = 2f;
    [Range(0f, 3f)]
    public float offset = 0f;

    private Rigidbody rB;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        rB = GetComponent<Rigidbody>();

        switch (movementAxis)
        {
            case Axis.X:
                startPosition = transform.position + new Vector3(height, 0, 0);
                break;
            case Axis.Y:
                startPosition = transform.position + new Vector3(0, height, 0);
                break;
            case Axis.Z:
                startPosition = transform.position + new Vector3(0, 0, height);
                break;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = Vector3.zero;

        switch (movementAxis)
        {
            case Axis.X:
                direction = new Vector3(Mathf.Sin(offset + Time.time * speed), 0, 0);
                break;
            case Axis.Y:
                direction = new Vector3(0, Mathf.Sin(offset + Time.time * speed), 0);
                break;
            case Axis.Z:
                direction = new Vector3(0, 0, Mathf.Sin(offset + Time.time * speed));
                break;
        }

        rB.MovePosition(startPosition + direction * height);
    }

    public void Reset()
    {
        transform.position = startPosition;
    }
}
