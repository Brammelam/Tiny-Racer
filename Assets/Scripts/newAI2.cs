using PathCreation;
using UnityEngine;

public class newAI2 : MonoBehaviour
{
    public GameObject target;
    public Rigidbody rb;
    public Rigidbody rb2;
    public GameObject cop;
    public GameObject deadCop;
    public PathCreator pathCreator;

    //public PathCreator pathCreator;
    public float smoothTime;

    public Vector3 velocity = Vector3.zero;
    public Vector3 copPosition = Vector3.zero;
    public Quaternion copRotation = Quaternion.Euler(0, 0, 0);
    public float distanceTravelled;
    public float speed = 0f;
    public rotationTracker rotationTracker;
    public speedTracker speedTracker;
    public checkShit check;
    public bool touching = false;

    [SerializeField] private float mult;
    public void Awake()
    {
        check ??= checkShit.FindObjectOfType<checkShit>();
    }

    public void Start()
    {
        smoothTime = 0.15f;
    }

    public void FixedUpdate()
    {
        pathCreator ??= PathCreator.FindObjectOfType<PathCreator>();
        check ??= checkShit.FindObjectOfType<checkShit>();
        if (check == null) return;
        distanceTravelled += speed * Time.deltaTime;

        if (Input.touchCount > 0 || Input.GetMouseButton(0) || Input.GetKey("space"))
        {
            touching = true;
        }
        else touching = false;

        if (!check.tipped && touching)
        {
            speed = Mathf.Min(speed + 1.5f, 100f);
        }
        else
        {
            speed = Mathf.Max(speed - 1.5f, 0f);
        }

        if (check != null && check.rt != null && pathCreator != null && pathCreator.path != null)
        {
            float mult = Mathf.Clamp((check.currentSpeed / 100) * 4f, 0f, 4f);
            Vector3 position = pathCreator.path.GetPointAtDistance(distanceTravelled);
            Quaternion rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled) * Quaternion.Euler(check.rt.turningangle * mult, 0f, 90f - (check.rt.turningangle * mult * (3 / 4)));
            transform.SetPositionAndRotation(position, rotation);
            check.rt?.FinishSetup();
        }
        check?.cam.UpdateCamera();
    }
}