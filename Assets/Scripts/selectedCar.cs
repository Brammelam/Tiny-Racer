using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class selectedCar : MonoBehaviour
{
    public bool settings = false;

    [SerializeField]
    public PlayerManager pm;
    public List<GameObject> cars = new List<GameObject>();
    public List<GameObject> hats = new List<GameObject>();
    public GameObject _hat;
    public GameObject cop;
    public List<string> carNames = new List<string>();
    public int carIndex;
    public int hatIndex;
    private const string carKey = "Selected Car";
    private const string hatKey = "Selected Hat";
    public Vector3 pos1 = Vector3.zero;
    public Vector3 pos2 = Vector3.zero;
    public Vector3 move;
    public float moveSpeed = 0.1f;
    public Text carText;

    public AudioMixer mixer;

    public Slider _volumeSlider;
    public Slider _musicSlider;
    public Slider _effectsSlider;

    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;

    private Resolution[] resolutions;

    public int _qualityIndex = 0;
    public bool _select = false;
    public bool carChangeEvent = false;

    changeMaterial change;

    [SerializeField]
    public CarsettingsSO carsettings;
    public ScoresSO leaderboardSO;

    [SerializeField]
    public hideButton hideButton;
    [SerializeField]
    private HatManager hatManager;


    private void Awake()
    {
        cars = GameObject.FindGameObjectsWithTag("car").ToList();
        hats = GameObject.FindGameObjectsWithTag("hatButton").ToList();
        cars = cars.OrderBy(car => car.name).ToList();

        move = new Vector3(100f, 0, 0);
        pos1 = this.transform.position;
        pos2 = this.transform.position - (move * 6);

        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicClass>().PlayMusic();
        LoadPrefs();
    }

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(true);
        }
    }

    private void OnApplicationQuit()
    {

    }

    public void Unlock()
    {
        int i = PlayerPrefs.GetInt("car") + 1;
        string s = i.ToString();
        string input = "getCar" + s;
        string gotcar = "gotCar" + s;

        pm.TriggerEvent(input);
        PlayerPrefs.SetString(gotcar, "true");
        PlayerPrefs.Save();
        change = transform.GetChild(pm.currentCar).gameObject.GetComponent<changeMaterial>();
        change.Unlock();
        
    }


    public void ChangeQuality(int qualityIndex)
    {
        qualityDropdown.value = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("Quality", qualityIndex);
        PlayerPrefs.Save();
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void Update()
    {
        if (pm == null) pm = GameObject.FindObjectOfType<PlayerManager>();

        if (!settings)
        {
            if (Input.GetKeyUp("a") || Input.GetKeyUp("left"))
            {
                PreviousCar();
                //removeNewNodeButton();
            }
            if (Input.GetKeyUp("d") || Input.GetKeyUp("right"))
            {
                NextCar();
                //removeNewNodeButton();
            }
            if (Input.GetKeyUp("space") || Input.GetKeyUp("enter"))
            {               
                //SelectCar();
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (GameObject car in cars) car.transform.Rotate(0, -1f, 0);
        foreach (GameObject hat in hats) hat.transform.Rotate(0, -1f, 0);
    }

    public void UpdateCarName()
    {
        int _car = PlayerPrefs.GetInt("car", 0);
        carText.text = carNames[_car];
        // Disable outlines on all hats when changing cars (hat is turned off so outline must also be turned off)
        GameObject[] rdwoc = GameObject.FindGameObjectsWithTag("hatButton");
        foreach (GameObject lol in rdwoc)
            lol.GetComponent<Outline>().enabled = false;
    }

    private void DontMoveHat(Vector3 _move)
    {
        foreach (GameObject hat in hats)
        {
            hat.transform.position = hat.transform.position + _move;
        }
    }

    private void DeletePlayerPrefsWhenSwitchingCars()
    {
        hatManager.RemoveHat();
        PlayerPrefs.SetInt("car", carIndex);
        PlayerPrefs.SetString("hat", "no");
        PlayerPrefs.SetInt("hatindex", -1);
        PlayerPrefs.SetInt("custom", 0);
        PlayerPrefs.Save();
    }

    public void NextCar()
    {
        if (pm.currentCar < 6)
        {
            this.transform.position = this.transform.position - move;
            DontMoveHat(move);
            pm.currentCar += 1;
            carIndex += 1;
            DeletePlayerPrefsWhenSwitchingCars();
            hideButton.CheckUnlock(carIndex);
        }
        else
        {
            this.transform.position = pos1;
            DontMoveHat(-6 * move);
            pm.currentCar = 0;
            carIndex = 0;
            DeletePlayerPrefsWhenSwitchingCars();
            hideButton.CheckUnlock(carIndex);
        }
    }

    public void PreviousCar()
    {
        if (pm.currentCar > 0)
        {
            this.transform.position = this.transform.position + move;
            DontMoveHat(-move);
            pm.currentCar -= 1;
            carIndex -= 1;
            DeletePlayerPrefsWhenSwitchingCars();
            hideButton.CheckUnlock(carIndex);
        }
        else
        {
            this.transform.position = pos2;
            DontMoveHat(6 * move);
            pm.currentCar = 6;
            carIndex = 6;
            DeletePlayerPrefsWhenSwitchingCars();
            hideButton.CheckUnlock(carIndex);
        }
    }

    public void SelectCar()
    {
        StartCoroutine(SelectCarCoroutine());
    }

    public IEnumerator SelectCarCoroutine()
    {
        bool done = false;
        yield return StartCoroutine(pm.SavePreferencesToFilePM());
        //SavePrefs();
        if (PlayerPrefs.HasKey("TutorialUnlock"))
            SceneManager.LoadScene(1);
        else
        {
            PlayerPrefs.SetInt("level", -1);
            SceneManager.LoadScene(2);
        }

        done = true;
        yield return new WaitWhile(() => done == false);
    }

    // Disabling music and effect sliders for now
    public void LoadPrefs()
    {

        carIndex = PlayerPrefs.GetInt("car", 0); //pm.carsettings;

        carText.text = carNames[carIndex];

        this.transform.position = this.transform.position - (move * carIndex);
        UpdateCarName();
        //DontMoveHat(move * carIndex);

        try
        {          
            pm.oldcolors = new Color[2,7];
            for (int col = 0; col < 2; col++)
            {
                for (int row = 0; row < 7; row++)
                {
                    pm.oldcolors[col, row] = cars[row].GetComponentInChildren<Renderer>().materials[col].color;
                }
            }
            pm.SetCarDefaultSettingsData();
            if (PlayerPrefs.GetInt("custom") == 1) pm.ModifyCar();
        }
        catch (Exception e)
        {
            Debug.Log("Failed with error: " + e);
        }

        float _masterVolume = PlayerPrefs.GetFloat("masterVolume", 0.1f);

        _volumeSlider.value = _masterVolume;

        mixer.SetFloat("masterVolume", Mathf.Log10(_masterVolume) * 20);

        _qualityIndex = PlayerPrefs.GetInt("Quality", 0);

        ChangeQuality(_qualityIndex);
    }

    public void OnDestroy()
    {
        PlayerPrefs.Save();
    }

    public void Quit()
    {
        Application.Quit();
    }
}