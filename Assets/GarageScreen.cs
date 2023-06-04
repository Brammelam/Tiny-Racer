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
        if (!carsettings.CustomCar)
        {
            int c = pm.currentCar;
            carsettings.BodyColor[0] = pm.oldcolors[0, c].r;
            carsettings.BodyColor[1] = pm.oldcolors[0, c].g;
            carsettings.BodyColor[2] = pm.oldcolors[0, c].b;
            carsettings.WindowColor[0] = pm.oldcolors[1, c].r;
            carsettings.WindowColor[1] = pm.oldcolors[1, c].g;
            carsettings.WindowColor[2] = pm.oldcolors[1, c].b;
        }

        Color.RGBToHSV(new Color(carsettings.BodyColor[0], carsettings.BodyColor[1], carsettings.BodyColor[2]), out float bodyH, out float bodyS, out float bodyV);
        sliderBodyHue.value = bodyH;
        sliderBodySaturation.value = bodyS;
        sliderBodyValue.value = bodyV;

        Color.RGBToHSV(new Color(carsettings.WindowColor[0], carsettings.WindowColor[1], carsettings.WindowColor[2]), out float windowH, out float windowS, out float windowV);
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
        carsettings.CustomCar = true;
    }

    public void SetBodyColor()
    {
        didyoufuckwiththeslidersbody = true;
        Material[] mat = materials.ToArray();
        bodyColor = Color.HSVToRGB(sliderBodyHue.value, sliderBodySaturation.value, sliderBodyValue.value);
        mat[0].SetColor("_Color", bodyColor);
        carRenderers[pm.currentCar].materials = mat;
        carsettings.CustomCar = true;
    }

    public void SavePreferences()
    {
        StartCoroutine(SavePreferencesToFile());
    }

    public IEnumerator SavePreferencesToFile()
    {
        bool done = false;
        int c = pm.currentCar;
        carsettings.CurrentCar = pm.currentCar;
        carsettings.CurrentHat = pm.currentHat;

        if (!didyoufuckwiththeslidersbody && !didyoufuckwiththesliderswindow)
        {

        } else
        {
            if (didyoufuckwiththeslidersbody)
            {
                carsettings.BodyColor[0] = bodyColor.r;
                carsettings.BodyColor[1] = bodyColor.g;
                carsettings.BodyColor[2] = bodyColor.b;
            }
            else
            {

            }
            if (didyoufuckwiththesliderswindow)
            {
                carsettings.WindowColor[0] = windowColor.r;
                carsettings.WindowColor[1] = windowColor.g;
                carsettings.WindowColor[2] = windowColor.b;
            }
            else
            {

            }
        }

        
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

        writer.WriteLine(selectedCar.carIndex);
        writer.WriteLine(selectedCar.hatIndex);
        writer.WriteLine(carsettings.CustomCar);
        writer.WriteLine(pm.currentLevel);
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
        });

        yield return new WaitWhile(() => done == false);
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
        });
        didyoufuckwiththeslidersbody = false;
        didyoufuckwiththesliderswindow = false;
        pm.ModifyCar();
        pm.LeaveGarage();
        yield return new WaitWhile(() => done == false);
    }

    public void ResetCarInGarage()
    {
        int c = pm.currentCar;

        renderer.materials[0].color = pm.oldcolors[0,c];
        renderer.materials[1].color = pm.oldcolors[1,c];

        carsettings.CustomCar = false;
        SetSliders();
        pm.ResetOriginalCar();
    }
}
