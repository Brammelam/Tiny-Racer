using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController2 : MonoBehaviour {

    #region Fields
    public float speed;
    private float speedMax = 60f;
    private float speedMin = 0f;
    private float acceleration = 200f;
    private float brakeSpeed = 1000f;
    private float reverseSpeed = 30f;
    private float idleSlowdown = 500f;

    public float turnSpeed;
    private float turnSpeedMax = 500f;
    private float turnSpeedAcceleration = 500f;
    private float turnIdleSlowdown = 500f;

    private float forwardAmount;
    private float turnAmount;

    public Rigidbody carRigidbody;
    [SerializeField] Vector3 vectorSpeed;
    [SerializeField] float torque ;
    [SerializeField] float flipValue ;
    private bool isFlipped = false;
    #endregion

    private void Awake() {
        carRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        
            if (!isFlipped){
                if (forwardAmount > 0) {
                    // Accelerating
                    speed += forwardAmount * acceleration * Time.deltaTime;
                }
                if (forwardAmount < 0) {
                    if (speed > 0) {
                        // Braking
                        speed += forwardAmount * brakeSpeed * Time.deltaTime;
                    } else {
                        // Reversing
                        speed += forwardAmount * reverseSpeed * Time.deltaTime;
                    }
                }

                if (forwardAmount == 0) {
                    // Not accelerating or braking
                    if (speed > 0) {
                        speed -= idleSlowdown * Time.deltaTime;
                    }
                    if (speed < 0) {
                        speed += idleSlowdown * Time.deltaTime;
                    }
                }

                speed = Mathf.Clamp(speed, speedMin, speedMax);

                carRigidbody.velocity = transform.forward * speed;

                if (speed < 0) {
                    // Going backwards, invert wheels
                    //turnAmount = turnAmount * -1f;
                }

                if (turnAmount > 0 || turnAmount < 0) {
                    // Turning
                    if ((turnSpeed > 0 && turnAmount < 0) || (turnSpeed < 0 && turnAmount > 0)) {
                        // Changing turn direction
                        float minTurnAmount = 20f;
                        turnSpeed = turnAmount * minTurnAmount;
                    }
                    turnSpeed += turnAmount * turnSpeedAcceleration * Time.deltaTime;
                } else {
                    // Not turning
                    if (turnSpeed > 0) {
                        turnSpeed -= turnIdleSlowdown * Time.deltaTime;
                    }
                    if (turnSpeed < 0) {
                        turnSpeed += turnIdleSlowdown * Time.deltaTime;
                    }
                    if (turnSpeed > -5f && turnSpeed < +5f) {
                        // Stop rotating
                        turnSpeed = 0f;
                    }
                }

                float speedNormalized = speed / speedMax;
                float invertSpeedNormalized = Mathf.Clamp(1 - speedNormalized, .75f, 1f);

                turnSpeed = Mathf.Clamp(turnSpeed, -turnSpeedMax, turnSpeedMax);

                carRigidbody.angularVelocity = new Vector3(0, turnSpeed * (invertSpeedNormalized * 1f) * Mathf.Deg2Rad, 0);

                if (transform.eulerAngles.x > 2 || transform.eulerAngles.x < -2 || transform.eulerAngles.z > 2 || transform.eulerAngles.z < -2) {
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                }

                vectorSpeed = carRigidbody.angularVelocity;
        
                if ((carRigidbody.angularVelocity.y > flipValue || carRigidbody.angularVelocity.y < - flipValue) && !isFlipped) 
                {                
                    if (speed > 79f) 
                    {
                        //FlipCar();
                        Debug.Log("FLIPPED");
                    }
                }
            }
    }
      
    /*
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == GameHandler.SOLID_OBJECTS_LAYER) {
            speed = Mathf.Clamp(speed, 0f, 20f);
        }
    }
    */
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
