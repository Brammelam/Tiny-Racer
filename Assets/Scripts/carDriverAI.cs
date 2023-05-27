using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carDriverAI : MonoBehaviour
{
	
	[SerializeField] private Transform targetPositionTransform;

	public carController2 carDriver;
	private Vector3 targetPoisition;
	public float angleToDir;
	public float dot;

	private void Awake() {
		carDriver = GetComponent<carController2>();
	}

	private void FixedUpdate() {
		SetTargetPosition(targetPositionTransform.position);
		float forwardAmount = 0f;
		float turnAmount = 0f;
		
		float reachedTargetDistance = 5f;
		float distanceToTarget = Vector3.Distance(transform.position, targetPoisition);
			if (distanceToTarget > reachedTargetDistance) {
				Vector3 dirToMovePosition = (targetPoisition - transform.position).normalized;
				dot = Vector3.Dot(transform.forward, dirToMovePosition);
				
				if (dot > 0) {
					forwardAmount = 1f;
					
					float stoppingDistance = 10f;
					float stoppingSpeed = 50f;
					if (distanceToTarget < stoppingDistance && carDriver.GetSpeed() > stoppingSpeed) {
						forwardAmount = -1f;
					}
					
				} else {
					forwardAmount = -1f;
					
				/*
					forwardAmount = -1f;
					float reverseDistance = 25f;
					if (distanceToTarget > reverseDistance) {
						forwardAmount = 1f;
					} else {
						forwardAmount = -1f;
					}
					*/
				}

				angleToDir = Vector3.SignedAngle(transform.forward, dirToMovePosition, Vector3.up);
				/*
				if(angleToDir == 0 || (angleToDir < 5f && angleToDir > -5f)) {
					turnAmount = 0;
				}
				
				else if (angleToDir > 5f && dot > 0) {
					turnAmount = 1f;
				}	else if (angleToDir < -5f && dot > 0){
					turnAmount = -1f;
				}
				*/
				if (angleToDir>0 ) turnAmount = 1f;
				else turnAmount = -1f;
				

			}
			else {
			// Reached target
				if(carDriver.GetSpeed() > 0f) {
					forwardAmount = -1f;
				} else {
					forwardAmount = 0f;

				}
				turnAmount = 0f;
			}


		carDriver.SetInputs(forwardAmount, turnAmount);
	}

	public void SetTargetPosition(Vector3 targetPoisition){
		this.targetPoisition = targetPoisition;
	}
}
