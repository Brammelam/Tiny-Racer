using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatManager : MonoBehaviour
{
    [SerializeField]
    private selectedCar sc;
    [SerializeField]
    private List<GameObject> hatPrefabs = new List<GameObject>();
    [SerializeField]
    private List<GameObject> currentHats = new List<GameObject>();

    private void Start()
    {
        SetHat();
    }

    public void OnButtonClick(int hatIndex)
    {
        int car = PlayerPrefs.GetInt("car", 0);

        // Check if a hat already exists on the car
        bool hatExists = HatExistsOnCar();

        if (hatExists)
        {
            RemoveHat();
            int currentHat = PlayerPrefs.GetInt("hatindex");
            if (currentHat == hatIndex) // If player is already wearing this hat, disable and return (allows removing the hat)
            {
                PlayerPrefs.SetInt("hatindex", -1);
                PlayerPrefs.SetString("hat", "no");
                return;
            }
           
        }

        if (hatIndex >= 0 && hatIndex < hatPrefabs.Count)
        {
            GameObject hatPrefab = hatPrefabs[hatIndex];

            // Instantiate the hat prefab and assign it to the car object
            GameObject hat = Instantiate(hatPrefab);
            hat.transform.SetParent(sc.cars[car].transform);

            hat.transform.localRotation = Quaternion.identity;
            hat.transform.localPosition = new Vector3(0, 1.4f, -0.3f);
            if (car == 2) // adjust for SUV
            {
                hat.transform.localPosition = new Vector3(0, 1.6f, -0.3f);
                if (hatPrefab.name == "party")
                    hat.transform.localPosition -= new Vector3(0, -0.2f, 0);
            }

            currentHats.Add(hat);
            string hatName = hatPrefab.name.Replace("(Clone)", "");

            PlayerPrefs.SetString("hat", hatName);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("hatindex", hatIndex);
            PlayerPrefs.Save();
        }
    }

    public void RemoveHat()
    {
        foreach (GameObject hat in currentHats)
        {
            Destroy(hat);
        }

        currentHats.Clear();
    }

    private bool HatExistsOnCar()
    {
        int car = sc.carIndex;
        GameObject carObject = sc.cars[car];

        // Iterate through the car's children
        for (int i = carObject.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = carObject.transform.GetChild(i);

            // Check if the child object has a HatManager component
            ReturnHatName hatIdentifier = child.GetComponent<ReturnHatName>();
            if (hatIdentifier != null)
            {
                // Hat already exists on the car
                return true;
            }
        }

        return false;
    }

    private void SetHat()
    {
        int car = sc.carIndex;
        int savedHatIndex = PlayerPrefs.GetInt("hatindex", -1);

        if (savedHatIndex >= 0 && savedHatIndex < hatPrefabs.Count)
        {
            GameObject hatPrefab = hatPrefabs[savedHatIndex];

            // Instantiate the hat prefab and assign it to the car object
            GameObject hat = Instantiate(hatPrefab);
            hat.transform.SetParent(sc.cars[car].transform);

            hat.transform.localRotation = new Quaternion(0, 0, 0, 0);
            hat.transform.localPosition = new Vector3(0, 1.4f, -0.3f);
            if (car == 2) // adjust for SUV
            {
                hat.transform.localPosition = new Vector3(0, 1.6f, -0.3f);
                if (this.name == "party")
                    hat.transform.localPosition -= new Vector3(0, -0.2f, 0);
            }

            currentHats.Add(hat);
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}