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
    public TMP_Text[] tmpTextObjects;
    private Color day, night;
    [SerializeField]
    private bool ready;


    private void Awake()
    {
        move = new Vector3(100f, 0, 0);
        // Camera positions for toggling current selected level
        levels = GameObject.FindGameObjectsWithTag("nature").ToList();
        pos1 = this.transform.position;
        pos2 = this.transform.position - (move * (levels.Count - 1));
    }
    public void Start()
    {
        levelIndex = PlayerPrefs.GetInt("level", 0);
        ready = false;
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
        if (levelIndex < (levels.Count - 1))
        {
            this.transform.position = this.transform.position - move;
            levelIndex += 1;

            if (levelIndex == 7 || levelIndex == 8) SetNightMode(true);
            else SetNightMode(false);
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

            if (levelIndex == 7 || levelIndex == 8) SetNightMode(true);
            else SetNightMode(false);
        }
        else
        {
            this.transform.position = pos2;
            levelIndex = (levels.Count - 1);
            SetNightMode(false);
        }
        
        UpdateLevelName();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void TutorialLevel()
    {
        SceneManager.LoadScene(2);
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
            foreach (TMP_Text _tmpText in tmpTextObjects)
            {
                _tmpText.color = night;
            }
            _switchOn.Play();
        }
        else
        {
            foreach (Text _text in textObjects)
            {
                _text.color = day;
            }
            foreach (TMP_Text _tmpText in tmpTextObjects)
            {
                _tmpText.color = day;
            }
            _switchOff.Play();
        }
    }

    public void UpdateLevelName()
    {
        int _levelIndexName = levelIndex + 1;
        levelText.text = "1 - " + _levelIndexName.ToString();
        int _tempScore = leaderBoard.leaderboardScores[levelIndex];
        int _tempPlayerScore = leaderBoard.leaderboardPlayerScores[levelIndex];

        float _floatScore = Mathf.Round(_tempScore / 100f * 100f) / 100f;
        float _floatPlayerScore = Mathf.Round(_tempPlayerScore / 100f * 100f) / 100f;

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
        PlayerPrefs.SetInt("level", levelIndex);
        PlayerPrefs.Save();
        pm.SetCurrentLevel(levelIndex + 3); // Taking into account carSelect and levelSelect and tutorial as first three levels
        yield return null;
                   
      
    }

    public void OnSOReady()
    {
        levelIndex = PlayerPrefs.GetInt("level", 0);

        this.transform.position = this.transform.position - (move * (levelIndex));

        //Toggle night mode
        if (levelIndex == 7 || levelIndex == 8) SetNightMode(true);
        else SetNightMode(false);
        
        ready = true;
        UpdateLevelName();

    }

    public void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}