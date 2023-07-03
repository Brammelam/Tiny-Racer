using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sheepScript : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float resetDistance = 60f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        moveSpeed = Random.Range(8, 12);
    }

    private void Update()
    {
        // Move the object forward
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // Check if the object has reached the reset distance
        if (Mathf.Abs(Vector3.Distance(transform.position, initialPosition)) >= resetDistance)
        {
            // Reset the object's position and rotate it 180 degrees
            ResetPosition();
        }
    }

    private void ResetPosition()
    {
        // Set the object's position back to the initial position
        transform.position = initialPosition;

    }
}
