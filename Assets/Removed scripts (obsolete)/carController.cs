using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController : MonoBehaviour
{
	void Start() {

		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = com;
	}
	/*
	public void GetInput() {
		m_horizontalInput = Input.GetAxis("Horizontal");
		m_verticalInput = Input.GetAxis("Vertical");
	}*/

	public void SetInputs(float forwardAmount, float turnAmount)
	{
		m_horizontalInput = turnAmount;
		m_verticalInput = forwardAmount;
	}

	private void Steer() {
		m_steeringAngle = maxSteerAngle * m_horizontalInput;
		frontDriverW.steerAngle = m_steeringAngle;
		frontPassengerW.steerAngle = m_steeringAngle;
	}

	private void Accelerate() {
		frontDriverW.motorTorque = m_verticalInput * motorForce;
		frontPassengerW.motorTorque = m_verticalInput * motorForce;
		rearW.motorTorque = m_verticalInput * motorForce;
	}

	private void UpdateWheelPoses(){
		UpdateWheelPose(frontDriverW, frontDriverT);
		UpdateWheelPose(frontPassengerW, frontPassengerT);
		UpdateWheelPose(rearW, rearT);
	}

	private void UpdateWheelPose(WheelCollider _collider, Transform _transform){
		Vector3 _pos = _transform.position;
		Quaternion _quat = _transform.rotation;

		_collider.GetWorldPose(out _pos, out _quat);

		_transform.position = _pos;
		_transform.rotation = _quat;
	}

	private void addLift(){
		if (rb != null)
		{
         float lift = liftCoefficient * rb.velocity.sqrMagnitude;
         rb.AddForceAtPosition(lift * transform.up, transform.position);
		 }
	}
	
	private void FixedUpdate() {
		//GetInput();
		//SetInputs(float forwardAmount, float turnAmount);
		Steer();
		Accelerate();
		UpdateWheelPoses();
		addLift();
		speed = rb.velocity;
		speedMag = speed.magnitude;
	}

	private float m_horizontalInput;
	private float m_verticalInput;
	private float m_steeringAngle;

	public WheelCollider frontDriverW, frontPassengerW;
	public WheelCollider rearW;
	public Transform frontDriverT, frontPassengerT;
	public Transform rearT;
	public float maxSteerAngle = 30;
	public float motorForce = 50;
	public Vector3 speed;
	public float speedMag;

	public Vector3 com;
	public Rigidbody rb;
	public float liftCoefficient;

}
