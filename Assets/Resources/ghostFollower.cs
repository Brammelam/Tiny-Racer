
using UnityEngine;
using PathCreation;

public class ghostFollower : MonoBehaviour
{



	public PathCreator pathCreator;
	public checkShit check;

	public float distanceTravelled;
	public float speed = 0f;


	public void Update()
	{
		if (pathCreator == null) pathCreator = PathCreator.FindObjectOfType<PathCreator>();
		if (check == null) check = checkShit.FindObjectOfType<checkShit>();

		/*
		transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
		transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
		transform.rotation *= Quaternion.Euler(0, 0, 90f);
		*/
	}
}
