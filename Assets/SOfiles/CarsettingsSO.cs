using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CarsettingsSO : ScriptableObject
{
    [SerializeField]
    private List<float> bodyColor;
    [SerializeField]
    private List<float> windowColor;
    [SerializeField]
    private int currentCar;
    [SerializeField]
    private int currentHat;
    [SerializeField]
    private string settingsKey;
    [SerializeField]
    private bool customCar;





    public List<float> BodyColor
    {
        get { return bodyColor; }
        set { bodyColor = value; }
    }    
    public List<float> WindowColor
    {
        get { return windowColor; }
        set { windowColor = value; }
    }
    public int CurrentCar
    {
        get { return currentCar; }
        set { currentCar = value; }
    }
    public int CurrentHat
    {
        get { return currentHat; }
        set { currentHat = value; }
    }
    public string SettingsKey
    {
        get { return settingsKey; }
        set { settingsKey = value; }
    }
    public bool CustomCar
    {
        get { return customCar; }
        set { customCar = value; }
    }




}
