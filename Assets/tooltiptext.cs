using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tooltiptext : MonoBehaviour
{
    private string tooltipText;
    void Awake()
    {
        int i = Random.Range(0, 5);        

        switch (i)
        {
            case 0:
                this.GetComponent<Text>().text = "Don't forget to customize your car in the Garage!";
                break;
            case 1:
                this.GetComponent<Text>().text = "Too much speed through corners will flip your car!";
                break;
            case 2:
                this.GetComponent<Text>().text = "Unlock new cars by completing different tracks!";
                break;
            case 3:
                this.GetComponent<Text>().text = "Watch out for animals crossing the road!";
                break;
            case 4:
                this.GetComponent<Text>().text = "Practice makes perfect!";
                break;
            case 5:
                this.GetComponent<Text>().text = "Become a racing legend by claiming the top spot on the leaderboard!";
                break;
        }
    }
}
