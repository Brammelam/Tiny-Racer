using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newController : MonoBehaviour {

    public float speed;
    private float speedMax = 80f;
    private float speedMin = 0f;
    [SerializeField]
    float acceleration = 0f;
    private float brakeSpeed = 1500f;
    private float reverseSpeed = 30f;
    private float idleSlowdown = 1000f;

    public float turnSpeed;
    private float turnSpeedMax = 500f;
    private float turnSpeedAcceleration = 500f;
    private float turnIdleSlowdown = 1000f;

    private float forwardAmount;
    public float turnAmount;

    public float flipValue = 3.9f;
    private bool isFlipped = false;

    public Rigidbody carRigidbody;

    private void Awake() {
        carRigidbody = GetComponent<Rigidbody>();
    }


    private void FlipCar() {
        Vector3 flipSpeed = -carRigidbody.velocity;
        
        carRigidbody.AddForce(Vector3.up * 5, ForceMode.VelocityChange);
        carRigidbody.AddForce(-carRigidbody.velocity*0.5f, ForceMode.VelocityChange);
        
        carRigidbody.AddTorque(flipSpeed.x, flipSpeed.y, flipSpeed.z, ForceMode.VelocityChange);

        carRigidbody.useGravity = true;
        isFlipped = true;
    }

    public void SetInputs(float forwardAmount, float turnAmount) {
        this.forwardAmount = forwardAmount;
        this.turnAmount = turnAmount;
    }

    public void ClearTurnSpeed() {
        turnSpeed = 0f;
    }

    public float GetSpeed() {
        return speed;
    }

    public void SetSpeedMax(float speedMax) {
        this.speedMax = speedMax;
    }

    public void SetTurnSpeedMax(float turnSpeedMax) {
        this.turnSpeedMax = turnSpeedMax;
    }

    public void SetTurnSpeedAcceleration(float turnSpeedAcceleration) {
        this.turnSpeedAcceleration = turnSpeedAcceleration;
    }

    public void StopCompletely() {
        speed = 0f;
        turnSpeed = 0f;
    }

}
