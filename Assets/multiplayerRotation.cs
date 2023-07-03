using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class multiplayerRotation : MonoBehaviour
{

    //Holds the previous frames rotation
    public Transform _car;

    //References to the relevent axis angle variables
    private Vector3 forwardVector = Vector3.zero;
    private Vector3 lastForwardVector = Vector3.zero;

    [SerializeField]
    public float driftingAngle = 25f;


    float t = 0f;

    private float stepSize;


    public bool wait, firstTime;
    public bool turningleft, turningright, tippingBack = false;
    [SerializeField]
    public float turningangle;
    public float targetangle;


    //delegate float TipCar(float tipAngle);

    private const int BUFFER_SIZE = 20;

    private int frameCount = 0;
    private Vector3[] forwardVectors = new Vector3[BUFFER_SIZE];

    [SerializeField]
    public float averageAngle;
    [SerializeField]
    private float driftCooldown = 2f;
    private float driftCooldownTimer = 0f;

    private multiplayerAI playerValues;

    private float delayDuration;
    public bool isDelaying;
    private float delayTimer;

    public enum CarState
    {
        Normal,
        Drifting,
        Returning
    }

    [SerializeField]
    private CarState currentState = CarState.Normal;

    public void Start()
    {
        delayDuration = 1f;
        isDelaying = false;
        delayTimer = 0f;

        wait = true;
        firstTime = true;
        targetangle = turningangle = 0f;
        stepSize = 1f / (30f);
        playerValues = GetComponent<multiplayerAI>();

    }

    public void DriftCarFunction()
    {
        turningangle = Mathf.Lerp(0f, targetangle, t);
        t += stepSize;
    }

    public void DriftCarBackFunction()
    {
        turningangle = Mathf.Lerp(targetangle, 0f, t);
        t += stepSize;
    }
    public void Update()
    {
        if (!firstTime)
        {
            if (isDelaying)
            {
                delayTimer += Time.deltaTime;
                averageAngle = 0f;

                if (delayTimer >= delayDuration)
                {
                    delayTimer = 0f;
                    isDelaying = false;

                }
            }
            else
            {
                // Add the current forward vector to the buffer
                forwardVectors[frameCount % BUFFER_SIZE] = transform.forward;

                // Compute the average forward vector over the last BUFFER_SIZE frames
                Vector3 avgForwardVector = Vector3.zero;
                for (int i = 0; i < BUFFER_SIZE; i++)
                {
                    avgForwardVector += forwardVectors[i];
                }
                avgForwardVector /= BUFFER_SIZE;
                //avgForwardVector -= transform.right * turningangle;

                // Compute the cross product of the current forward vector and the average forward vector
                Vector3 crossProduct = Vector3.Cross(transform.forward, avgForwardVector);
                averageAngle = Vector3.Angle(transform.forward, avgForwardVector);

                switch (currentState)
                {
                    case CarState.Normal:
                        // Start drifting if average angle exceeds the drifting angle threshold
                        if (Mathf.Abs(averageAngle) > driftingAngle && driftCooldownTimer <= 0f && playerValues.speed > 50f)
                        {
                            if (crossProduct.y > 0)
                                targetangle = 10f + (25f * (playerValues.speed / 100));
                            else
                                targetangle = -10f - (25f * (playerValues.speed / 100));

                            currentState = CarState.Drifting;
                            t = 0f;

                            // Start the cooldown timer
                            driftCooldownTimer = driftCooldown;
                        }
                        break;

                    case CarState.Drifting:
                        DriftCarFunction();

                        // Check if the maximum angle is reached
                        if (Mathf.Abs(turningangle) >= Mathf.Abs(targetangle))
                        {
                            currentState = CarState.Returning;
                            t = 0f;
                        }
                        break;

                    case CarState.Returning:

                        // Check if the car has returned to the starting position
                        if (Mathf.Abs(turningangle) <= 0.2f)
                        {
                            currentState = CarState.Normal;
                            targetangle = 0f;
                            turningangle = 0f;
                        }
                        break;
                }
            }

            if (driftCooldownTimer > 0f)
                driftCooldownTimer -= Time.deltaTime;

            frameCount++;
        }
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

    public void ClearForwardVectorBuffer()
    {
        for (int i = 0; i < BUFFER_SIZE; i++)
        {
            forwardVectors[i] = Vector3.zero;
        }
    }
}