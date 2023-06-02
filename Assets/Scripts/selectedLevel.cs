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
    [Header("Buttons and stuff")]
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

    [Header("Leaderboards")]
    public TextMeshProUGUI playerNames;
    public TextMeshProUGUI playerScores;
    public TextMeshProUGUI playerOwnScore;


    public Light directionalLight;
    public AudioSource _switchOn;
    public AudioSource _switchOff;
    public bool nightModeActive;

    public PlayerManager pm;
    public LeaderBoard leaderBoard;

    [Header("Scores")]
    private FloatSO ScoreSO;

    [Header("Text stuff")]
    public Text[] textObjects;
    private Color day, night;

    private bool ready = false;


    public void Start()
    {
        levels = GameObject.FindGameObjectsWithTag("nature").ToList();
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

        LookForStuff();
    }

    private void LookForStuff()
    {
        if (pm == null)
        {
            GameObject playerManagerObject = new GameObject("PlayerManager");
            pm = playerManagerObject.AddComponent<PlayerManager>();
        }

        if (leaderBoard == null)
            leaderBoard = GameObject.FindObjectOfType<LeaderBoard>();
        if (pm != null && leaderBoard != null)
        {
            ready = true;

            LoadPrefs();            
            UpdateLevelName();
        }
    }

    public void OnApplicationQuit()
    {
        UpdateLevelName();
    }

    public void Update()
    {
        foreach (GameObject level in levels)
        {
            level.transform.Rotate(0, -1f, 0);
        }
        if (!ready)
        {
            LookForStuff();
        } else
        {
            if (Input.GetKeyUp("a") || Input.GetKeyUp("left")) PreviousLevel();
            if (Input.GetKeyUp("d") || Input.GetKeyUp("right")) NextLevel();
            if (Input.GetKeyUp("space") || Input.GetKeyUp("enter")) StartCoroutine(SelectLevel());
            if (Input.GetKeyUp("escape")) ReturnToMenu();
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

    public void PreviousLevel()
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
        if (leaderBoard != null)
        {
            string _tempScore = leaderBoard.leaderboardScores[_levelIndex];
            string _tempPlayerScore = leaderBoard.leaderboardPlayerScores[_levelIndex];

            float _floatScore = float.Parse(_tempScore);
            float _floatPlayerScore = float.Parse(_tempPlayerScore);

            _floatScore *= 0.01f;
            _floatPlayerScore *= 0.01f;
            // Format the highscores retrieved from LeaderBoard
            playerNames.text = leaderBoard.leaderboardNames[_levelIndex];
            playerScores.text = _floatScore.ToString() + "s";
            playerOwnScore.text = _floatPlayerScore.ToString() + "s";
            pm.currentLevel = _levelIndex;
            SavePrefs(_floatScore, _floatPlayerScore);

        } else
        {
            playerNames.text = "No global score";
            playerScores.text = "Make your claim!";
            playerOwnScore.text = "Set your own time!";
        }
    }

    public void Wrapper()
    {
        StartCoroutine(SelectLevel());
    }

    IEnumerator SelectLevel()
    {
        pm.SetCurrentLevel(levelIndex);
        yield return null;
                   
      
    }

    public void SavePrefs(float _score, float _playerScore)
    {
        pm.currentScoreSO.CurrentScore = _score;
        pm.currentScoreSO.CurrentPlayerScore = _playerScore;
    }

    public void LoadPrefs()
    {
        // Levels start at 1
        levelIndex = pm.currentLevel + 1;

        // Camera positions for toggling current selected level
        pos1 = this.transform.position;
        pos2 = this.transform.position - (move * 8);
        this.transform.position = this.transform.position - (move * (levelIndex - 1));

        //Toggle night mode
        if (levelIndex > 7) SetNightMode(true);
        else SetNightMode(false);
        UpdateLevelName();
    }
}