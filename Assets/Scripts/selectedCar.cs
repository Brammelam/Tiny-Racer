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
    public Vector3 move = new Vector3(100f, 0, 0);
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


    private void Awake()
    {     
        carIndex = pm.currentCar;
        cars = GameObject.FindGameObjectsWithTag("car").ToList();
        hats = GameObject.FindGameObjectsWithTag("hatButton").ToList();
        cars = cars.OrderBy(car => car.name).ToList();
        GameObject.FindGameObjectWithTag("Ambient").GetComponent<AmbientClass>().StopAmbientMusic();
        GameObject.FindGameObjectWithTag("engineNoise").GetComponent<CarMusicClass>().StopCarMusic();
        GameObject.FindGameObjectWithTag("City").GetComponent<CityScript>().StopCityMusic();
        GameObject.FindGameObjectWithTag("Snow").GetComponent<SnowScript>().StopSnowMusic();
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicClass>().PlayMusic();

        pm = GameObject.FindObjectOfType<PlayerManager>();

        pos1 = this.transform.position;
        pos2 = this.transform.position - (move * 6);
        
        carText.text = carNames[pm.currentCar];
    }


    private void OnApplicationQuit()
    {
        SavePrefs();
    }


    public void Unlock()
    {
        int i = pm.currentCar + 1;
        string s = i.ToString();
        string input = "getCar" + s;

        pm.TriggerEvent(input);
        pm.unlockedCars.Add("gotcar" + i.ToString());
        change = transform.GetChild(pm.currentCar).gameObject.GetComponent<changeMaterial>();
        change.Unlock();
        
    }


    public void ChangeQuality(int qualityIndex)
    {
        qualityDropdown.value = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
        Debug.Log("Set quality to: " + qualityIndex);
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

    public void addNewNode(int _index)
    {
        hatIndex = _index;

        if (_index == 0)
            _hat = Instantiate(Resources.Load("tophat") as GameObject);
        if (_index == 1)
            _hat = Instantiate(Resources.Load("crown") as GameObject);
        if (_index == 2)
            _hat = Instantiate(Resources.Load("party") as GameObject);
        if (_index != 3)
        {
            _hat.transform.SetParent(this.gameObject.transform.GetChild(pm.currentCar));

            _hat.transform.localRotation = new Quaternion(0, 0, 0, 0);
            _hat.transform.localPosition = new Vector3(0, 1.4f, -0.3f);
            if (pm.currentCar == 2) // adjust for SUV
            {
                _hat.transform.localPosition = new Vector3(0, 1.6f, -0.3f);
                if (hatIndex == 2)
                    _hat.transform.localPosition -= new Vector3(0, -0.2f, 0);
            }
        }
        SavePrefs();
    }

    public void removeNewNode(int _index)
    {
        Destroy(_hat);
        hatIndex = 3;
        SavePrefs();
    }

    public void removeNewNodeButton()
    {
        removeNewNode(hatIndex);
    }

    private void Update()
    {
        if (pm == null) pm = GameObject.FindObjectOfType<PlayerManager>();

        if (!settings)
        {
            if (Input.GetKeyUp("a") || Input.GetKeyUp("left"))
            {
                PreviousCar();
                removeNewNodeButton();
            }
            if (Input.GetKeyUp("d") || Input.GetKeyUp("right"))
            {
                NextCar();
                removeNewNodeButton();
            }
            if (Input.GetKeyUp("space") || Input.GetKeyUp("enter"))
            {               
                SelectCar();
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
        SavePrefs();
        carText.text = carNames[pm.currentCar];
        addNewNode(hatIndex);
        carChangeEvent = true;

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

    public void NextCar()
    {
        if (pm.currentCar < 6)
        {
            this.transform.position = this.transform.position - move;
            DontMoveHat(move);
            pm.currentCar += 1;            
        }
        else
        {
            this.transform.position = pos1;
            DontMoveHat(-6 * move);
            pm.currentCar = 0;
        }
    }

    public void PreviousCar()
    {
        if (pm.currentCar > 0)
        {
            this.transform.position = this.transform.position + move;
            DontMoveHat(-move);
            pm.currentCar -= 1;
        }
        else
        {
            this.transform.position = pos2;
            DontMoveHat(6 * move);
            pm.currentCar = 6;
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
        pm.carsettings.CurrentCar = pm.currentCar;
        pm.carsettings.CurrentHat = hatIndex;
        if(pm.unlockedCars.Contains("TutorialUnlock"))
            SceneManager.LoadScene(1);
        else
        {
            pm.currentLevel = 9;
            SceneManager.LoadScene(11);
        }

        done = true;
        yield return new WaitWhile(() => done == false);
    }

    public void SavePrefs()
    {
        /*
        PlayerPrefs.SetInt(carKey, carIndex);
        PlayerPrefs.SetInt(hatKey, hatIndex);
        PlayerPrefs.Save();
        */
    }

    // Disabling music and effect sliders for now
    public void LoadPrefs()
    {
        hatIndex = carsettings.CurrentHat;
        carIndex = carsettings.CurrentCar;
        pm.currentCar = carIndex;
        pm.currentHat = hatIndex;

        this.transform.position = this.transform.position - (move * carIndex);
        DontMoveHat(move * carIndex);
        addNewNode(hatIndex);

        try
        {
            if (pm == null) pm = GameObject.FindObjectOfType<PlayerManager>();
            pm.oldcolors = new Color[2,7];
            for (int col = 0; col < 2; col++)
            {
                for (int row = 0; row < 7; row++)
                {
                    pm.oldcolors[col, row] = cars[row].GetComponentInChildren<Renderer>().materials[col].color;
                }
            }
            pm.SetCarDefaultSettingsData();
        }
        catch (Exception e)
        {
            Debug.Log("Failed with error: " + e);
        }

        float _masterVolume = PlayerPrefs.GetFloat("masterVolume", 0.2f);

        _volumeSlider.value = _masterVolume;

        mixer.SetFloat("masterVolume", Mathf.Log10(_masterVolume) * 20);

        _qualityIndex = PlayerPrefs.GetInt("Quality", 0);

        ChangeQuality(_qualityIndex);
    }


    public void Quit()
    {
        Application.Quit();
    }
}