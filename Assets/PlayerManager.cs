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
using System.Globalization;

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
    public GameObject guestScreen;
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
    public List<int> ghosts;
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

    private void Start()
    {
        ghostData = new List<float>();
        ghostsSO = Resources.Load<GhostsSO>("SO/GhostsSO");
        //audioManager.GetComponent<AudioManager>().Initialize();
    }

    public void Setup()
    {
        selectButton.interactable = false;
        StartCoroutine(SetupRoutine());
        
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
        }
    }

    public IEnumerator ReturnToMenu()
    {
        float[] stepProgress = { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };

        // Disable any login screens if they are active
        GameObject[] welcomeScreens = GameObject.FindGameObjectsWithTag("welcome");
        foreach (GameObject welcomeScreen in welcomeScreens)
        {
            welcomeScreen.SetActive(false);
        }

        string _playa = PlayerPrefs.GetString("name");
        playerName.text = "Welcome back, " + _playa + "!";
        currentCar = PlayerPrefs.GetInt("car", 0);

        IEnumerator[] coroutines = {
            SetUpUI(),
            leaderBoard.FetchHighscores(),
            leaderBoard.FetchPlayerScores(),
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
        float[] stepProgress = { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.2f };

        IEnumerator[] coroutines = {

            GetPlayerName(),
            DownloadPlayerFileKeys(),
            CheckCars(),
            GetCarSettingsData(),
            leaderBoard.FetchHighscores(),
            leaderBoard.FetchPlayerScores(),
            SetUpUI(),
            GetSO(),            
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
        //SetGhostId();

        // Progress complete
        Progress = 1f;
        UpdateProgress(Progress);       
        
    }

    public IEnumerator DisableStartScreens()
    {
        bool done = false;
        startScreen.SetActive(false);
        guestScreen.SetActive(false);
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
        
        carsettings = Resources.Load<CarsettingsSO>("SO/CarsettingsSO"); ;

        // Load the carsettings for the player 
        currentCar = PlayerPrefs.GetInt("car", 0); //currentCar = carsettings.CurrentCar;

        currentLevel = PlayerPrefs.GetInt("level", 0); //currentLevel = leaderboardSO.CurrentLevel;
        if (currentLevel == -1 && PlayerPrefs.HasKey("TutorialUnlock"))
            currentLevel = 0; // Fix player getting stuck in tutorial

        playerNameString = PlayerPrefs.GetString("name");
        
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
            if (!response.success)
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
                    string item = inventory[i].asset.name.ToString();
                    unlockedCars.Add(item);
                    if (!PlayerPrefs.HasKey(item)) PlayerPrefs.SetInt(item, 1);                       

                }
                PlayerPrefs.Save();
                done = true;
            }
            else
            {
                Debug.Log("Could not get inventory " + response.Error);
                done = true;
            }
        });
        
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
                PlayerPrefs.SetString("name", playerNameString);
                PlayerPrefs.Save();
                LootLockerSDKManager.SetPlayerName(playerNameString, (response) =>
                {
                    if (response.success)
                    {

                        playerName.text = "Welcome back, " + response.name.ToString() + "!";
                        playerNameString = response.name.ToString();
                        PlayerPrefs.SetString("name", playerNameString);
                        PlayerPrefs.Save();
                        //currentScoreSO.CurrentPlayerName = playerNameString;
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
        {


            LootLockerSDKManager.SetPlayerName(playernameInputfield.text, (response) =>
            {
                if (response.success)
                {

                    playerName.text = "Welcome back, " + response.name.ToString() + "!";
                    playerNameString = response.name.ToString();
                    PlayerPrefs.SetString("name", playerNameString);
                    PlayerPrefs.Save();

                }
                else
                {
                    Debug.Log("Setting player name failed: " + response.Error);
                }
            });
        }
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
                    //currentScoreSO.CurrentPlayerName = playerNameString;
                    PlayerPrefs.SetString("name", playerNameString);
                    PlayerPrefs.Save();
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
        string gotcar = "gotcar" + _c;
        currentCar = PlayerPrefs.GetInt("car", 0);
        if (currentCar == 0 || PlayerPrefs.HasKey(gotcar))
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
        SceneManager.LoadScene(_currentLevel);
    }
   
    public void SaveToFile(List<float> _ghostData)
    {
        string filePath = Path.Combine(Application.persistentDataPath + "/" + currentLevel + ".txt");
        StreamWriter writer = new StreamWriter(filePath, false);
        for (int i = 0; i < _ghostData.Count; i++)
            writer.WriteLine(_ghostData[i].ToString("F2"));
        writer.Close();
        StartCoroutine(UploadGhostData());
    }
    
    public IEnumerator DownloadLevelTextFile(string _s)
    {
        bool done = false;
        int _currentLevel = PlayerPrefs.GetInt("level", 0);
        UnityWebRequest www = UnityWebRequest.Get(_s);
        yield return www.SendWebRequest();

        string filePath = Path.Combine(Application.persistentDataPath + "/" + _currentLevel + ".txt");

        //List<string> temp = new List<string>(www.downloadHandler.text.Split(','));
        string[] lines = www.downloadHandler.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        try
        {
            foreach (string line in lines)
            {
                if (float.TryParse(line, out float floatValue))
                {
                    ghostData.Add(floatValue);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Failed at {e}");
            done = true;
        }

        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator DownloadSettingsTextFile(string _s)
    {
        // We use a custom txt file stored on LootLocker for more permanent storage of playerprefs :D
        bool done = false;       

        UnityWebRequest www = UnityWebRequest.Get(_s);
        yield return www.SendWebRequest();

        string filePath = Path.Combine(Application.persistentDataPath + "/carsettings.txt");

        string[] stringarray = www.downloadHandler.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

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
        int n = int.Parse(stringarray[8]);
        //
        int o = int.Parse(stringarray[9]);
        // Update CarsettingsSO
        PlayerPrefs.SetFloat("b1", f);
        PlayerPrefs.SetFloat("b2", g);
        PlayerPrefs.SetFloat("b2", h);
        PlayerPrefs.SetFloat("b2", i);
        PlayerPrefs.SetFloat("b2", j);
        PlayerPrefs.SetFloat("b2", k);
        PlayerPrefs.SetInt("car", l);
        PlayerPrefs.SetInt("hatindex", m);
        PlayerPrefs.SetInt("custom", n);
        PlayerPrefs.SetInt("level", o);
        PlayerPrefs.Save();

        done = true;
        yield return new WaitWhile(() => done == false);
    }

    /*
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
        int _level = PlayerPrefs.GetInt("level", 0);

        int _ghostFileID = ghostsSO.GhostIds[_level];

        LootLockerSDKManager.GetPlayerFile(_ghostFileID, (response) =>
        {
            if (response.success)
            {
                if (response.id == _ghostFileID)
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
        int _settingsKey = 0;
        if (PlayerPrefs.HasKey("settings"))
        {
            _settingsKey = PlayerPrefs.GetInt("settings");

            LootLockerSDKManager.GetPlayerFile(_settingsKey, (response) =>
            {
                if (response.success)
                {
                    if (response.id == _settingsKey)
                    {
                        s = response.url;
                        StartCoroutine(DownloadSettingsTextFile(s));
                    }
                    else Debug.Log("FileId did not match!");
               
                    done = true;
                }
                else
                {
                    Debug.Log("Failed getting car settings data " + response.Error);
                    done = true;
                }
            });
        }
        else done = true; 
        yield return new WaitWhile(() => done == false);
    }

    public void SetCarDefaultSettingsData()
    {
        int _car = PlayerPrefs.GetInt("car", 0);
        newrenderer = sc.cars[_car].GetComponentInChildren<MeshRenderer>();
        newrenderer.materials[0].color = oldcolors[0, _car];
        newrenderer.materials[1].color = oldcolors[1, _car];
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
            {
                done = true;
            }
            else
            {
                Debug.Log("Error uploading player ghost! ");
                done = true;
            }
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
                if (item.key == "9999")
                {
                    int _settings = int.Parse(item.value);
                    PlayerPrefs.SetInt("settings", _settings);
                    PlayerPrefs.Save();
                    //carsettings.SettingsKey = item.value;
                }

                i++;
            }
            done = true;
        }); yield return new WaitWhile(() => done == false);
    }

    public IEnumerator DownloadGhostId()
    {
        bool done = false;
        int i = 0;
        int _level = PlayerPrefs.GetInt("level", 0);

        LootLockerSDKManager.GetEntirePersistentStorage((response) =>
        {
            foreach (var item in response.payload)
            {
                if (item.key == i.ToString())
                {
                    ghostsSO.GhostIds[i] = int.Parse(item.value);
                }

                i++;
            }
            done = true;
        }); yield return new WaitWhile(() => done == false);
    }


    public void StartUploadGhost(List<float> _ghostData)
    {
        SaveToFile(_ghostData);
    }
    
    
    public IEnumerator UploadGhostData()
    {
        int _level = PlayerPrefs.GetInt("level", 0);
        int _playerId = PlayerPrefs.GetInt("playerid");
        string _ghostFileId = "ghost" + _level;

        bool done = false;
        if (currentLevel != 9)
        {
            int _fileID = ghostsSO.GhostIds[_level];
            string textFilePath = Path.Combine(Application.persistentDataPath + "/" + _level + ".txt");

            if (PlayerPrefs.HasKey(_ghostFileId)) // Delete old ghost if we have one
            {                
                LootLockerSDKManager.DeletePlayerFile(_fileID, (response) =>
                {
                    if (response.statusCode != 200) Debug.Log("Old ghost data does not exist");
                });
            }
            // Upload the new ghost data
            string _levelString = _level.ToString();
            LootLockerSDKManager.UploadPlayerFile(textFilePath, _levelString, true, (response) =>
            {
                if (response.success)
                {
                    ghostsSO.GhostIds[_level] = response.id;                   
                    
                    PlayerPrefs.SetInt(_ghostFileId, response.id); // store the filekey in playerprefs
                    PlayerPrefs.Save();
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
    

    public void ResetOriginalCar()
    {
        newrenderer = sc.cars[currentCar].GetComponentInChildren<MeshRenderer>();
        newrenderer.materials[0].color = oldcolors[0, currentCar];
        newrenderer.materials[1].color = oldcolors[1, currentCar];
    }

    public void ModifyCar()
    {
        float b1 = PlayerPrefs.GetFloat("b1");
        float b2 = PlayerPrefs.GetFloat("b2");
        float b3 = PlayerPrefs.GetFloat("b3");
        float w1 = PlayerPrefs.GetFloat("w1");
        float w2 = PlayerPrefs.GetFloat("w2");
        float w3 = PlayerPrefs.GetFloat("w3");

        int _car = PlayerPrefs.GetInt("car");
        newrenderer = sc.cars[_car].GetComponentInChildren<MeshRenderer>();
        
        Color _bodyColor = new Color(b1, b2, b3);
        Color _windowColor = new Color(w1, w2, w3);

        newrenderer.materials[0].color = _bodyColor;
        newrenderer.materials[1].color = _windowColor;
            
    }

    // Preferences are saved in the garageScreen
    /*
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
    */

    public IEnumerator SavePreferencesToFilePM()
    {

        PlayerPrefs.SetInt("car", currentCar);

        bool done = false;
        string filePath = Path.Combine(Application.persistentDataPath + "/carsettings.txt");
        StreamWriter writer = new StreamWriter(filePath, false);

        int f = int.Parse(carsettings.SettingsKey);
        int _settingsKey = PlayerPrefs.GetInt("settings");
        
        // check if settings exists, if so, delete previous settings
        if (PlayerPrefs.HasKey("settings")) {
            _settingsKey = PlayerPrefs.GetInt("settings");
            LootLockerSDKManager.DeletePlayerFile(_settingsKey, (response) =>
            {
                if (!response.success)
                    Debug.Log("Failed removing old playerfiles! " + response.Error);
            });
        }

        writer.WriteLine(PlayerPrefs.GetFloat("b1"));  //writer.WriteLine(carsettings.BodyColor[0]);
        writer.WriteLine(PlayerPrefs.GetFloat("b2"));  //writer.WriteLine(carsettings.BodyColor[1]);
        writer.WriteLine(PlayerPrefs.GetFloat("b3"));  //writer.WriteLine(carsettings.BodyColor[2]);
        writer.WriteLine(PlayerPrefs.GetFloat("w1"));  //writer.WriteLine(carsettings.WindowColor[0]);
        writer.WriteLine(PlayerPrefs.GetFloat("w2"));  //writer.WriteLine(carsettings.WindowColor[1]);
        writer.WriteLine(PlayerPrefs.GetFloat("w3"));  //writer.WriteLine(carsettings.WindowColor[2]);

        writer.WriteLine(PlayerPrefs.GetInt("car", 0));  //writer.WriteLine(carsettings.CurrentCar);
        writer.WriteLine(PlayerPrefs.GetInt("hatindex", -1));  //writer.WriteLine(carsettings.CurrentHat);
        writer.WriteLine(PlayerPrefs.GetInt("custom", 0));  //writer.WriteLine(carsettings.CustomCar);
        writer.WriteLine(PlayerPrefs.GetInt("level", 0));  //writer.WriteLine(leaderboardSO.CurrentLevel);
        writer.Close();

        LootLockerSDKManager.UploadPlayerFile(filePath, "settings", true, (response) =>
        {
            if (response.success)
            {
                PlayerPrefs.SetInt("settings", response.id);
                PlayerPrefs.Save();
                //carsettings.SettingsKey = response.id.ToString();
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
        string key = "9999";
        string pi = carsettings.SettingsKey.ToString();
        string _settingsKey = PlayerPrefs.GetInt("settings").ToString();

        LootLockerSDKManager.UpdateOrCreateKeyValue(key, _settingsKey, (response) =>
        {
            if (response.success)
                done = true;
            else
            {
                Debug.Log("Failed updating the KeyValue pair for the settings! " + response.Error);
                done = true;
            }
        }); yield return new WaitWhile(() => done == false);
    }

    public void OnDestroy()
    {
        PlayerPrefs.Save();
    }


}
