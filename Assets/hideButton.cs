using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hideButton : MonoBehaviour
{
    public selectedCar sc;
    public PlayerManager pm;
    public GameObject button;
    public GameObject playButton;
    public Text unlockText;
    public int unlockedindex;
    // Start is called before the first frame update
    void Start()
    {

    }



    // Update is called once per frame
    void FixedUpdate()
    {
        int i = pm.currentCar+1;
        if (button == null) button = GameObject.FindGameObjectWithTag("unlockbutton");
        if (pm.currentCar != 0)
        {
            
            // You have claimed the unlocked car
            if (pm.unlockedCars.Contains("gotcar" + i))
            {
                unlockText.text = "";

                var tempColor = button.GetComponent<Image>().color;
                tempColor.a = 0f;
                button.GetComponent<Image>().color = tempColor;
                button.GetComponentInChildren<Text>().text = "";
                button.SetActive(false);
                playButton.SetActive(true);
            }

            // You are eligble to unlock the car, but have not done so yet
            else if (pm.unlockedCars.Contains("car" + i) && !(pm.unlockedCars.Contains("gotcar" + i)))
            {
                unlockText.text = "";
                playButton.SetActive(false);
                button.SetActive(true);
                button.GetComponent<Image>().color = Color.green;
                button.GetComponentInChildren<Text>().text = "UNLOCK";
            }

            // You are not eligble to unlock the car
            else
            {
                string _complete = "Complete level 1 - " + i;
                unlockText.text = _complete;
                var tempColor = button.GetComponent<Image>().color;
                tempColor.a = 0f;
                button.SetActive(false);
                playButton.SetActive(false);
            }
        } else
        {
            unlockText.text = "";

            var tempColor = button.GetComponent<Image>().color;
            tempColor.a = 0f;
            button.GetComponent<Image>().color = tempColor;
            button.GetComponentInChildren<Text>().text = "";
            button.SetActive(false);
            playButton.SetActive(true);
        }

    }
}
