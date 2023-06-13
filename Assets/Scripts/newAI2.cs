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

    private float timer = 0f;
    private float interval = 0.05f;

    [SerializeField] private float mult;
    public void Awake()
    {
        check ??= checkShit.FindObjectOfType<checkShit>();

    }

    public void Start()
    {
        smoothTime = 0.15f;
        particlePool = gameObject.AddComponent<ParticlePool>();

    }

    public void Update()
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

        if (speed > 0) particlePool.EnableNextParticle(speed);

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