using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class checkShit : MonoBehaviour
{
    LeaderBoard leaderBoard;
    PlayerManager pm;

    public List<GameObject> cars;
    public List<GameObject> deadCars;

    private List<float> playerRecord;
    public List<float> saveRecord;

    public List<float> loadedGhost;

    public float ghostDistanceTravelled;

    public GameObject ghost;
    public ghostFollower _ghostFollower;
    public GameObject cop;
    public GameObject deadCop;
    public Rigidbody rb;
    public Rigidbody rb2;
    public Vector3 copPosition = Vector3.zero;
    public Vector3 direction = Vector3.zero;
    public Vector3 flipSpeed = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public Quaternion copRotation = Quaternion.Euler(0, 0, 0);
    public bool tipped = false;
    public speedTracker st;
    public rotationTracker rt;
    private float tipAngleLeft = 20f;
    private float tipAngleRight = -20f;
    [SerializeField]
    private float tipSpeed = 1.6f;
    public PathCreator pathCreator;
    private IEnumerator coroutine;

    // private IEnumerator playCoroutine;
    public float startTime = 0f;

    public float elapsedTime = 0f;
    public float recordTime;
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

    public CarMusicClass _car;

    public Score _score;
    public GameObject _hat;

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

    string _s;
    [SerializeField]
    float currspeed;

    public void Awake()
    {
        pm = GameObject.FindObjectOfType<PlayerManager>();
        _score = GameObject.FindObjectOfType<Score>();
        leaderBoard = GameObject.FindObjectOfType<LeaderBoard>();

        playerRecord = new List<float>();
        saveRecord = new List<float>();
       
        currentLevel = pm.currentLevel;

        Physics.gravity = new Vector3(0, -50F, 0);
        LoadPrefs();

        cars = new List<GameObject>(7);
        cars.Add(Resources.Load<GameObject>("PlayerCars/NormalCar2"));
        cars.Add(Resources.Load<GameObject>("PlayerCars/NormalCar"));
        cars.Add(Resources.Load<GameObject>("PlayerCars/SUV"));
        cars.Add(Resources.Load<GameObject>("PlayerCars/SportsCar"));
        cars.Add(Resources.Load<GameObject>("PlayerCars/SportsCar2"));
        cars.Add(Resources.Load<GameObject>("PlayerCars/Taxi"));
        cars.Add(Resources.Load<GameObject>("PlayerCars/Cop"));

        deadCars = new List<GameObject>(7);
        deadCars.Add(Resources.Load<GameObject>("deadCars/deadNormalCar2"));
        deadCars.Add(Resources.Load<GameObject>("deadCars/deadNormalCar"));
        deadCars.Add(Resources.Load<GameObject>("deadCars/deadSUV"));
        deadCars.Add(Resources.Load<GameObject>("deadCars/deadSportsCar"));
        deadCars.Add(Resources.Load<GameObject>("deadCars/deadSportsCar2"));
        deadCars.Add(Resources.Load<GameObject>("deadCars/deadTaxi"));
        deadCars.Add(Resources.Load<GameObject>("deadCars/deadCop"));

        ghost = Instantiate(Resources.Load("ghostCar") as GameObject);
        _ghostFollower = ghost.GetComponent<ghostFollower>();
        

        if (currentLevel < 3 || currentLevel == 7 || currentLevel == 8 || currentLevel == 9)
        {
            GameObject.FindGameObjectWithTag("Ambient").GetComponent<AmbientClass>().PlayAmbientMusic();
            if (currentLevel == 7 || currentLevel == 8)
            {
                GameObject.FindGameObjectWithTag("Ambient").GetComponent<AudioSource>().pitch = 0.5f;
            }
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


        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<cameraFollow>();
        LoadGhost();
        /*
        // Wait until finished saving
        if (GameObject.FindGameObjectWithTag("save") != null)
            StartCoroutine(WaitForGhost());
        else DontWaitForGhost();
        */
    }

    public void Start()
    {
        tipped = false;
        recordTime = pm.currentScoreSO.CurrentScore;
        cop = Instantiate(cars[pm.currentCar]);
        deadCop = Instantiate(deadCars[pm.currentCar]);
        cop.GetComponent<newAI2>().enabled = true;
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
    }

    public void LoadPrefs()
    {
        whatCar = pm.carsettings.CurrentCar;
        hatIndex = pm.carsettings.CurrentHat;

    }
 
    public void LoadGhost()
    {
        loadedGhost = pm.ghostData;
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
        string levelKey = "Record level " + currentLevel;
        PlayerPrefs.SetFloat(levelKey, recordTime);
        PlayerPrefs.SetInt(carKey, whatCar);
        if (completedTutorialLap)
            PlayerPrefs.SetInt("Tutorial", 1);
        PlayerPrefs.Save();
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyUp("escape")) Menu();

        if (pm == null) 
            pm = GameObject.FindObjectOfType<PlayerManager>();
        if (_score == null) 
            _score = GameObject.FindObjectOfType<Score>();

        if (!_score.isActiveAndEnabled) _score.enabled = true;

        if (leaderBoard == null)
            leaderBoard = GameObject.FindObjectOfType<LeaderBoard>();

        if (!tipped)
        {
            if (cop == null)
                cop = GameObject.FindGameObjectWithTag("Player");

            if (rb == null)
                rb = Rigidbody.FindObjectOfType<Rigidbody>();
            
            if (st == null)
                st = speedTracker.FindObjectOfType<speedTracker>();
           
            if (rt == null)           
                rt = rotationTracker.FindObjectOfType<rotationTracker>();
            
            if (player == null)            
                player = cop.GetComponent<newAI2>();
            
            if (ghost == null)            
               ghost = GameObject.FindGameObjectWithTag("Ghost");
            
            if (_hat == null && hatIndex != 3)
            {
                if (hatIndex == 0)
                    _hat = Instantiate(Resources.Load("tophat1") as GameObject);
                if (hatIndex == 1)
                    _hat = Instantiate(Resources.Load("crown1") as GameObject);
                if (hatIndex == 2)
                    _hat = Instantiate(Resources.Load("party1") as GameObject);
                _hat.transform.SetParent(cop.transform);

                _hat.transform.localPosition = new Vector3(0, 1.4f, -0.3f);
                if (whatCar == 5)
                    _hat.transform.localPosition = new Vector3(0, 1.6f, -0.3f);
                if (hatIndex == 2)
                    _hat.transform.localPosition -= new Vector3(0, -0.2f, 0);
            }

            currentSpeed = player.speed;

            if (((Time.timeScale * st.speed) >= (Time.timeScale * tipSpeed)) && !tipped)
            {
                if ((rt.averageAngle > 0 && rt.averageAngle > tipAngleLeft))
                {
                    // Left turn condition is met, flip car
                    FlipCar();
                }
                else if ((rt.averageAngle < 0 && rt.averageAngle < tipAngleRight))
                {
                    // Right turn condition is met, flip car
                    FlipCar();
                }
            }
        }
        // Restart level on spacebar, touch screen or mouseclick
        else if (tipped)
        {
            if (Input.GetKeyUp("space") || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButtonDown(0)) WaitForReset(0.8f);
        }
    }

    public void FixedUpdate()
    {
        currspeed = Time.timeScale * st.speed;
        if (player != null && player.touching && !hasStarted)
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
            if (pm.currentLevel == 9)
            {
                if (someCount == 0 )
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

            // start ghost when player starts playing
            if (index < loadedGhost.Count && currentLevel != 9)
            {
                float ghostDistance = loadedGhost[index];

                ghost.transform.SetPositionAndRotation(pathCreator.path.GetPointAtDistance(ghostDistance), pathCreator.path.GetRotationAtDistance(ghostDistance));
                index++;
            }

            // If player completes a lap, reset values and start saving
            // this part continues working even when new scenes are loaded
            if (player.distanceTravelled >= pathCreator.path.length && !tipped)
            {
                float c = currentLevel + 1;
                string s = "car" + c.ToString();

                if (!pm.unlockedCars.Contains(s) && currentLevel != 9)
                {
                    pm.TriggerEvent("grantCar"+c);
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

                // Save ghost
                if (recordTime == 0 || elapsedTime < recordTime)
                {

                    pm.UpdateScoreText(Mathf.Round(elapsedTime * 100)/100);
                    
                    // Upload highscore
                    int _recordTime = Mathf.RoundToInt(elapsedTime * -100);

                    StartCoroutine(leaderBoard.SubmitScoreCoroutine(_recordTime, currentLevel));
                    
                    loadedGhost = playerRecord;

                    pm.StartUploadGhost(playerRecord);


                    SavePrefs();
                }
                playerRecord = new List<float>();
                startTime = Time.time;
                Victory();
            }

            // Start recording the player
            playerRecord.Add(player.distanceTravelled);
        }
    }

    // celebration when completing a lap. Very slow... causes stuttering
    // Removed for now
    private void Victory()
    {       
        Vector3 spawnVector3 = pathCreator.path.GetPointAtDistance(0) + new Vector3(0, 5, 0);
        GameObject conf1 = Instantiate(Resources.Load("confetti1"), spawnVector3, Quaternion.Euler(-40f, 0, 0f)) as GameObject;
        //GameObject conf2 = Instantiate(Resources.Load("confetti2"), spawnVector3, Quaternion.Euler(-140f, 0, 0f)) as GameObject;

        Destroy(conf1, 3f);
        //Destroy(conf2, 3f);
       
    }

    public void FlipCar()
    {
        tipped = true;

        copPosition = cop.transform.position;
        copRotation = cop.transform.rotation;
        direction = cop.transform.forward;
        if (hatIndex != 3)
        {
            _hat.transform.parent = null;
            _hat.GetComponent<Rigidbody>().isKinematic = false;
        }
        Destroy(cop);
        Destroy(rb);
        deadCop.SetActive(true);
        deadCop.transform.SetPositionAndRotation(copPosition, copRotation);

        // Calculate flip direction based on the corner direction
        Vector3 flipDirection = Quaternion.Euler(0f, 0f, -90f) * direction;


        deadCop.GetComponent<Rigidbody>().AddForce(Vector3.up * 10000f, ForceMode.Impulse);
        deadCop.GetComponent<Rigidbody>().AddForce(direction * 10000f, ForceMode.Impulse);
        deadCop.GetComponent<Rigidbody>().AddTorque(flipDirection * 5000f * -rt.averageAngle, ForceMode.Impulse);
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
        yield return leaderBoard.FetchHighscores();
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        SavePrefs();
        GameObject.FindGameObjectWithTag("City").GetComponent<CityScript>().StopCityMusic();
        GameObject.FindGameObjectWithTag("Snow").GetComponent<SnowScript>().StopSnowMusic();
        GameObject.FindGameObjectWithTag("Ambient").GetComponent<AudioSource>().pitch = 1f;
        GameObject.FindGameObjectWithTag("Ambient").GetComponent<AmbientClass>().StopAmbientMusic();
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicClass>().PlayMusic();
        _car.StopCarMusic();
        SceneManager.LoadScene(0);
    }
}