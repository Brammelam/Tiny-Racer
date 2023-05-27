using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class titleCar : MonoBehaviour
{
    public List<GameObject> cars = new List<GameObject>(7);
    public GameObject ActiveCar;
    public int whatCar;
    const string carKey = "Selected Car";

    // Start is called before the first frame update
    void Start()
    {
        LoadPrefs();
       // GameObject ActiveCar = Instantiate(cars[whatCar]);
        //ActiveCar.transform.localScale = new Vector3(20f, 20f, 20f);
        //ActiveCar.transform.rotation *= Quaternion.Euler(0, 90f, -40f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadPrefs()
    {
        int selectedCar = PlayerPrefs.GetInt(carKey, 0);
        whatCar = selectedCar;
    }
}
