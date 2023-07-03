using PathCreation;
using UnityEngine;

public class multiplayerAI : MonoBehaviour
{
    public GameObject cop;
    public PathCreator pathCreator;
    private SetCameraDistance cameraDistanceScript;
    private Rect inputRegion;

    public float smoothTime;

    public Vector3 velocity = Vector3.zero;
    public Vector3 copPosition = Vector3.zero;
    private Vector3 previousPosition = Vector3.zero;
    public Quaternion copRotation = Quaternion.Euler(0, 0, 0);
    [SerializeField]
    private Transform driftPoint;
    public float distanceTravelled;
    public float speed = 0f;

    public multiplayerScript multiplayer;
    [SerializeField] private GameObject lobby;
    public bool touching = false;
    private ParticlePool particlePool;

    private int interval = 0;

    [SerializeField] private float mult;
    public void Awake()
    {
        multiplayer ??= multiplayerScript.FindObjectOfType<multiplayerScript>();
    }

    public void Start()
    {
        lobby = GameObject.FindGameObjectWithTag("lobby");
        cameraDistanceScript = FindObjectOfType<SetCameraDistance>();
        smoothTime = 0.15f;
        particlePool = gameObject.AddComponent<ParticlePool>();

        float screenWidth = Screen.width;
        inputRegion = new Rect(0, Screen.height - 200, screenWidth, 200);
    }

    public void Update()
    {
        pathCreator ??= PathCreator.FindObjectOfType<PathCreator>();
        multiplayer ??= multiplayerScript.FindObjectOfType<multiplayerScript>();
        if (multiplayer == null) return;
        if (lobby.activeSelf) return;

        distanceTravelled += speed * Time.deltaTime;
        if (Input.GetKey("space")) touching = true;

        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            // Get the touch or mouse position
            Vector2 inputPosition = Input.mousePosition;

            // Check if the player is touching the excluded zone
            if (!inputRegion.Contains(inputPosition)) touching = true;
        }

        else touching = false;

        if (!multiplayer.tipped && touching) speed = Mathf.Min(speed + 1.5f, 100f);
        else speed = Mathf.Max(speed - 1.5f, 0f);

        interval++;
        if (speed > 0 && interval % 2 == 0) particlePool.EnableNextParticle(speed);

        if (multiplayer != null && multiplayer.rt != null && pathCreator != null && pathCreator.path != null && !multiplayer.tipped)
        {
            Vector3 position = pathCreator.path.GetPointAtDistance(distanceTravelled);
            Quaternion rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled) * Quaternion.Euler(0f, 0f, 90f);

            // Set the new position and rotation of the car
            transform.SetPositionAndRotation(position, rotation);

            // Rotate the car around the drift point first
            transform.RotateAround(driftPoint.position, Vector3.up, -multiplayer.rt.turningangle);

            if (speed > 1f)
                multiplayer.rt.DriftCarBackFunction();

            multiplayer.rt?.FinishSetup();
        }
        multiplayer?.cam.UpdateCamera();
    }
}