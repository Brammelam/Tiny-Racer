using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class CarAITargetMouse : MonoBehaviour
{
	
	[SerializeField] public Transform targetTransform;
	public PathCreator pathCreator;
	public float speed = 5f;
	float distanceTravelled;
	private bool isFollowing = false;
	[SerializeField] private Camera screenCamera;
	//[SerializeField] private GameHandler gh;
	


	private void Update() {
		distanceTravelled += speed * Time.deltaTime;
		targetTransform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);

	var mousePos = Input.mousePosition;
		if (isFollowing) {
			Ray ray;
			RaycastHit hit;

			ray = screenCamera.ScreenPointToRay(mousePos);
			if(Physics.Raycast(ray, out hit) && hit.collider.gameObject.CompareTag("Terrain"))
			{
				targetTransform.position = hit.point;
			}
		}

		if (Input.GetMouseButtonDown(0)) {
			isFollowing = !isFollowing;
		}
	}



	

}
