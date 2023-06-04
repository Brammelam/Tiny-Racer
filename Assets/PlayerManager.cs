using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using LootLocker.Requests;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    [Header("UI")]
    public Button nextButton;
    public Button previousButton;
    public Button selectButton;
    public GameObject cam1;
    public GameObject cam2;
    public AudioListener audio1;
    public AudioListener audio2;
    public LeaderBoard leaderBoard;
    public TMP_InputField playernameInputfield;
    public TMP_InputField playernameInputfieldSettings;
    public Text playerName;
    public GameObject infoText;
    public GameObject confirmation;
    public GameObject unlockButton;
    public GameObject playButton;
    public Text unlockText;
    public List<changeMaterial> changemat;
    private List<Material> materials;
    public List<Material> oldmaterials;
    public Color[,] oldcolors;
    public MeshRenderer newrenderer = new MeshRenderer();
    public MeshRenderer oldrenderer;
    public MeshRenderer[] alloriginalcolors;
    public delegate void ProgressUpdateDelegate(float progress);
    public event ProgressUpdateDelegate OnProgressUpdate;
    public ProgressBar progressBar;
    //public List<changeMaterial> changemat;

    [Header("Screens")]
    public GameObject welcomeScreen;
    public GameObject selectionScreen;
    public GameObject settingsScreen;
    public GameObject loadingScreen;
    public GameObject garageScreen;
    public GameObject startScreen;
    public GameObject registerScreen;
    public GameObject loginScreen;
    public GameObject resetpasswordScreen;
    public GarageScreen garage;
    public selectedCar sc;
    public GameObject selectedCar;




    [Header("Persistent Data")]
    public List<string> leaderboardNames;
    public List<string> leaderboardScores;
    public List<string> leaderboardPlayerScores;
    public FloatSO levelSO;
    public ScoresSO leaderboardSO;
    public CurrentscoreSO currentScoreSO;
    public GhostsSO ghostsSO;
    public UnlockedCarsSO unlockedCarsSO;
    public CarsettingsSO carsettings;
    public bool currentScoreSObool = false;
    public int currentCar;
    public int currentHat;
    public int currentLevel;
    public float currentTime;
    public int playerId;
    public float currentHS;
    public string playerNameString;
    public List<string> unlockedCars;
    public List<float> ghostData;
    //public List<int> ghosts;
    //public List<int> playerIds;
    public bool allSOfound;

    public float Progress { get; private set; }
    public event Action<int> LevelChanged;
    public event Action SOReady;
    GameObject audioManager;

    void Awake()
    {
        allSOfound = false;

        //audioManager = Instantiate(Resources.Load("AudioManagerPrefab") as GameObject);
        sc = GameObject.FindObjectOfType<selectedCar>();
    }

    public void OnSOReady()
    {
        SOReady?.Invoke();
    }

    public void OnLevelChanged()
    {
        LevelChanged?.Invoke(currentLevel);
    }

    private void Start()
    {
        StartCoroutine(GetSO());
        //audioManager.GetComponent<AudioManager>().Initialize();
    }

    public void Setup()
    {
        selectButton.interactable = false;
        StartCoroutine(SetupRoutine());
        //ghosts = ghostsSO.GhostIds;
    }

    public void Update()
    {
        // Call SetPlayerName when pressing Enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (settingsScreen.activeSelf == true)
            {
                if (playernameInputfieldSettings.text != "")
                {
                    SetPlayerName();
                    confirmation.SetActive(true);
                }
            }

            else if (welcomeScreen.activeSelf == true)
            {
                if (playernameInputfield.text != "")
                    SetPlayerName();
                welcomeScreen.SetActive(false);
                selectionScreen.SetActive(true);
                selectedCar.SetActive(true);
            }
        }
    }
    /*
    IEnumerator SetupRoutine()
    {
        yield return LoginRoutine();
        yield return leaderBoard.FetchHighscores();
        yield return GetPlayerName();
        yield return CheckCars();

        yield return DownloadPlayerFileKeys();
        
        yield return GetCarSettingsData();
            
        yield return SetUpUI();
        //SetGhostId();
    }
    */

    // Add something like this - We are making too many calls to the server for no reason, save in a DoNotDestroy object or something..
    public IEnumerator ReturnToMenu()
    {
        float[] stepProgress = { 0.25f, 0.25f, 0.25f };

        // Disable any login screens if they are active
        GameObject[] welcomeScreens = GameObject.FindGameObjectsWithTag("welcome");
        foreach (GameObject welcomeScreen in welcomeScreens)
        {
            welcomeScreen.SetActive(false);
        }

        IEnumerator[] coroutines = {
            SetUpUI(),
            GetSO(),
            DisableStartScreens()
        };
        Progress = 0f;

        for (int i = 0; i < coroutines.Length; i++)
        {
            yield return StartCoroutine(coroutines[i]);
            Progress += stepProgress[i];
            UpdateProgress(Progress);
        }

        Progress = 1f;
        UpdateProgress(Progress);

        yield return null;
    }

    IEnumerator SetupRoutine()
    {
        loadingScreen.SetActive(true);
        float[] stepProgress = { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.2f };

        IEnumerator[] coroutines = {        

            GetPlayerName(),           
            DownloadPlayerFileKeys(),
            CheckCars(),
            GetCarSettingsData(),
            leaderBoard.FetchHighscores(),
            leaderBoard.FetchPlayerScores(),
            SetUpUI(),
            DisableStartScreens(),

    };

        Progress = 0f;

        for (int i = 0; i < coroutines.Length; i++)
        {
            yield return StartCoroutine(coroutines[i]);
            Progress += stepProgress[i];
            UpdateProgress(Progress);
        }

        // Set Ghost ID
        // SetGhostId();

        // Progress complete
        Progress = 1f;
        UpdateProgress(Progress);
        
        
        SetSO();
    }

    public IEnumerator DisableStartScreens()
    {
        bool done = false;
        startScreen.SetActive(false);
        registerScreen.SetActive(false);
        loginScreen.SetActive(false);
        resetpasswordScreen.SetActive(false);
        loadingScreen.SetActive(false);
        done = true;
        yield return new WaitWhile(() => done == false);
    }

    void UpdateProgress(float progress)
    {
        // Invoke the event to notify subscribers
        OnProgressUpdate?.Invoke(progress);
    }

    IEnumerator SetUpUI()
    {
        bool done = false;
        selectionScreen.SetActive(true);
        selectedCar.SetActive(true);
        nextButton.onClick.AddListener(selectedCar.GetComponent<selectedCar>().NextCar);
        previousButton.onClick.AddListener(selectedCar.GetComponent<selectedCar>().PreviousCar);
        nextButton.onClick.AddListener(selectedCar.GetComponent<selectedCar>().UpdateCarName);
        previousButton.onClick.AddListener(selectedCar.GetComponent<selectedCar>().UpdateCarName);
        
        selectButton.gameObject.GetComponentInChildren<Text>().text = "PLAY";
        selectButton.interactable = true;

        
        done = true;
        yield return new WaitWhile(() => done == false);
    }

    /* GuestSession disabled while trying out WhitelabelLogin
    IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
             
                done = true;
            }
            else
            {
                Debug.Log("Could not start session" + response.Error);

                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }
    */

    public class LootLockerResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DataObject Data { get; set; }
    }

    // Nested DataObject class definition
    public class DataObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public void SetSO()
    {

    }

    public IEnumerator GetSO()
    {   
        // Load the Scriptable Objects
        leaderboardSO = Resources.Load<ScoresSO>("SO/ScoresSO");
        unlockedCarsSO = Resources.Load<UnlockedCarsSO>("SO/UnlockedCarsSO");
        currentScoreSO = Resources.Load<CurrentscoreSO>("SO/CurrentscoreSO");
        //ghostsSO;  Disabled for now, will enable ghosts later
        carsettings = Resources.Load<CarsettingsSO>("SO/CarsettingsSO"); ;
        unlockedCars = unlockedCarsSO.UnlockedCars;

        // Load the carsettings for the player
        currentCar = carsettings.CurrentCar;
        currentHat = carsettings.CurrentHat;
        currentLevel = leaderboardSO.CurrentLevel;
        if (leaderboardSO.CurrentLevel == 9 && unlockedCars.Contains("TutorialUnlock"))
        {
            leaderboardSO.CurrentLevel = 0;
            currentLevel = 0; // Fix player getting stuck in tutorial
        }
        playerNameString = currentScoreSO.CurrentPlayerName;
        //sc.addNewNode(currentHat);  This results in the player being able to wear two hats lol
        
        //load the leaderboards for highscores on levelselect
        leaderboardNames = leaderboardSO.Names;
        leaderboardScores = leaderboardSO.Values;
        leaderboardPlayerScores = leaderboardSO.PlayerValues;
        allSOfound = true;
        yield return null;
    }

    public void UpdateScoreText(float _score, bool _uploadPlayerScoreAsGlobal)
    {
        if (_uploadPlayerScoreAsGlobal)
        {
            leaderboardSO.Values[currentLevel] = _score.ToString();
            leaderboardSO.PlayerValues[currentLevel] = _score.ToString();
            leaderboardSO.Names[currentLevel] = playerNameString;
            currentScoreSO.CurrentScore = _score;
            currentScoreSO.CurrentPlayerScore = _score;
        } 
        else
        {
            leaderboardSO.PlayerValues[currentLevel] = _score.ToString();            
            currentScoreSO.CurrentPlayerScore = _score;
        }
    }

    public void TriggerEvent(string _car)
    {
        string triggerName = _car;
        LootLockerSDKManager.ExecuteTrigger(triggerName, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully retrieved triggered events");                
                unlockedCarsSO.UnlockedCars.Add(_car);
            }
            else
            {
                Debug.Log("Error retrieving triggered events");
            }
        });
    }

    // Fetch unlocked cars and check if tutorial is completed for player
    IEnumerator CheckCars()
    {
        bool done = false;
        LootLockerSDKManager.GetInventory((response) =>
        {
            if (response.success)
            {

                LootLockerInventory[] inventory = response.inventory;
                for (int i = 0; i < inventory.Length; i++)
                {

                    unlockedCars.Add(inventory[i].asset.name.ToString());

                }           

                done = true;
            }
            else
            {
                Debug.Log("Could not get inventory " + response.Error);
                done = true;
            }
        });
        unlockedCarsSO.UnlockedCars = unlockedCars;
        yield return new WaitWhile(() => done == false);
        
    }

    public void SetPlayerName()
    {
        if (settingsScreen.activeSelf == true)
        {

            if (playernameInputfieldSettings.text != "")
            {
                infoText.SetActive(true);
                infoText.GetComponent<Text>().text = @"Name changed! <('-'<)";
                playerNameString = playernameInputfieldSettings.text;
                LootLockerSDKManager.SetPlayerName(playerNameString, (response) =>
                {
                    if (response.success)
                    {

                        playerName.text = "Welcome back, " + response.name.ToString() + "!";
                        playerNameString = response.name.ToString();
                        currentScoreSO.CurrentPlayerName = playerNameString;
                    }
                    else
                    {
                        Debug.Log("Setting player name failed: " + response.Error);
                    }
                });
            }
            else
            {
                infoText.SetActive(true);
                infoText.GetComponent<Text>().text = @"Name can't be empty! \('o')/";
            }
        }
        else if (playernameInputfield.text != "")
            LootLockerSDKManager.SetPlayerName(playernameInputfield.text, (response) =>
            {
                if (response.success)
                {

                    playerName.text = "Welcome back, " + response.name.ToString() + "!";
                    playerNameString = response.name.ToString();

                }
                else
                {
                    Debug.Log("Setting player name failed: " + response.Error);
                }
            });
    }

    public void RemoveInfoText()
    {
        infoText.SetActive(false);
    }

    public void StartGetPlayerName()
    {
        StartCoroutine(GetPlayerName());
    }
    IEnumerator GetPlayerName()
    {
        bool done = false;
        LootLockerSDKManager.GetPlayerName((response) =>
        {
            if (response.success)
            {
                if (response.name != "")
                {
                    playerNameString = response.name.ToString();
                    playerName.text = "Welcome back, " + response.name.ToString() + "!";
                    currentScoreSO.CurrentPlayerName = playerNameString;
                }

                done = true;
            }
            else
            {

                Debug.Log("Getting player name failed: " + response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }

    public void SetSettings()
    {
        if (settingsScreen.activeInHierarchy)
        {
            settingsScreen.SetActive(false);
            selectedCar.SetActive(true);
            selectionScreen.SetActive(true);
        }
        else
        {
            selectionScreen.SetActive(false);
            selectedCar.SetActive(false);
            settingsScreen.SetActive(true);
            playernameInputfieldSettings.text = playerNameString;
        }
    }

    public void SetGarage()
    {
        string _c = (currentCar + 1).ToString();
        if (currentCar == 0 || unlockedCars.Contains("gotcar" + _c))
        {
            garageScreen.SetActive(true);
            garageScreen.GetComponent<GarageScreen>().SetCar(currentCar);
            cam2.SetActive(true);
            audio1.enabled = false;
            audio2.enabled = true;
            cam1.SetActive(false);
            selectionScreen.SetActive(false);
        }

    }

    public void LeaveGarage()
    {
        garageScreen.GetComponent<GarageScreen>().DestroyCar();
        garageScreen.SetActive(false);
        cam2.SetActive(false);
        cam1.SetActive(true);

        audio1.enabled = true;
        audio2.enabled = false;
        selectionScreen.SetActive(true);
    }

    public void SetCurrentLevel(int _currentLevel)
    {
        
        leaderboardSO.CurrentLevel = _currentLevel - 2; //Subtracting the carSelect and levelSelect levels

        SceneManager.LoadScene(_currentLevel);
    }

    /*
    public void SaveToFile(List<float> _ghostData)
    {
        string filePath = Path.Combine(Application.persistentDataPath + "/" + currentLevel + ".txt");
        StreamWriter writer = new StreamWriter(filePath, false);
        for (int i = 0; i < _ghostData.Count; i++)
            writer.WriteLine(_ghostData[i].ToString("F2"));
        writer.Close();
        StartCoroutine(UploadGhostData());
    }
    */
    public IEnumerator DownloadLevelTextFile(string _s)
    {
        bool done = false;

        UnityWebRequest www = UnityWebRequest.Get(_s);
        yield return www.SendWebRequest();

        string filePath = Path.Combine(Application.persistentDataPath + "/" + currentLevel + ".txt");

        List<string> temp = new List<string>(www.downloadHandler.text.Split(','));
        string[] stringarray = www.downloadHandler.text.Split(separator: '\n');
        try
        {
            for (int i = 0; i < stringarray.Length; i++)
            {

                float f = float.Parse(stringarray[i]);
                ghostData.Add(f);
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Failed at {e}");
        }

        done = true;
        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator DownloadSettingsTextFile(string _s)
    {
        bool done = false;

        

        UnityWebRequest www = UnityWebRequest.Get(_s);
        yield return www.SendWebRequest();

        string filePath = Path.Combine(Application.persistentDataPath + "/carsettings.txt");

        List<string> temp = new List<string>(www.downloadHandler.text.Split(','));
        string[] stringarray = www.downloadHandler.text.Split(separator: '\n');

        // Body
        float f = float.Parse(stringarray[0]);
        float g = float.Parse(stringarray[1]);
        float h = float.Parse(stringarray[2]);
        // Window
        float i = float.Parse(stringarray[3]);
        float j = float.Parse(stringarray[4]);
        float k = float.Parse(stringarray[5]);
        // CurrentCar
        int l = int.Parse(stringarray[6]);
        // CurrentHat
        int m = int.Parse(stringarray[7]);
        // CustomCar
        bool n = bool.Parse(stringarray[8]);
        //
        int o = int.Parse(stringarray[9]);
        // Update CarsettingsSO
        carsettings.BodyColor[0] = f;
        carsettings.BodyColor[1] = g;
        carsettings.BodyColor[2] = h;
        carsettings.WindowColor[0] = i;
        carsettings.WindowColor[1] = j;
        carsettings.WindowColor[2] = k;
        carsettings.CurrentCar = l;
        carsettings.CurrentHat = m;
        carsettings.CustomCar = n;
        leaderboardSO.CurrentLevel = o;

        done = true;
        yield return new WaitWhile(() => done == false);
    }

    /* DISABLED - Can't access files belonging to other players
     * 
     * 
    public IEnumerator GetGlobalGhostData()
    {
        bool done = false;
        int f = ghostsSO.GhostIds[currentLevel];
        string fp = ghostsSO.PlayerIds[currentLevel].ToString();

        var test = LootLocker.LootLockerEndPoints.getPlayerFilesByPlayerId;

        Debug.Log(test.endPoint);

        LootLockerSDKManager.GetAllPlayerFiles((response) =>
        {
            if (response.success)
            {
                Debug.Log(response);

                LootLockerPlayerFile[] files = response.items;
            }

            else Debug.Log(response.Error);

            done = true;
        });     
        yield return new WaitWhile(() => done == false);
    }

    */

    public IEnumerator GetGhostData()
    {

        bool done = false;
        int f = ghostsSO.GhostIds[currentLevel];

        LootLockerSDKManager.GetPlayerFile(f, (response) =>
        {
            if (response.success)
            {
                if (response.id == f)
                {
                    var s = response.url;
                    StartCoroutine(DownloadLevelTextFile(s));
                }
                else Debug.Log("FileId did not match!");


                done = true;
            }
            else
            {
                Debug.Log("Failed getting ghost data " + response.Error);
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator GetCarSettingsData()
    {
        var s = "";
        bool done = false;
        int f = int.Parse(carsettings.SettingsKey);

        LootLockerSDKManager.GetPlayerFile(f, (response) =>
        {
            if (response.success)
            {
                if (response.id == f)
                {
                    s = response.url;
                    StartCoroutine(DownloadSettingsTextFile(s));
                }
                else Debug.Log("FileId did not match!");
               
                done = true;
            }
            else
            {
                Debug.Log("Failed getting ghost data " + response.Error);
                done = true;
            }
        }); yield return new WaitWhile(() => done == false);
    }

    public void SetCarDefaultSettingsData()
    {
        newrenderer = sc.cars[currentCar].GetComponentInChildren<MeshRenderer>();
        newrenderer.materials[0].color = oldcolors[0, currentCar];
        newrenderer.materials[1].color = oldcolors[1, currentCar];
        foreach (changeMaterial changemat in changemat) changemat.SetLockedCars();
    }

    public IEnumerator UploadPlayerGhost()
    {
        bool done = false;
        string key = currentLevel.ToString();
        string pi = ghostsSO.GhostIds[currentLevel].ToString();

        LootLockerSDKManager.UpdateOrCreateKeyValue(key, pi, (response) =>
        {
            if (response.success)
                done = true;
        }); yield return new WaitWhile(() => done == false);
    }

    // Fetch ghost data and carsettings (color, which car, which hat)
    public IEnumerator DownloadPlayerFileKeys()
    {
        bool done = false;
        int i = 0;
        LootLockerSDKManager.GetEntirePersistentStorage((response) =>
        {

            foreach (var item in response.payload)
            {
                if (item.key == i.ToString())
                {

                    ghostsSO.GhostIds[i] = int.Parse(item.value);
                }
                if (item.key == "999")
                {

                    carsettings.SettingsKey = item.value;
                }

                i++;
            }
            done = true;
        }); yield return new WaitWhile(() => done == false);
    }


    public void StartUploadGhost(List<float> _ghostData)
    {
        //SaveToFile(_ghostData);
    }

    /*
    public IEnumerator UploadGhostData()
    {
        bool done = false;
        if (currentLevel != 9)
        {
            int f = ghostsSO.GhostIds[currentLevel];
            string textFilePath = Path.Combine(Application.persistentDataPath + "/" + currentLevel + ".txt");

            if (f > 0) // check if ghostId exists, if so, delete previous ghost
                LootLockerSDKManager.DeletePlayerFile(f, (response) =>
                {
                    if (response.statusCode != 200) Debug.Log(response.Error);
                });

            LootLockerSDKManager.UploadPlayerFile(textFilePath, currentLevel + " Ghost " + playerId, true, (response) =>
            {
                if (response.success)
                {
                    ghostsSO.GhostIds[currentLevel] = response.id;
                    ghostsSO.PlayerIds[currentLevel] = playerId;
                    StartCoroutine(UploadPlayerGhost());
                    done = true;
                }

                else
                {
                    Debug.Log(response.Error);
                    done = true;
                }
            }); yield return new WaitWhile(() => done == false);
        }
    }
    */

    public void ResetOriginalCar()
    {
        newrenderer = sc.cars[currentCar].GetComponentInChildren<MeshRenderer>();
        newrenderer.materials[0].color = oldcolors[0, currentCar];
        newrenderer.materials[1].color = oldcolors[1, currentCar];
    }

    public void ModifyCar()
    {
        newrenderer = sc.cars[currentCar].GetComponentInChildren<MeshRenderer>();

        Color _bodyColor = new Color(carsettings.BodyColor[0], carsettings.BodyColor[1], carsettings.BodyColor[2]);
        Color _windowColor = new Color(carsettings.WindowColor[0], carsettings.WindowColor[1], carsettings.WindowColor[2]);

        newrenderer.materials[0].color = _bodyColor;
        newrenderer.materials[1].color = _windowColor;

        sc.addNewNode(carsettings.CurrentHat);
        

        
        
    }

    public void SavePreferences()
    {
        carsettings.BodyColor[0] = garage.bodyColor.r;
        carsettings.BodyColor[1] = garage.bodyColor.g;
        carsettings.BodyColor[2] = garage.bodyColor.b;
        carsettings.WindowColor[0] = garage.windowColor.r;
        carsettings.WindowColor[1] = garage.windowColor.g;
        carsettings.WindowColor[2] = garage.windowColor.b;
        carsettings.CurrentCar = currentCar;
        carsettings.CurrentHat = sc.hatIndex;
        currentHat = sc.hatIndex;
    }

    public IEnumerator SavePreferencesToFilePM()
    {
        int c = currentCar;

        bool done = false;
        string filePath = Path.Combine(Application.persistentDataPath + "/carsettings.txt");
        StreamWriter writer = new StreamWriter(filePath, false);

        int f = int.Parse(carsettings.SettingsKey);

        if (f != 0) // check if settings exists, if so, delete previous settings
            LootLockerSDKManager.DeletePlayerFile(f, (response) =>
            {
                if (response.statusCode != 200)
                    Debug.Log(response.Error);

            });

        writer.WriteLine(carsettings.BodyColor[0]);
        writer.WriteLine(carsettings.BodyColor[1]);
        writer.WriteLine(carsettings.BodyColor[2]);
        writer.WriteLine(carsettings.WindowColor[0]);
        writer.WriteLine(carsettings.WindowColor[1]);
        writer.WriteLine(carsettings.WindowColor[2]);

        writer.WriteLine(carsettings.CurrentCar);
        writer.WriteLine(carsettings.CurrentHat);
        writer.WriteLine(carsettings.CustomCar);
        writer.WriteLine(leaderboardSO.CurrentLevel);
        writer.Close();

        LootLockerSDKManager.UploadPlayerFile(filePath, "carsettings", true, (response) =>
        {
            if (response.success)
            {
                carsettings.SettingsKey = response.id.ToString();
                StartCoroutine(UploadCarsettingsKey());
                done = true;
            }

            else
            {
                Debug.Log("Failed uploading carsettings: " + response.Error);
                done = true;
            }
        }); yield return new WaitWhile(() => done == false);
    }

    public IEnumerator UploadCarsettingsKey()
    {
        bool done = false;
        string key = "999";
        string pi = carsettings.SettingsKey.ToString();

        LootLockerSDKManager.UpdateOrCreateKeyValue(key, pi, (response) =>
        {
            if (response.success)
                done = true;
        }); yield return new WaitWhile(() => done == false);
    }


}
