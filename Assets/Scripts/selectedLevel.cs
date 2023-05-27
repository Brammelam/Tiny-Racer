using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class selectedLevel : MonoBehaviour
{
    public Button nextButton;
    public Button previousButton;
    public Button selectButton;
    public Button exitButton;
    public Button tutorialButton;
    public GameObject nightBackDrop;
    private List<GameObject> levels = new List<GameObject>();
    public int levelIndex;
    public int tutorialIndex;
    public float levelRecord;
    public float levelGlobalRecord;
    private const string levelKey = "Selected Level";
    private const string tutorialKey = "Tutorial";
    public Vector3 pos1 = Vector3.zero;
    public Vector3 pos2 = Vector3.zero;
    public Vector3 move = new Vector3(100f, 0, 0);
    public float moveSpeed = 0.1f;
    public Text levelText;
    public Text levelRecordText;
    public Text levelRecordTitleText;

    public TextMeshProUGUI playerNames;
    public TextMeshProUGUI playerScores;

    public Light directionalLight;
    public AudioSource _switchOn;
    public AudioSource _switchOff;
    public bool nightModeActive;

    public PlayerManager pm;
    public LeaderBoard leaderBoard;

    [SerializeField]
    private FloatSO ScoreSO;

    [SerializeField]
    public Text[] textObjects;
    private Color day, night;

    // Start is called before the first frame update
    public void Awake()
    {
        pm = GameObject.FindObjectOfType<PlayerManager>();
        leaderBoard = GameObject.FindObjectOfType<LeaderBoard>();
        
        LoadPrefs();

        levels = GameObject.FindGameObjectsWithTag("nature").ToList();

        nextButton.onClick.AddListener(NextLevel);
        previousButton.onClick.AddListener(PreviousLevel);
        selectButton.onClick.AddListener(Wrapper);

        UpdateLevelName();

        // Camera positions for toggling current selected level
        pos1 = this.transform.position;
        pos2 = this.transform.position - (move * 8);
        this.transform.position = this.transform.position - (move * (levelIndex - 1));


    }

    public void Start()
    {
        Light[] lights = FindObjectsOfType<Light>();
        foreach (Light light in lights)
        {
            if (light.type == LightType.Directional)
            {
                directionalLight = light;
            }
        }

        //Set the colors
        day = new Color(0.196f, 0.196f, 0.196f);
        night = new Color(0.86f, 0.86f, 0.86f);

        //Toggle night mode
        if (levelIndex > 7) SetNightMode(true);
        else SetNightMode(false);
    }

    public void OnApplicationQuit()
    {
        SavePrefs();
    }

    public void Update()
    {
        if (Input.GetKeyUp("a") || Input.GetKeyUp("left")) PreviousLevel();
        if (Input.GetKeyUp("d") || Input.GetKeyUp("right")) NextLevel();
        if (Input.GetKeyUp("space") || Input.GetKeyUp("enter")) StartCoroutine(SelectLevel());
        if (Input.GetKeyUp("escape")) ReturnToMenu();
    }

    public void FixedUpdate()
    {
        foreach (GameObject level in levels)
        {
            level.transform.Rotate(0, -1f, 0);
        }
    }

    public void NextLevel()
    {
        if (levelIndex < 9)
        {
            this.transform.position = this.transform.position - move;
            levelIndex += 1;
            if (levelIndex == 8)
            {
                SetNightMode(true);
            }
        }
        else
        {
            this.transform.position = pos1;
            levelIndex = 1;
            SetNightMode(false);
        }

        UpdateLevelName();
    }

    private void PreviousLevel()
    {
        if (levelIndex > 1)
        {
            this.transform.position = this.transform.position + move;
            levelIndex -= 1;
            if (levelIndex == 7) SetNightMode(false);
        }
        else
        {
            this.transform.position = pos2;
            levelIndex = 9;
            SetNightMode(true);
        }
        
        UpdateLevelName();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void TutorialLevel()
    {
        SceneManager.LoadScene(11);
    }

    public void SetNightMode(bool _mode)
    {
        nightBackDrop.SetActive(_mode);
        directionalLight.enabled = !_mode;
        if (_mode)
        {
            foreach (Text _text in textObjects) {
                _text.color = night;
            }
            _switchOn.Play();
        }
        else
        {
            foreach (Text _text in textObjects)
            {
                _text.color = day;
            }
            _switchOff.Play();
        }
    }

    public void UpdateLevelName()
    {
        int _levelIndex = levelIndex - 1;
        levelText.text = "1 - " + levelIndex.ToString();
        if (pm.leaderboardScores[_levelIndex] != "")
        {
            string _tempScore = pm.leaderboardScores[_levelIndex];
            float _floatScore = float.Parse(_tempScore);
            _floatScore *= -0.01f;
            // Format the highscores retrieved from LeaderBoard
            playerNames.text = pm.leaderboardNames[_levelIndex];
            playerScores.text = (_floatScore).ToString() + "s";
            pm.UpdateScoreText(_floatScore);

        } else
        {
            playerNames.text = "No record for level";
            playerScores.text = "Make your claim!";
            pm.UpdateScoreText(0);
        }


        SavePrefs();
    }

    public void Wrapper()
    {
        StartCoroutine(SelectLevel());
    }

    IEnumerator SelectLevel()
    {
        pm.SetCurrentLevel(levelIndex);
        yield return pm.GetGhostData();
                   
      
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetInt(levelKey, levelIndex);
        PlayerPrefs.Save();

    }

    public void LoadPrefs()
    {
        var tutorial = PlayerPrefs.GetInt(tutorialKey, 1);
        levelIndex = 1;
        tutorialIndex = tutorial;
    }
}