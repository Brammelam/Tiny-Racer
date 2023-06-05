﻿using System;
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
    public Vector3 move;
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
    [SerializeField]
    private bool ready;

    private void Awake()
    {
        move = new Vector3(100f, 0, 0);
        // Camera positions for toggling current selected level
        pos1 = this.transform.position;
        pos2 = this.transform.position - (move * 8);
    }
    public void Start()
    {
        ready = false;
        levels = GameObject.FindGameObjectsWithTag("nature").ToList();
        Light[] lights = FindObjectsOfType<Light>();
        foreach (Light light in lights)
        {
            if (light.type == LightType.Directional)
            {
                directionalLight = light;
            }
        }

        GameObject playerManagerObject = new GameObject("PlayerManager");
        pm = playerManagerObject.AddComponent<PlayerManager>();

        //Set the colors
        day = new Color(0.196f, 0.196f, 0.196f);
        night = new Color(0.86f, 0.86f, 0.86f);

        LookForStuff();
    }

    private void LookForStuff()
    {
        if (pm == null)
            GameObject.FindObjectOfType<PlayerManager>();

        if (leaderBoard == null)
            leaderBoard = GameObject.FindObjectOfType<LeaderBoard>();

        if (pm != null && leaderBoard != null)
        {
            ready = true;
            OnSOReady();
        }
    }

    public void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }

    private void FixedUpdate()
    {
        foreach (GameObject level in levels)
        {
            level.transform.Rotate(0, -1f, 0);
        }
    }

    public void Update()
    {

        while (!ready)
        {
            LookForStuff();
            return;
        }
        
        if (Input.GetKeyUp("a") || Input.GetKeyUp("left")) PreviousLevel();
        if (Input.GetKeyUp("d") || Input.GetKeyUp("right")) NextLevel();
        if (Input.GetKeyUp("space") || Input.GetKeyUp("enter")) StartCoroutine(SelectLevel());
        if (Input.GetKeyUp("escape")) ReturnToMenu();

    }

    public void NextLevel()
    {
        if (levelIndex < 8)
        {
            this.transform.position = this.transform.position - move;
            levelIndex += 1;
            
            if (levelIndex == 7) SetNightMode(true);            
        }
        else
        {
            this.transform.position = pos1;
            levelIndex = 0;
            SetNightMode(false);
        }

        UpdateLevelName();
    }

    public void PreviousLevel()
    {
        if (levelIndex > 0)
        {
            this.transform.position = this.transform.position + move;
            levelIndex -= 1;
            
            if (levelIndex == 6) SetNightMode(false);
        }
        else
        {
            this.transform.position = pos2;
            levelIndex = 8;
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
        int _levelIndexName = levelIndex + 1;
        levelText.text = "1 - " + _levelIndexName.ToString();

        string _tempScore = leaderBoard.leaderboardScores[levelIndex];
        string _tempPlayerScore = leaderBoard.leaderboardPlayerScores[levelIndex];

        float _floatScore = float.Parse(_tempScore) / 100;
        float _floatPlayerScore = float.Parse(_tempPlayerScore) / 100;

        // Format the highscores retrieved from LeaderBoard
        playerNames.text = leaderBoard.leaderboardNames[levelIndex];
        playerScores.text = _floatScore.ToString() + "s";
        playerOwnScore.text = _floatPlayerScore.ToString() + "s";

        PlayerPrefs.SetFloat("highScore", _floatScore);
        PlayerPrefs.SetFloat("playerScore", _floatPlayerScore);
        PlayerPrefs.SetInt("level", levelIndex);
        PlayerPrefs.Save();
    }

    public void Wrapper()
    {
        StartCoroutine(SelectLevel());
    }

    IEnumerator SelectLevel()
    {
        pm.SetCurrentLevel(levelIndex + 2); // Taking into account carSelect and levelSelect as first two levels
        yield return null;
                   
      
    }

    public void OnSOReady()
    {
        levelIndex = PlayerPrefs.GetInt("level", 0);

        this.transform.position = this.transform.position - (move * (levelIndex));

        //Toggle night mode
        if (levelIndex > 7) SetNightMode(true);
        else SetNightMode(false);
        
        ready = true;
        UpdateLevelName();

    }

    public void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}