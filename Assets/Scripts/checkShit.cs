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

    [SerializeField]
    private List<float> playerRecord;
    public List<float> saveRecord;

    public List<float> loadedGhost;

    public float ghostDistanceTravelled;

    public GameObject ghost;
    public ghostFollower _ghostFollower;
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
    public string hat;

    private int frame;
    private GameObject donut;

    public GameObject save;
    public saveLevel saveLevel;

    // tutorial stuff
    private bool tutorialLevel = false;
    public float slowMo = 1f;

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

    private lapTime lapTime;


    public void Awake()
    {
        // Set ingame framerate to 60
        Application.targetFrameRate = 60;
        
        // Generate a playermanager
        GameObject playerManagerObject = new GameObject("PlayerManager");
        pm = playerManagerObject.AddComponent<PlayerManager>();

        tipped = false;

        playerRecord = new List<float>();
        saveRecord = new List<float>();
        lapTime = GameObject.FindObjectOfType<lapTime>();
        
        Physics.gravity = new Vector3(0, -50F, 0);

        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<cameraFollow>();


        // Wait until finished saving
        //if (GameObject.FindGameObjectWithTag("save") != null)
        //    StartCoroutine(WaitForGhost());
        //else DontWaitForGhost();
        playerRecord = new List<float>();

    }

    IEnumerator LoadPrefs()
    {
        //currentLevel = pm.currentLevel;

        globalRecordTime = PlayerPrefs.GetFloat("highScore");  //globalRecordTime = pm.currentScoreSO.CurrentScore;
        playerRecordTime = PlayerPrefs.GetFloat("playerScore");  //playerRecordTime = pm.currentScoreSO.CurrentPlayerScore;

        whatCar = PlayerPrefs.GetInt("car", 0);
        
        currentLevel = PlayerPrefs.GetInt("level", 0);
        if (SceneManager.GetActiveScene().buildIndex == 2) tutorialLevel = true;
        if (tutorialLevel) globalRecordTime = 0;

        yield return pm.DownloadGhostId(); // Find the file id for the ghost data of the current track
        yield return pm.GetGhostData(); // Adds the data to the pm.ghostData list of floats we use to animate the ghost
        yield return pm.GetSO(); // Load the Scriptable Objects
        loadedGhost = pm.ghostData; // load the ghostdata for use in the GameLogic()
        

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

        if (PlayerPrefs.GetString("hat") != "no") yield return SetHat();

        if (currentLevel < 3 || currentLevel == 8 || currentLevel == 9 || currentLevel == 9)
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
        else if (currentLevel > 3 && currentLevel < 7)
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
  
        ghost = GameObject.FindGameObjectWithTag("Ghost") ?? ghost;

        ghost = Instantiate(Resources.Load("ghostCar") as GameObject);
        _ghostFollower = ghost.GetComponent<ghostFollower>();

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
            deadCop.GetComponentInChildren<MeshRenderer>().materials[0].color = _colbody;
            deadCop.GetComponentInChildren<MeshRenderer>().materials[1].color = _colwindow;
        }
        deadCop.SetActive(false);
        //loadedGhost = pm.ghostData; 
    }

    IEnumerator SetHat()
    {
        int _car = PlayerPrefs.GetInt("car");
        string _hatName = PlayerPrefs.GetString("hat");
        //Debug.Log("Found this hat: " + _hatName);

        string hatLocation = _hatName + "1"; // add 1 which are the smaller models

        _hat = Instantiate(Resources.Load("GameHats/" + hatLocation) as GameObject);

        _hat.transform.SetParent(cop.transform);

        _hat.transform.localRotation = new Quaternion(0, 0, 0, 0);
        _hat.transform.localPosition = new Vector3(0, 1.4f, -0.3f);
        if (_car == 2) // adjust for SUV
        {
            _hat.transform.localPosition = new Vector3(0, 1.6f, -0.3f);
            
        }
        if (_hat.name.Contains("halo")) _hat.transform.localPosition += new Vector3(0, 0.2f, 0);
        if (_hat.name.Contains("rkey")) _hat.transform.localPosition += new Vector3(0, 0.2f, 0);
        if (_hat.name.Contains("party")) _hat.transform.localPosition += new Vector3(0, -0.2f, 0);
        if (_hat.name.Contains("antenna")) _hat.transform.localPosition += new Vector3(0, -0.35f, -0.25f);
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
        st = FindObjectOfType<speedTracker>() ?? st;

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
        else
        {
            if (Input.GetKeyUp("escape")) Menu();

            if (!_score.isActiveAndEnabled) _score.enabled = true;

            if (ready)
            {
                
                GameLogic();
            }

            if (!tipped && st != null)
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

            // Start recording the player
            playerRecord.Add(player.distanceTravelled);

            //TUTORIAL STUFF
            if (tutorialLevel)
            {
                if (someCount == 0)
                {
                    if (player.distanceTravelled > 120 && player.distanceTravelled < 135)
                    {
                        tutorialIndex = 0;

                    }

                    if (player.distanceTravelled > 275 && player.distanceTravelled < 280)
                    {
                        tutorialIndex = 1;
                    }

                }
            }

            // Spawn a donut every 100 frames and destroy it after 5 seconds
            if (frame > 100 && whatCar == 6 && !tipped)
            {
                donut = Instantiate(Resources.Load("donut"), cop.transform.position, cop.transform.rotation) as GameObject;
                donut.GetComponent<Rigidbody>().velocity = -cop.transform.right * 10 + new Vector3(0, UnityEngine.Random.Range(15, 20), 0) + cop.transform.forward * UnityEngine.Random.Range(15, 30);
                frame = 0;
                Destroy(donut, 30f);
            }

            
            // start ghost when player starts playing
            if (index < loadedGhost.Count && !tutorialLevel)
            {
                float ghostDistance = loadedGhost[index];

                ghost.transform.SetPositionAndRotation(pathCreator.path.GetPointAtDistance(ghostDistance), pathCreator.path.GetRotationAtDistance(ghostDistance));
                index++;
            }
                     
            // If player completes a lap, reset values and start saving
            if (player.distanceTravelled >= pathCreator.path.length && !tipped)
            {
                float _currentLevel = currentLevel + 1; // cars start at 1
                string _currrentLevelString = currentLevel.ToString();
                string _car = "car" + _currentLevel.ToString();
                string _gotCar = "gotcar" + _currentLevel.ToString();
                
                if (!PlayerPrefs.HasKey(_car) && !tutorialLevel)
                {
                    string triggerCarUnlock = "grantCar" + _currrentLevelString;
                    pm.TriggerEvent(triggerCarUnlock);
                    PlayerPrefs.SetInt(_car, 1);
                    PlayerPrefs.Save();
                }

                // Disables repeating tutorial text after completing first lap
                if (tutorialLevel)
                {
                    completedTutorial = true;
                    completedTutorialLap = true;
                    // Set tutorial completed
                    if (someCount == 0)
                    {
                        pm.TriggerEvent("tutorialUnlock");
                        PlayerPrefs.SetString("TutorialUnlock", "true");
                        PlayerPrefs.Save();
                    }

                    someCount = 1;
                }

                // Reset values
                player.distanceTravelled = 0;
                index = 0;

                // Player beat global record
                if ((globalRecordTime == 0 || elapsedTime < globalRecordTime) && !tutorialLevel)
                {
                    bool isGlobalRecord = true;

                    Victory(2); // instatiate some confetti

                    float _tempScore = Mathf.Round((elapsedTime * 100)) / 100f;
                    
                    globalRecordTime = _tempScore;
                    playerRecordTime = _tempScore;

                    lapTime.SetRecord(_tempScore); // Update the UI

                    PlayerPrefs.SetFloat("highScore", _tempScore);
                    PlayerPrefs.SetFloat("playerScore", _tempScore);
                    PlayerPrefs.Save();
                    // Upload highscore
                    int _recordTime = Mathf.RoundToInt(elapsedTime * 100);

                    StartCoroutine(leaderBoard.SubmitScoreCoroutine(_recordTime, currentLevel, isGlobalRecord));
                    pm.UpdateScoreText(_recordTime, isGlobalRecord);

                    loadedGhost = playerRecord;

                    pm.StartUploadGhost(playerRecord);

                }

                // Player only beat own record, not global record
                else if ((((elapsedTime < playerRecordTime) && (elapsedTime > globalRecordTime)) || (playerRecordTime == 0)) && !tutorialLevel)
                {
                    bool isGlobalRecord = false;

                    Victory(1);
                    float _tempScore = Mathf.Round((elapsedTime * 100)) / 100f;

                    playerRecordTime = _tempScore;

                    lapTime.SetRecord(_tempScore); // Update the UI

                    PlayerPrefs.SetFloat("playerScore", _tempScore);
                    PlayerPrefs.Save();
                    // Upload highscore
                    int _recordTime = Mathf.RoundToInt(elapsedTime * 100);
                    StartCoroutine(leaderBoard.SubmitScoreCoroutine(_recordTime, currentLevel, isGlobalRecord));
                    pm.UpdateScoreText(_recordTime, isGlobalRecord);

                    loadedGhost = playerRecord;

                    pm.StartUploadGhost(playerRecord);
                }

                playerRecord = new List<float>(); // Resets the ghost data array
                startTime = Time.time;                
            }
        }
    }

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
        if (PlayerPrefs.GetInt("hatindex") > -1)
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
        GameObject.FindGameObjectWithTag("engineNoise").GetComponent<CarMusicClass>().StopCarMusic();
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

    public void SetTimeScale()
    {
        Time.timeScale = 1f;
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