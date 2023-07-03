using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LootLocker.Requests;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Linq;


public class TaxiDialog : MonoBehaviour
{
    private static TaxiDialog instance;
    [SerializeField] private GameObject taxiPrefab;
    [SerializeField] private Text dialogText;
    [SerializeField] private Animator anim;


    public void SetAnimationDetails(string dialogType)
    {
        
        Text[] textComponents = GetComponentsInChildren<Text>(true);

        foreach (Text text in textComponents)
        {
            if (text.gameObject.name == "dialogText") dialogText = text;
        }

        int counter = Random.Range(1, 6);
        string dialog = dialogType + counter;

        // Some achievements will have step-progress like 5-10-15 which will be implemented later
        switch (dialog)
        {
            case "a1":
                dialogText.text = "To the city! Stat!";
                break;
            case "a2":
                dialogText.text = "I need to reach my destination, taxiperson";
                break;
            case "a3":
                dialogText.text = "Do you know the way?";
                break;
            case "a4":
                dialogText.text = "Step on it, driver!";
                break;
            case "a5":
                dialogText.text = "I'm in a hurry, let's go!!";
                break;
            case "a6":
                dialogText.text = "Can we make it in time?";
                break;
            case "b1":
                dialogText.text = "Oh no, my eggplants!";
                break;
            case "b2":
                dialogText.text = "I don't think cars should be upside down!";
                break;
            case "b3":
                dialogText.text = "Bruh, at least I'm wearing a seatbelt!";
                break;
            case "b4":
                dialogText.text = "I hope my insurance covers this!";
                break;
            case "b5":
                dialogText.text = "This is not how I imagined my ride!";
                break;
            case "b6":
                dialogText.text = "Ahh, my venti pink drink! It's everywhere!";
                break;
            case "c1":
                dialogText.text = "Well that was fun, thank you!";
                break;
            case "c2":
                dialogText.text = "Good heavens! That was.. an experience!";
                break;
            case "c3":
                dialogText.text = "We made it! Do you take cash?";
                break;
            case "c4":
                dialogText.text = "How much do I owe you?";
                break;
            case "c5":
                dialogText.text = "We're finally here! What a relief!";
                break;
            case "c6":
                dialogText.text = "Here's fine, thank you!";
                break;

        }

        PlayAnimationClip();
    }

    private void PlayAnimationClip()
    {
        anim.Play("taxidialoganimation");
    }
}