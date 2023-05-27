using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationTracker : MonoBehaviour
{

    //Holds the previous frames rotation
    public Transform _car;

    //References to the relevent axis angle variables

    private Vector3 forwardVector = Vector3.zero;
    private Vector3 lastForwardVector = Vector3.zero;
    public float angle = 0f;
    [SerializeField]
    public float tippingAngle = 10f;
    float t = 0f;
    private float timer = 0.4f;
    public float currentAngle = 0f;

    public bool wait, firstTime;
    public bool turningleft, turningright, tippingBack = false;
    public float turningangle;
    public float targetangle;
    [SerializeField]
    public float averageAngle;
    delegate float TipCar(float tipAngle);
    [SerializeField]
    bool tipcarbool, tipcarboolback = false;

    private const int BUFFER_SIZE = 30;

    private int frameCount = 0;
    private Vector3[] forwardVectors = new Vector3[BUFFER_SIZE];

    public void Start()
    {
        wait = true;
        firstTime = true;
        forwardVector = transform.forward;
        lastForwardVector = transform.forward;
        targetangle = turningangle = 0f;
    }

    public void TipCarFunction()
    {
        turningangle = Mathf.Lerp(0f, targetangle, t / timer);
        t += Time.deltaTime;
    }

    public void TipCarBackFunction()
    {
        turningangle = Mathf.Lerp(targetangle, 0f, t / timer);
        t += Time.deltaTime;
    }
    public void Update()
    {
        if (!firstTime)
        {
            // Add the current forward vector to the buffer
            forwardVectors[frameCount % BUFFER_SIZE] = transform.forward;

            // Compute the average forward vector over the last 10 frames
            Vector3 avgForwardVector = Vector3.zero;
            for (int i = 0; i < BUFFER_SIZE; i++)
            {
                avgForwardVector += forwardVectors[i];
            }
            avgForwardVector /= BUFFER_SIZE;

            // Compute the cross product of the current forward vector and the average forward vector
            Vector3 crossProduct = Vector3.Cross(transform.forward, avgForwardVector);

            averageAngle = Vector3.Angle(transform.forward, avgForwardVector);

            // Turn left
            if (crossProduct.y > 0)
            {
                averageAngle = Vector3.Angle(transform.forward, avgForwardVector);

            }
            // or turn right
            else if (crossProduct.y < 0)
            {
                averageAngle = -Vector3.Angle(transform.forward, avgForwardVector);

            }

            // Car starts tipping if average angle exceeds treshold
            if (Mathf.Abs(averageAngle) > tippingAngle)
            {
                if (crossProduct.y > 0)
                    targetangle = 2f;
                else
                    targetangle = -2f;
                tipcarbool = true;
                t = 0f;
            }

            // Start tilting the car out
            if (tipcarbool)
                TipCarFunction();

            // Stop tipping if the maximum angle is reached
            if ((Mathf.Abs(turningangle) >= Mathf.Abs(targetangle)) && tipcarbool)
            {
                tipcarbool = false;
                tipcarboolback = true;
                t = 0f;
            }

            if (tipcarboolback)
                if (Mathf.Abs(turningangle) > 0.1f)
                    TipCarBackFunction();
                else
                {
                    targetangle = 0f;
                    turningangle = 0f;
                    tipcarboolback = false;
                }

            
            frameCount++;
        }
    }
    public float GetAverageAngle()
    {
        return averageAngle;
    }



 


    public void FinishSetup()
    {
        if (firstTime)
        {
            forwardVector = transform.forward;
            lastForwardVector = transform.forward;
            firstTime = false;
        }
    }
}