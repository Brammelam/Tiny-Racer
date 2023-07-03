using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathCreation;

public class taxiCamera : MonoBehaviour
{
    public Transform objectToFollow;
    public float offset = 0.12f;
    public float smoothTime = 0.04f;
    public Vector3 forwardCop = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public TaxiGameHandler check;
    public PathCreator pathCreator;

    public float cameraDistance;

    private void Awake()
    {
        check = GameObject.FindObjectOfType<TaxiGameHandler>();
    }

    public void Start()
    {
        float cameraDistancePreference = PlayerPrefs.GetFloat("cameradistance", 30f);
        cameraDistance = cameraDistancePreference;
        smoothTime = 0.08f;
        this.gameObject.GetComponent<Camera>().orthographicSize = cameraDistance;
    }

    public void UpdateCameraDistance(float _distance)
    {
        this.gameObject.GetComponent<Camera>().orthographicSize = _distance;
        PlayerPrefs.SetFloat("cameradistance", _distance);
        PlayerPrefs.Save();
    }

    public void Update()
    {
        // Zoom in when car is tipped
        if (check.tipped && this.gameObject.GetComponent<Camera>().orthographicSize > 20f)
            this.gameObject.GetComponent<Camera>().orthographicSize = this.gameObject.GetComponent<Camera>().orthographicSize - 0.01f;

    }

    public void UpdateCamera()
    {
        if (pathCreator == null) pathCreator = PathCreator.FindObjectOfType<PathCreator>();

        if (objectToFollow == null)
            objectToFollow = GameObject.FindGameObjectWithTag("target").GetComponent<Transform>();

        float t = 0f;
        if (check != null)
            t = check.player.speed / 100f;
        if (!check.tipped && objectToFollow != null)
        {
            // Zoom out slightly, relative to car speed  This makes me throw up, disabled
            //this.gameObject.GetComponent<Camera>().orthographicSize = cameraDistance + Mathf.SmoothStep(0, 3f, t); 

            // The camera follows a point ahead of the car determined by an offset (float value) to see the road ahead
            if (offset > 10f)
                offset = 0.5f * check.player.speed;
            else offset = 10f;

            Vector3 _followMe = pathCreator.path.GetPointAtDistance(check.player.distanceTravelled + (offset));
            transform.position = Vector3.SmoothDamp(transform.position, _followMe, ref velocity, smoothTime);
        }
    }
}