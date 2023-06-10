using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LootLocker.Requests;
using System.Collections;

public class GarageScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject car;
    [SerializeField]
    PlayerManager pm;
    [SerializeField]
    selectedCar selectedCar;
    [SerializeField]
    GameObject cam2;
    [SerializeField]
    GameObject cam1;
    [SerializeField]
    GameObject placeholder;
    [SerializeField]
    private List<Material> materials;
    [SerializeField]
    public MeshRenderer renderer;
    [SerializeField]
    private MeshRenderer[] carRenderers;

    [SerializeField]
    public Slider sliderBodyHue;
    [SerializeField]
    public Slider sliderBodySaturation;
    [SerializeField]
    public Slider sliderBodyValue;
    [SerializeField]
    public Slider sliderWindowHue;
    [SerializeField]
    public Slider sliderWindowSaturation;
    [SerializeField]
    public Slider sliderWindowValue;
    [SerializeField]
    public Color bodyColor;
    [SerializeField]
    public Color windowColor;
    [SerializeField]
    private Color oldbodyColor;
    [SerializeField]
    private Color oldwindowColor;
    [SerializeField]
    private Material[] materialColors;
    [SerializeField]
    CarsettingsSO carsettings;

    private bool didyoufuckwiththeslidersbody, didyoufuckwiththesliderswindow = false;

    public void Awake()
    {
        carsettings = pm.carsettings;
    }

    public void Start()
    {
        //Adds a listener to the main slider and invokes a method when the value changes.
        sliderBodyHue.onValueChanged.AddListener(delegate { SetBodyColor(); });
        sliderBodySaturation.onValueChanged.AddListener(delegate { SetBodyColor(); });
        sliderBodyValue.onValueChanged.AddListener(delegate { SetBodyColor(); });
        sliderWindowHue.onValueChanged.AddListener(delegate { SetWindowColor(); });
        sliderWindowSaturation.onValueChanged.AddListener(delegate { SetWindowColor(); });
        sliderWindowValue.onValueChanged.AddListener(delegate { SetWindowColor(); });       
    }

    public void SetCar(int carIndex)
    {      
        if (car != null)
        {
            Destroy(car);  // Destroy the existing car if it already exists
        }

        var _count = selectedCar.cars.Count;
        carRenderers = new MeshRenderer[_count];

        List<MeshRenderer> renderers = new List<MeshRenderer>();
        for (int i = 0; i < _count; i++)
        {
            Transform childTransform = selectedCar.cars[i].GetComponentInChildren<MeshRenderer>().transform;
            MeshRenderer childRenderer = childTransform.GetComponent<MeshRenderer>();

            if (childRenderer != null)
            {
                carRenderers[i] = childRenderer;
            }
        }

        car = Instantiate(selectedCar.cars[carIndex], placeholder.transform);
        car.transform.localPosition = new Vector3(0,0,0);

        car.GetComponent<horn>().enabled = false;
        car.GetComponent<changeMaterial>().enabled = false;
        renderer = car.GetComponentInChildren<MeshRenderer>();
        materials = new List<Material>(renderer.materials);
        SetSliders();

        // Store the MeshRenderer of the new car in the carRenderers array
        carRenderers[pm.currentCar] = renderer;
    }

    public void SetSliders()
    {
        if (!(PlayerPrefs.GetInt("custom") == 1))
        {
            int c = pm.currentCar;

            PlayerPrefs.SetFloat("b1", pm.oldcolors[0, c].r);
            PlayerPrefs.SetFloat("b2", pm.oldcolors[0, c].g);
            PlayerPrefs.SetFloat("b3", pm.oldcolors[0, c].b);
            PlayerPrefs.SetFloat("w1", pm.oldcolors[1, c].r);
            PlayerPrefs.SetFloat("w2", pm.oldcolors[1, c].g);
            PlayerPrefs.SetFloat("w3", pm.oldcolors[1, c].b);

            PlayerPrefs.Save();
        }

        float b1 = PlayerPrefs.GetFloat("b1");
        float b2 = PlayerPrefs.GetFloat("b2");
        float b3 = PlayerPrefs.GetFloat("b3");
        float w1 = PlayerPrefs.GetFloat("w1");
        float w2 = PlayerPrefs.GetFloat("w2");
        float w3 = PlayerPrefs.GetFloat("w3");

        Color.RGBToHSV(new Color(b1,b2,b3), out float bodyH, out float bodyS, out float bodyV);
        Color.RGBToHSV(new Color(w1,w2,w3), out float windowH, out float windowS, out float windowV);
        
        sliderBodyHue.value = bodyH;
        sliderBodySaturation.value = bodyS;
        sliderBodyValue.value = bodyV;

        sliderWindowHue.value = windowH;
        sliderWindowSaturation.value = windowS;
        sliderWindowValue.value = windowV;     

    }

    public void DestroyCar()
    {
        Destroy(carRenderers[pm.currentCar].gameObject);
    }

    public void SetWindowColor()
    {
        didyoufuckwiththesliderswindow = true;
        Material[] mat = materials.ToArray();
        windowColor = Color.HSVToRGB(sliderWindowHue.value, sliderWindowSaturation.value, sliderWindowValue.value);
        mat[1].SetColor("_Color", windowColor);
        carRenderers[pm.currentCar].materials = mat;
        //carsettings.CustomCar = true;
        PlayerPrefs.SetInt("custom", 1);
        PlayerPrefs.Save();
    }

    public void SetBodyColor()
    {
        didyoufuckwiththeslidersbody = true;
        Material[] mat = materials.ToArray();
        bodyColor = Color.HSVToRGB(sliderBodyHue.value, sliderBodySaturation.value, sliderBodyValue.value);
        mat[0].SetColor("_Color", bodyColor);
        carRenderers[pm.currentCar].materials = mat;
        //carsettings.CustomCar = true;
        PlayerPrefs.SetInt("custom", 1);
        PlayerPrefs.Save();
    }

    public void SavePreferences()
    {
        StartCoroutine(SavePreferencesToFile());
    }

    public IEnumerator SavePreferencesToFile()
    {
        bool doneDeletingPlayerFile = false;
        int c = pm.currentCar;

        if (!didyoufuckwiththeslidersbody && !didyoufuckwiththesliderswindow)
        {

        } else
        {
            if (didyoufuckwiththeslidersbody)
            {
                PlayerPrefs.SetFloat("b1", bodyColor.r);  //carsettings.BodyColor[0] = bodyColor.r;
                PlayerPrefs.SetFloat("b2", bodyColor.g);  //carsettings.BodyColor[1] = bodyColor.g;
                PlayerPrefs.SetFloat("b3", bodyColor.b);  //carsettings.BodyColor[2] = bodyColor.b;
            }
            if (didyoufuckwiththesliderswindow)
            {
                PlayerPrefs.SetFloat("w1", windowColor.r);  //carsettings.WindowColor[0] = windowColor.r;
                PlayerPrefs.SetFloat("w2", windowColor.g);  //carsettings.WindowColor[1] = windowColor.g;
                PlayerPrefs.SetFloat("w3", windowColor.b);  //carsettings.WindowColor[2] = windowColor.b;
            }
        }
        
        string filePath = Path.Combine(Application.persistentDataPath + "/carsettings.txt");
        StreamWriter writer = new StreamWriter(filePath, false);

        int _settingsKey = PlayerPrefs.GetInt("settings");
        // check if settings exists, if so, delete previous settings
        if (PlayerPrefs.HasKey("settings")) { 
            LootLockerSDKManager.DeletePlayerFile(_settingsKey, (response) =>
            {
                if (response.success)
                {
                    doneDeletingPlayerFile = true;
                }
                else
                {
                    Debug.Log("Failed deleting Playerfile settings in Garage!" + response.Error);
                    doneDeletingPlayerFile = true;
                }

            });
            yield return new WaitUntil(() => doneDeletingPlayerFile == true);
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
        writer.WriteLine(PlayerPrefs.GetInt("level"));  //writer.WriteLine(leaderboardSO.CurrentLevel);
        writer.Close();

        bool doneUploadingPlayerFile = false;
        LootLockerSDKManager.UploadPlayerFile(filePath, "settings", true, (response) =>
        {
            if (response.success)
            {             
                carsettings.SettingsKey = response.id.ToString();
                PlayerPrefs.SetInt("settings", response.id);
                StartCoroutine(UploadCarsettingsKey());
                doneUploadingPlayerFile = true;
            }

            else
            {
                Debug.Log("Failed uploading settings!  " + response.Error);
                doneUploadingPlayerFile = true;
            }
        });

        yield return new WaitUntil(() => doneUploadingPlayerFile == true);
    }

    public IEnumerator UploadCarsettingsKey()
    {
        bool done = false;
        string key = "9999";
        string _settingsKey = PlayerPrefs.GetInt("settings").ToString();
        string pi = carsettings.SettingsKey.ToString();

        LootLockerSDKManager.UpdateOrCreateKeyValue(key, _settingsKey, (response) =>
        {
            if (response.success)
                done = true;
            else
            {
                Debug.Log("There was an error uploading the carsettings key to LootLocker!");
                done = true;
            }
        });
        didyoufuckwiththeslidersbody = false;
        didyoufuckwiththesliderswindow = false;
        pm.ModifyCar();
        pm.LeaveGarage();
        yield return new WaitUntil(() => done == true);
    }

    public void ResetCarInGarage()
    {
        int c = pm.currentCar;

        renderer.materials[0].color = pm.oldcolors[0,c];
        renderer.materials[1].color = pm.oldcolors[1,c];

        PlayerPrefs.SetInt("custom", 0);
        SetSliders();
        pm.ResetOriginalCar();
    }
}
