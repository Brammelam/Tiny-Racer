using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class checkShit : MonoBehaviour
{
    public LeaderBoard leaderBoard;
    public PlayerManager pm;

    public List<GameObject> cars;
    public List<GameObject> deadCars;

    private List<float> playerRecord;
    public List<float> saveRecord;

    //public List<float> loadedGhost;

    //public float ghostDistanceTravelled;

    //public GameObject ghost;
    //public ghostFollower _ghostFollower;
    public GameObject cop;
    public GameObject deadCop;
    public Rigidbody rb, rb2;
    public Vector3 copPosition = Vector3.zero;
    public Vector3 direction = Vector3.zero;
    public Vector3 flipSpeed = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public Quaternion copRotation = Quaternion.Euler(0, 0, 0);
    public bool tipped = false;
    public event Action<bool> TippedChanged;
    public speedTracker st;
    public rotationTracker rt;
    //private float tipAngleLeft = 35f;
    //private float tipAngleRight = -35f;
    private const float MIN_ANGLE = 45f;
    private const float FLIP_ANGLE = 65f;
    private const float MAX_SPEED = 100f;
    [SerializeField]
    private float tipSpeed = 1.6f;
    public PathCreator pathCreator;
    private IEnumerator coroutine;
    public float currspeed;

    // private IEnumerator playCoroutine;
    public float startTime = 0f;

    public float elapsedTime = 0f;
    public float globalRecordTime;
    public float playerRecordTime;
    public bool hasStarted = false;
    public newAI2 player;

    public Animator transition;
    private const string carKey = "Selected Car";
    public cameraFollow cam;

    public int whatCar;
    public int currentLevel;
    public float currentSpeed;
    public Button menuButton;  

    public bool lapCompleted = false;

    public int index = 0;
    public int saveIndex = 0;
    public int lapLength = 0;
    public Vector3 lastGhostDistance;

    public Score _score;
    public GameObject _hat;
    public CarMusicClass _car;
    public int hatIndex;

    private int frame;
    private GameObject donut;

    public GameObject save;
    public saveLevel saveLevel;

    // tutorial stuff
    public float slowMo = 0.5f;

    public bool setSlow = false;
    public GameObject tutorial;
    public int tutorialIndex = 0;
    public bool completedTutorial = false;
    public bool completedTutorialLap = false;
    public int someCount = 0;

    [SerializeField]
    private bool allObjectsFound = false;
    private bool ready = false;
    public AudioManager audioManager;


    public void Awake()
    {
        // Set ingame framerate to 60
        Application.targetFrameRate = 60;
        
        // Generate a playermanager
        GameObject playerManagerObject = new GameObject("PlayerManager");
        pm = playerManagerObject.AddComponent<PlayerManager>();

        tipped = false;

        currentLevel = pm.currentLevel;
        playerRecord = new List<float>();
        saveRecord = new List<float>();
        
        Physics.gravity = new Vector3(0, -50F, 0);

        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<cameraFollow>();

        /*
        // Wait until finished saving
        if (GameObject.FindGameObjectWithTag("save") != null)
            StartCoroutine(WaitForGhost());
        else DontWaitForGhost();
        */


    }

    IEnumerator LoadPrefs()
    {
        currentLevel = pm.currentLevel;

        globalRecordTime = pm.currentScoreSO.CurrentScore;
        playerRecordTime = pm.currentScoreSO.CurrentPlayerScore;
        if (currentLevel == 9) globalRecordTime = 0;

        whatCar = pm.carsettings.CurrentCar;
        hatIndex = pm.carsettings.CurrentHat;
        currentLevel = pm.currentLevel;

        cars = new List<GameObject>(1);
        deadCars = new List<GameObject>(1);

        switch (whatCar)
        {
            case 0:
                cars.Add(Resources.Load<GameObject>("PlayerCars/NormalCar2"));
                deadCars.Add(Resources.Load<GameObject>("deadCars/deadNormalCar2"));
                break;
            case 1:
                cars.Add(Resources.Load<GameObject>("PlayerCars/NormalCar"));
                deadCars.Add(Resources.Load<GameObject>("deadCars/deadNormalCar"));
                break;
            case 2:
                cars.Add(Resources.Load<GameObject>("PlayerCars/SUV"));
                deadCars.Add(Resources.Load<GameObject>("deadCars/deadSUV"));
                break;
            case 3:
                cars.Add(Resources.Load<GameObject>("PlayerCars/SportsCar"));
                deadCars.Add(Resources.Load<GameObject>("deadCars/deadSportsCar"));
                break;
            case 4:
                cars.Add(Resources.Load<GameObject>("PlayerCars/SportsCar2"));
                deadCars.Add(Resources.Load<GameObject>("deadCars/deadSportsCar2"));
                break;
            case 5:
                cars.Add(Resources.Load<GameObject>("PlayerCars/Taxi"));
                deadCars.Add(Resources.Load<GameObject>("deadCars/deadTaxi"));
                break;
            default:
                cars.Add(Resources.Load<GameObject>("PlayerCars/Cop"));
                deadCars.Add(Resources.Load<GameObject>("deadCars/deadCop"));
                break;
        }
           
        cop = Instantiate(cars[0]);
        deadCop = Instantiate(deadCars[0]);

        yield return WaitForAssignments();
        if (hatIndex != 3) yield return SetHat();

        if (currentLevel < 3 || currentLevel == 7 || currentLevel == 8 || currentLevel == 9)
        {
            GameObject.FindGameObjectWithTag("Ambient").GetComponent<AmbientClass>().PlayAmbientMusic();
            if (currentLevel == 7 || currentLevel == 8)
            {
                GameObject.FindGameObjectWithTag("Ambient").GetComponent<AudioSource>().pitch = 0.5f;
            }
            else GameObject.FindGameObjectWithTag("Ambient").GetComponent<AudioSource>().pitch = 1f;
        }
        else if (currentLevel == 3)
        {
            GameObject.FindGameObjectWithTag("City").GetComponent<CityScript>().PlayCityMusic();
            GameObject.FindGameObjectWithTag("Music").GetComponent<MusicClass>().StopMusic();
        }
        else if (currentLevel > 3 && currentLevel != 7)
        {
            GameObject.FindGameObjectWithTag("Snow").GetComponent<SnowScript>().PlaySnowMusic();
        }

        _car = GameObject.FindGameObjectWithTag("engineNoise").GetComponent<CarMusicClass>();
        _car.PlayCarMusic();
        _car.timeElapsed = 0;


        ready = true;

    }

    private void OnSpeedChanged(float _speed)
    {
        currentSpeed = _speed;
        Debug.Log("Updating speed with " + _speed);
    }
 
    IEnumerator WaitForAssignments()
    {
        // Wait until the required assignments are successful
        while (cop.GetComponent<newAI2>() == null ||
               cop.GetComponent<Rigidbody>() == null ||
               cop.GetComponent<speedTracker>() == null ||
               cop.GetComponent<rotationTracker>() == null ||
               deadCop.GetComponent<Rigidbody>() == null
               )
        {
            yield return null;
        }

        // Assign the components to the variables
        cop = GameObject.FindGameObjectWithTag("Player");
        player = cop.GetComponent<newAI2>();
        rb = cop.GetComponent<Rigidbody>();
        st = cop.GetComponent<speedTracker>();
        rt = cop.GetComponent<rotationTracker>();
        rb2 = deadCop.GetComponent<Rigidbody>();
        Debug.Log("Now assigning SPEEDTRACKER DELEGATE");
        



        //ghost = GameObject.FindGameObjectWithTag("Ghost") ?? ghost;

        //ghost = Instantiate(Resources.Load("ghostCar") as GameObject);
        //_ghostFollower = ghost.GetComponent<ghostFollower>();

        if (pm.carsettings.CustomCar)
        {
            Color _colbody = new Color(pm.carsettings.BodyColor[0], pm.carsettings.BodyColor[1], pm.carsettings.BodyColor[2]);
            Color _colwindow = new Color(pm.carsettings.WindowColor[0], pm.carsettings.WindowColor[1], pm.carsettings.WindowColor[2]);
            cop.GetComponentInChildren<MeshRenderer>().materials[0].color = _colbody;
            cop.GetComponentInChildren<MeshRenderer>().materials[1].color = _colwindow;
            deadCop.GetComponentInChildren<MeshRenderer>().materials[0].color = _colbody;
            deadCop.GetComponentInChildren<MeshRenderer>().materials[1].color = _colwindow;
        }
        deadCop.SetActive(false);
        //loadedGhost = pm.ghostData;     


        
    }

    IEnumerator SetHat()
    {
        string hatName = "";
        Vector3 hatPosition = new Vector3(0, 1.4f, -0.3f);

        switch (hatIndex)
        {
            case 0:
                hatName = "tophat1";
                break;
            case 1:
                hatName = "crown1";
                break;
            case 2:
                hatName = "party1";
                hatPosition -= new Vector3(0, -0.2f, 0);
                break;
        }

        _hat = Instantiate(Resources.Load(hatName) as GameObject);
        _hat.transform.SetParent(cop.transform);
        _hat.transform.localPosition = hatPosition;

        if (whatCar == 5)
            _hat.transform.localPosition = new Vector3(0, 1.6f, -0.3f);

        yield return null;
    }


    /*
    public void DontWaitForGhost()
    {
        loadedGhost = pm.ghostData;
        bool flagged = true;
        int i = 0;

        // Load ghost data
        while (flagged)
        {
            float input = PlayerPrefs.GetFloat("saveGhost" + i + currentLevel, -1);

            if (input == -1)
                flagged = false;
            else
            {
                loadedGhost.Add(input);
                i++;
            }
        }
    }
    */
    public void SavePrefs()
    {
        /*
        string levelKey = "Record level " + currentLevel;
        PlayerPrefs.SetFloat(levelKey, recordTime);
        PlayerPrefs.SetInt(carKey, whatCar);
        if (completedTutorialLap)
            PlayerPrefs.SetInt("Tutorial", 1);
        PlayerPrefs.Save();
        */
    }

    public void FindObjectsInScene()
    {

        pm = FindObjectOfType<PlayerManager>() ?? pm;
        _score = FindObjectOfType<Score>() ?? _score;
        leaderBoard = FindObjectOfType<LeaderBoard>() ?? leaderBoard;

        // check if all conditions are met
        if (pm != null && _score != null && leaderBoard != null)
        {
            allObjectsFound = true;
            StartCoroutine(LoadPrefs());

        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (!allObjectsFound)
        {
            FindObjectsInScene();
            return;
        }

        if (Input.GetKeyUp("escape")) Menu();

        if (!_score.isActiveAndEnabled) _score.enabled = true;

        if (ready) GameLogic();

        if (!tipped)
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

            //TUTORIAL STUFF
            if (currentLevel == 9)
            {
                if (someCount == 0)
                {
                    if (player.distanceTravelled > 120 && player.distanceTravelled < 135 && !setSlow)
                    {
                        tutorialIndex = 0;
                        setSlow = true;
                        Time.timeScale = 0.1f;
                        Time.fixedDeltaTime = Time.timeScale * 0.02f;
                    }

                    if (player.distanceTravelled > 135 && player.distanceTravelled < 275 && setSlow)
                    {
                        if (Time.timeScale < 1f)
                        {

                            Time.timeScale += 0.02f;
                            Time.fixedDeltaTime = Time.timeScale * 0.02f;
                            if (Time.timeScale >= 1f)
                            {
                                Time.timeScale = 1f;
                                Time.fixedDeltaTime = 0.02f;
                            }
                        }

                        if (Time.timeScale == 1f)
                            setSlow = false;
                    }

                    if (player.distanceTravelled > 275 && player.distanceTravelled < 280 && !setSlow)
                    {
                        tutorialIndex = 1;
                        setSlow = true;
                        Time.timeScale = 0.05f;
                        Time.fixedDeltaTime = Time.timeScale * 0.02f;
                    }

                    if (player.distanceTravelled > 280 && setSlow)
                    {
                        if (Time.timeScale < 1f)
                        {
                            Time.timeScale += 0.02f;
                            Time.fixedDeltaTime = Time.timeScale * 0.02f;
                            if (Time.timeScale >= 1f)
                            {
                                Time.timeScale = 1f;
                                Time.fixedDeltaTime = 0.02f;
                            }
                        }

                        if (Time.timeScale == 1f)
                            setSlow = false;
                    }
                }
            }

            // Spawn a donut every 100 frames and destroy it after 5 seconds
            if (frame > 100 && whatCar == 6 && !tipped)
            {
                donut = Instantiate(Resources.Load("donut"), cop.transform.position, cop.transform.rotation) as GameObject;
                donut.GetComponent<Rigidbody>().velocity = -cop.transform.right * 10 + new Vector3(0, UnityEngine.Random.Range(15, 20), 0) + cop.transform.forward * UnityEngine.Random.Range(15, 30);
                frame = 0;
                Destroy(donut, 5f);
            }

            /*
            // start ghost when player starts playing
            if (index < loadedGhost.Count && currentLevel != 9)
            {
                float ghostDistance = loadedGhost[index];

                ghost.transform.SetPositionAndRotation(pathCreator.path.GetPointAtDistance(ghostDistance), pathCreator.path.GetRotationAtDistance(ghostDistance));
                index++;
            }
            */
            // If player completes a lap, reset values and start saving
            // this part continues working even when new scenes are loaded
            if (player.distanceTravelled >= pathCreator.path.length && !tipped)
            {
                float _currentLevel = currentLevel + 1; // cars start at 1
                string _currrentLevelString = currentLevel.ToString();
                string _car = "car" + _currentLevel.ToString();
                string _gotCar = "gotcar" + _currentLevel.ToString();
                
                if (!pm.unlockedCars.Contains(_car) && currentLevel != 9)
                {
                    string triggerCarUnlock = "grantCar" + _currrentLevelString;
                    pm.TriggerEvent(triggerCarUnlock);
                    pm.unlockedCars.Add(_car);
                }

                // Disables repeating tutorial text after completing first lap
                if (currentLevel == 9)
                {
                    completedTutorial = true;
                    completedTutorialLap = true;
                    // Set tutorial completed
                    if (someCount == 0)
                    {
                        pm.TriggerEvent("tutorialUnlock");
                        pm.unlockedCars.Add("TutorialUnlock");
                    }

                    someCount = 1;
                }

                // Reset values
                player.distanceTravelled = 0;
                index = 0;

                // Player beat global record
                if ((globalRecordTime == 0 || elapsedTime < globalRecordTime) && currentLevel != 9)
                {
                    bool isGlobalRecord = true;

                    Victory(2);
                    float _tempScore = Mathf.Round((elapsedTime * 100) / 100);
                    
                    globalRecordTime = _tempScore;
                    playerRecordTime = _tempScore;

                    // Upload highscore
                    int _recordTime = Mathf.RoundToInt(elapsedTime * 100);

                    StartCoroutine(leaderBoard.SubmitScoreCoroutine(_recordTime, currentLevel, isGlobalRecord));
                    pm.UpdateScoreText(_recordTime, isGlobalRecord);
                    //loadedGhost = playerRecord;

                    //pm.StartUploadGhost(playerRecord);


                    SavePrefs();
                }

                // Player only beat own record, not global record
                else if ((((elapsedTime < playerRecordTime) && (elapsedTime > globalRecordTime)) || (playerRecordTime == 0)) && currentLevel != 9)
                {
                    bool isGlobalRecord = false;

                    Victory(1);
                    float _tempScore = Mathf.Round((elapsedTime * 100) / 100);
                    
                    playerRecordTime = _tempScore;
                    // Upload highscore
                    int _recordTime = Mathf.RoundToInt(elapsedTime * 100);

                    StartCoroutine(leaderBoard.SubmitScoreCoroutine(_recordTime, currentLevel, isGlobalRecord));
                    pm.UpdateScoreText(_recordTime, isGlobalRecord);
                }
                playerRecord = new List<float>(); // Resets the ghost data array
                startTime = Time.time;
                
            }

            // Start recording the player
            playerRecord.Add(player.distanceTravelled);
        }
    }

    // celebration when completing a lap. Very slow... causes stuttering
    // Removed for now
    private void Victory(int _i)
    {       
        Vector3 spawnVector3 = pathCreator.path.GetPointAtDistance(0) + new Vector3(0, 5, 0);
        GameObject conf1 = Instantiate(Resources.Load("confetti1"), spawnVector3, Quaternion.Euler(-40f, 0, 0f)) as GameObject;
        if (_i > 1) // Beating global record justifies double the confetti
        {
            GameObject conf2 = Instantiate(Resources.Load("confetti2"), spawnVector3, Quaternion.Euler(-140f, 0, 0f)) as GameObject;
        }

        Destroy(conf1, 3f);
        //Destroy(conf2, 3f);
       
    }

    public void FlipCar()
    {
        tipped = true;
        TippedChanged?.Invoke(tipped);
        copPosition = cop.transform.position;
        copRotation = cop.transform.rotation;
        direction = cop.transform.forward;
        if (hatIndex != 3)
        {
            _hat.transform.parent = null;
            _hat.GetComponent<Rigidbody>().isKinematic = false;
        }
        Destroy(cop);
        //Destroy(rb);
        deadCop.SetActive(true);
        deadCop.transform.SetPositionAndRotation(copPosition, copRotation);

        // Calculate flip direction based on the corner direction
        Vector3 flipDirection = Quaternion.Euler(0f, 0f, -90f) * direction;

        rb2.AddForce(Vector3.up * 10000f, ForceMode.Impulse);
        rb2.AddForce(direction * 10000f, ForceMode.Impulse);
        rb2.AddTorque(flipDirection * 5000f * -rt.averageAngle, ForceMode.Impulse);
        Instantiate(Resources.Load("carCrashSound") as GameObject);
    }

    // This prevents corrupted saves by forcing to wait until a save is complete
    // before restarting the map
    private void WaitForReset(float _waitTime)
    {
        coroutine = LoadNextScene(_waitTime);
        StartCoroutine(coroutine);
    }

    private IEnumerator LoadNextScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SavePrefs();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        StartCoroutine(ReturnToMenu());
    }

    IEnumerator ReturnToMenu()
    {
        SavePrefs();
        //yield return leaderBoard.FetchHighscores();
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        SceneManager.LoadScene(0);
        yield return null;
    }

    private void OnDestroy()
    {
        GameObject.FindGameObjectWithTag("Ambient").GetComponent<AudioSource>().pitch = 1f;
        GameObject.FindGameObjectWithTag("Ambient").GetComponent<AmbientClass>().StopAmbientMusic();
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicClass>().StopMusic();
        GameObject.FindGameObjectWithTag("City").GetComponent<CityScript>().StopCityMusic();
        GameObject.FindGameObjectWithTag("engineNoise").GetComponent<CarMusicClass>().StopCarMusic();
        GameObject.FindGameObjectWithTag("Snow").GetComponent<SnowScript>().StopSnowMusic();
    }

}