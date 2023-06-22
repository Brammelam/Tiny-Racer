using System;
using UnityEngine;

public class speedTracker : MonoBehaviour
{
	public float CurrentSpeed { get; private set; }
	public event Action<float> SpeedChanged;
	public Vector3 carPosition = Vector3.zero;
	public Vector3 lastPosition = Vector3.zero;
	[SerializeField]
	public float speed = 0;
	public Vector3 direction = Vector3.zero;
	public void FixedUpdate()
	{
		carPosition = transform.position;

		speed = (carPosition - lastPosition).magnitude;
		direction = (carPosition - lastPosition);
		lastPosition = transform.position;

	}
}
