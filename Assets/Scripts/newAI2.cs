using PathCreation;
using UnityEngine;

public class newAI2 : MonoBehaviour
{
    //public GameObject target;
    //public Rigidbody rb;
    //public Rigidbody rb2;
    public GameObject cop;
    public GameObject deadCop;
    public PathCreator pathCreator;
    private SetCameraDistance cameraDistanceScript;
    private Rect inputRegion;
    //public PathCreator pathCreator;
    public float smoothTime;

    public Vector3 velocity = Vector3.zero;
    public Vector3 copPosition = Vector3.zero;
    private Vector3 previousPosition = Vector3.zero;
    public Quaternion copRotation = Quaternion.Euler(0, 0, 0);
    [SerializeField]
    private Transform driftPoint;
    public float distanceTravelled;
    public float speed = 0f;

    public checkShit check;
    public bool touching = false;
    private Transform rotationPoint;
    private ParticlePool particlePool;

    private int interval = 0;

    [SerializeField] private float mult;
    public void Awake()
    {
        check ??= checkShit.FindObjectOfType<checkShit>();

    }

    public void Start()
    {
        cameraDistanceScript = FindObjectOfType<SetCameraDistance>();
        smoothTime = 0.15f;
        particlePool = gameObject.AddComponent<ParticlePool>();

        float screenWidth = Screen.width;
        inputRegion = new Rect(0, Screen.height - 200, screenWidth, 200);

    }

    public void Update()
    {
        interval++;
        pathCreator ??= PathCreator.FindObjectOfType<PathCreator>();
        check ??= checkShit.FindObjectOfType<checkShit>();
        if (check == null) return;                

        distanceTravelled += speed * Time.deltaTime;
        if (Input.GetKey("space")) touching = true;

        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            // Get the touch or mouse position
            Vector2 inputPosition = Input.mousePosition;

            // Check if the player is touching the excluded zone
            if (!inputRegion.Contains(inputPosition))
            {
                touching = true;
            }
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

        if (speed > 0 && interval % 2 == 0) particlePool.EnableNextParticle(speed);

        if (check != null && check.rt != null && pathCreator != null && pathCreator.path != null)
        {
            
            Vector3 position = pathCreator.path.GetPointAtDistance(distanceTravelled);
            Quaternion rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled) * Quaternion.Euler(0f, 0f, 90f);

            // Set the new position and rotation of the car
            transform.SetPositionAndRotation(position, rotation);

            // Rotate the car around the drift point first
            transform.RotateAround(driftPoint.position, Vector3.up, -check.rt.turningangle);

            if (speed > 1f)
                check.rt.DriftCarBackFunction();

            check.rt?.FinishSetup();
        }
        check?.cam.UpdateCamera();
    }
}