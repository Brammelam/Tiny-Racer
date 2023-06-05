using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hideButton : MonoBehaviour
{
    public selectedCar sc;
    public PlayerManager pm;
    [SerializeField]
    public int car;
    public GameObject unlockButton;
    public GameObject playButton;
    public Button unlockButtonButton;
    public Text unlockText;
    public int unlockedindex;
    
    // Start is called before the first frame update
    void Start()
    {
        pm = GameObject.FindObjectOfType<PlayerManager>();
        car = PlayerPrefs.GetInt("car", 0);
        CheckUnlock(car);
    }


    public void TriggerButton()
    {
        unlockButtonButton.interactable = false;
        string _gotcar = "gotcar" + (pm.currentCar + 1);
        PlayerPrefs.SetString(_gotcar, "true");
        StartCoroutine(ChangeButton());
    }

    IEnumerator ChangeButton()
    {
        yield return new WaitForSeconds(0.2f);
        unlockText.text = "";

        var tempColor = unlockButton.GetComponent<Image>().color;
        tempColor.a = 0f;
        unlockButton.GetComponent<Image>().color = tempColor;
        unlockButton.GetComponentInChildren<Text>().text = "";
        unlockButton.SetActive(false);
        playButton.SetActive(true);

    }


    // Update is called once per frame
    public void CheckUnlock(int _carIndex)
    {
        car = _carIndex;
        int i = _carIndex + 1; // cars start at 1
        string gotcar = "gotcar" + i;
        string carindex = "car" + i;

        if (unlockButton == null) { unlockButton = GameObject.FindGameObjectWithTag("unlockbutton"); }

        Debug.Log("Here is going fucky???" + _carIndex);
        // Car 0 is active by default
        if (_carIndex == 0)
        {
            unlockText.text = "";
            var tempColor = unlockButton.GetComponent<Image>().color;
            tempColor.a = 0f;
            unlockButton.GetComponent<Image>().color = tempColor;
            unlockButton.GetComponentInChildren<Text>().text = "";
            unlockButton.SetActive(false);
            playButton.SetActive(true);

        }
        else
        {
            // You have claimed the unlocked car
            if (PlayerPrefs.HasKey(carindex) && (PlayerPrefs.HasKey(gotcar)))
            {
                unlockText.text = "";

                var tempColor = unlockButton.GetComponent<Image>().color;
                tempColor.a = 0f;
                unlockButton.GetComponent<Image>().color = tempColor;
                unlockButton.GetComponentInChildren<Text>().text = "";
                unlockButton.SetActive(false);
                playButton.SetActive(true);
            }

            // You are eligble to unlock the car, but have not done so yet
            else if (PlayerPrefs.HasKey(carindex) && (!PlayerPrefs.HasKey(gotcar)))
            {
                unlockText.text = "";
                playButton.SetActive(false);
                unlockButton.SetActive(true);
                unlockButton.GetComponent<Image>().color = Color.green;
                unlockButton.GetComponentInChildren<Text>().text = "UNLOCK";
            }

            // You are not eligble to unlock the car
            else
            {
                string _complete = "Complete level 1 - " + i;
                unlockText.text = _complete;
                var tempColor = unlockButton.GetComponent<Image>().color;
                tempColor.a = 0f;
                unlockButton.SetActive(false);
                playButton.SetActive(false);
            }
        }
    }
}
