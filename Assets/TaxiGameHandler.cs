using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TaxiGameHandler : MonoBehaviour
{
    public List<GameObject> cars;

    [SerializeField]
    private List<float> playerRecord;
    public List<float> saveRecord;

    public GameObject cop;

    public Rigidbody rb;
    public Vector3 copPosition = Vector3.zero;
    public Vector3 direction = Vector3.zero;
    public Vector3 flipSpeed = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public Quaternion copRotation = Quaternion.Euler(0, 0, 0);
    public bool tipped = false;
    public event Action<bool> TippedChanged;
    public speedTracker st;
    public taxiRotation rt;

    private const float MIN_ANGLE = 45f;
    private const float FLIP_ANGLE = 65f;
    private const float MAX_SPEED = 100f;

    public PathCreator pathCreator;
    private IEnumerator coroutine;
    public float currspeed;

    // private IEnumerator playCoroutine;
    public float startTime = 0f;

    public float elapsedTime = 0f;
    public float globalRecordTime;
    public float playerRecordTime;
    public bool hasStarted = false;
    public taxiAI player;

    public Animator transition;
    public taxiCamera cam;

    public int whatCar;
    public int currentLevel;
    public float currentSpeed;
    public Button menuButton;

    public bool lapCompleted = false;

    public int index = 0;
    public int saveIndex = 0;
    public int lapLength = 0;

    public Score _score;
    public GameObject _hat;
    public CarMusicClass _car;
    public int hatIndex;
    public string hat;

    private int frame;
    private GameObject donut;

    public GameObject save;
    public saveLevel saveLevel;

    [SerializeField]
    private bool allObjectsFound = false;
    private bool ready = false;
    public bool completedDelivery = false;

    private lapTime lapTime;
    [SerializeField] private TaxiDialog achievementHandler;
    [SerializeField] private GameObject starHandler;

    public bool closedDialog = false;


    public void Awake()
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        if (playerManager != null)
        {
            Destroy(playerManager);
        }
        tipped = false;
        Application.targetFrameRate = 60;
        playerRecord = new List<float>();
        saveRecord = new List<float>();
        lapTime = GameObject.FindObjectOfType<lapTime>();

        Physics.gravity = new Vector3(0, -50F, 0);

        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<taxiCamera>();
        achievementHandler = FindAnyObjectByType<TaxiDialog>();
        playerRecord = new List<float>();
    }

    public void Start()
    {
        PlayerPrefs.SetInt("crashed", 0);
        StartCoroutine(LoadPrefs());
    }

    IEnumerator LoadPrefs()
    {
        globalRecordTime = PlayerPrefs.GetFloat("highScore");
        playerRecordTime = PlayerPrefs.GetFloat("playerScore");

        whatCar = PlayerPrefs.GetInt("car", 0);

        currentLevel = PlayerPrefs.GetInt("level", 0);

        cars = new List<GameObject>(1);

        cars.Add(Resources.Load<GameObject>("PlayerCars/TaximodeTaxi"));
        cop = Instantiate(cars[0]);

        yield return WaitForAssignments();
        yield return SetHat();
        
        ready = true;
    }

    IEnumerator WaitForAssignments()
    {

        // Assign the components to the variables
        cop = GameObject.FindGameObjectWithTag("Player");
        player = cop.GetComponent<taxiAI>();
        rb = cop.GetComponent<Rigidbody>();
        st = cop.GetComponent<speedTracker>();
        rt = cop.GetComponent<taxiRotation>();

        if (PlayerPrefs.GetInt("custom") == 1)
        {
            float b1 = PlayerPrefs.GetFloat("b1");
            float b2 = PlayerPrefs.GetFloat("b2");
            float b3 = PlayerPrefs.GetFloat("b3");
            float w1 = PlayerPrefs.GetFloat("w1");
            float w2 = PlayerPrefs.GetFloat("w2");
            float w3 = PlayerPrefs.GetFloat("w3");

            Color _colbody = new Color(b1, b2, b3);
            Color _colwindow = new Color(w1, w2, w3);
            cop.GetComponentInChildren<MeshRenderer>().materials[0].color = _colbody;
            cop.GetComponentInChildren<MeshRenderer>().materials[1].color = _colwindow;

            yield return null;

        }

        allObjectsFound = true;
    }

    IEnumerator SetHat()
    {
        //int _car = PlayerPrefs.GetInt("car");
        string _hatName = PlayerPrefs.GetString("hat");
        int _hatindex = PlayerPrefs.GetInt("hatindex", -1);
        if (_hatindex <= -1) yield break;
        if (_hatName == "no") yield break;

        string hatLocation = _hatName + "1"; // add 1 which are the smaller models
        Debug.Log("Found hat" + _hatName + " at index " + PlayerPrefs.GetInt("hatindex"));
        _hat = Instantiate(Resources.Load("GameHats/" + hatLocation) as GameObject);

        _hat.transform.SetParent(cop.transform);

        _hat.transform.localRotation = new Quaternion(0, 0, 0, 0);
        _hat.transform.localPosition = new Vector3(0, 1.4f, -0.3f);

        if (_hat.name.Contains("halo")) _hat.transform.localPosition += new Vector3(0, 0.2f, 0);
        if (_hat.name.Contains("rkey")) _hat.transform.localPosition += new Vector3(0, 0.2f, 0);
        if (_hat.name.Contains("party")) _hat.transform.localPosition += new Vector3(0, -0.2f, 0);
        if (_hat.name.Contains("antenna")) _hat.transform.localPosition += new Vector3(0, -0.35f, -0.25f);
        yield return null;
    }

    // Update is called once per frame
    public void Update()
    {
        if (!allObjectsFound)
        {
            return;
        }
        else
        {
            if (Input.GetKeyUp("escape")) Menu();

            if (ready & !completedDelivery)
            {
                GameLogic();
            }

            if (!tipped && st != null && !completedDelivery)
            {
                currentSpeed = st.speed * 50;
                float thresholdAngle = MIN_ANGLE + (FLIP_ANGLE - MIN_ANGLE) * (player.speed / MAX_SPEED);
                float adjustedThresholdAngle = thresholdAngle + Mathf.Abs(rt.turningangle);

                // Perform your logic based on the average angle and threshold angle
                if (Mathf.Abs(rt.averageAngle) > adjustedThresholdAngle)
                {
                    FlipCar();
                }
            }

            // Restart level on spacebar, touch screen or mouseclick
            else if (tipped)
            {
                if (Input.GetKeyUp("space") || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButtonDown(0)) WaitForReset(0.8f);
            }
        }
    }

    public void GameLogic()
    {
        currspeed = Time.timeScale * st.CurrentSpeed;
        if (player.touching && !hasStarted)
        {
            startTime = Time.time;
            hasStarted = true;
            frame = 0;
        }
        if (hasStarted)
        {
            elapsedTime = Time.time - startTime;
            frame++;

            // Spawn a donut every 100 frames and destroy it after 5 seconds
            if (frame > 100 && whatCar == 6 && !tipped)
            {
                donut = Instantiate(Resources.Load("donut"), cop.transform.position, cop.transform.rotation) as GameObject;
                donut.GetComponent<Rigidbody>().velocity = -cop.transform.right * 10 + new Vector3(0, UnityEngine.Random.Range(15, 20), 0) + cop.transform.forward * UnityEngine.Random.Range(15, 30);
                frame = 0;
                Destroy(donut, 30f);
            }

            // If player completes a lap, reset values and start saving
            if (player.distanceTravelled >= (pathCreator.path.length - 150f) && !tipped)
            {
                CompleteDelivery();
                // Player beat global record
                if ((globalRecordTime == 0 || elapsedTime < globalRecordTime))
                {
                    float _tempScore = Mathf.Round((elapsedTime * 100)) / 100f;

                    globalRecordTime = _tempScore;
                    playerRecordTime = _tempScore;

                    PlayerPrefs.SetFloat("highScore", _tempScore);
                    PlayerPrefs.SetFloat("playerScore", _tempScore);
                    PlayerPrefs.Save();
                }

                // Player only beat own record, not global record
                else if ((((elapsedTime < playerRecordTime) && (elapsedTime > globalRecordTime)) || (playerRecordTime == 0)))
                {
                    float _tempScore = Mathf.Round((elapsedTime * 100)) / 100f;

                    playerRecordTime = _tempScore;

                    PlayerPrefs.SetFloat("playerScore", _tempScore);
                    PlayerPrefs.Save();
                }

                //startTime = Time.time;
            }
        }
    }

    public void CompleteDelivery()
    {
        if (!completedDelivery)
        {
            achievementHandler.SetAnimationDetails("c");
            completedDelivery = true;
            starHandler.SetActive(true);
        }
    }

    public void NextDelivery()
    {
        SceneManager.LoadScene("CityTaxi");
    }

    public void FlipCar()
    {
        int crashAmount = PlayerPrefs.GetInt("crashed", 0);
        crashAmount += 1;
        PlayerPrefs.SetInt("crashed", crashAmount);
        PlayerPrefs.Save();

        tipped = true;
        st.speed = 0f;
        TippedChanged?.Invoke(tipped);
        copPosition = cop.transform.position;
        copRotation = cop.transform.rotation;
        direction = cop.transform.forward;
        if (_hat != null)
        {
            _hat.transform.parent = null;
            _hat.GetComponent<Rigidbody>().isKinematic = false;
        }
        rb = cop.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        // Calculate flip direction based on the corner direction
        Vector3 flipDirection = Quaternion.Euler(0f, 0f, -90f) * direction;

        rb.AddForce(Vector3.up * 10000f, ForceMode.Impulse);
        rb.AddForce(direction * 10000f, ForceMode.Impulse);
        rb.AddTorque(flipDirection * 5000f * -rt.averageAngle, ForceMode.Impulse);
        Instantiate(Resources.Load("carCrashSound") as GameObject);

        achievementHandler.SetAnimationDetails("b");
    }

    // This prevents corrupted saves by forcing to wait until a save is complete
    // before restarting the map
    private void WaitForReset(float _waitTime)
    {
        coroutine = ResetCar(_waitTime);
        StartCoroutine(coroutine);
    }

    private IEnumerator ResetCar(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        rt.isDelaying = true;
        rt.ClearForwardVectorBuffer();
        rb.isKinematic = true;
        tipped = false;
    }

    public void CloseDialog()
    {
        closedDialog = true;
    }

    public void Menu()
    {
        StartCoroutine(ReturnToMenu());
    }

    IEnumerator ReturnToMenu()
    {
        SceneManager.LoadScene(0);
        yield return null;
    }

    private void OnDestroy()
    {
        GameObject ambientGameObject = GameObject.FindGameObjectWithTag("Ambient");
        if (ambientGameObject != null)
        {
            AudioSource ambientAudioSource = ambientGameObject.GetComponent<AudioSource>();
            if (ambientAudioSource != null)
            {
                ambientAudioSource.pitch = 1f;
            }

            AmbientClass ambientClass = ambientGameObject.GetComponent<AmbientClass>();
            if (ambientClass != null)
            {
                ambientClass.StopAmbientMusic();
            }
        }

        GameObject musicGameObject = GameObject.FindGameObjectWithTag("Music");
        if (musicGameObject != null)
        {
            MusicClass musicClass = musicGameObject.GetComponent<MusicClass>();
            if (musicClass != null)
            {
                musicClass.StopMusic();
            }
        }

        GameObject cityGameObject = GameObject.FindGameObjectWithTag("City");
        if (cityGameObject != null)
        {
            CityScript cityScript = cityGameObject.GetComponent<CityScript>();
            if (cityScript != null)
            {
                cityScript.StopCityMusic();
            }
        }

        GameObject engineNoiseGameObject = GameObject.FindGameObjectWithTag("engineNoise");
        if (engineNoiseGameObject != null)
        {
            CarMusicClass carMusicClass = engineNoiseGameObject.GetComponent<CarMusicClass>();
            if (carMusicClass != null)
            {
                carMusicClass.StopCarMusic();
            }
        }

        GameObject snowGameObject = GameObject.FindGameObjectWithTag("Snow");
        if (snowGameObject != null)
        {
            SnowScript snowScript = snowGameObject.GetComponent<SnowScript>();
            if (snowScript != null)
            {
                snowScript.StopSnowMusic();
            }
        }
    }
}