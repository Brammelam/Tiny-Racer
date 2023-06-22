using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContentManager : MonoBehaviour
{
    [SerializeField] public List<Texture2D> imageList;

    private static ContentManager instance;

    private Text foundTextComponent;
    private RawImage foundImageComponent;

    private void Awake()
    {        

        // Load the relevant items
        FindComponents();
        SetComponents();

    }

    private void FindComponents()
    {
        // Find the Text component
        foundTextComponent = GetComponentInChildren<Text>();

        if (foundTextComponent == null)
        {
            Debug.LogWarning("Text component not found in children");
        }

        // Find the Image component
        foundImageComponent = GetComponentInChildren<RawImage>();

        if (foundImageComponent == null)
        {
            Debug.LogWarning("Image component not found in children");
        }
    }

    private void SetComponents()
    {
        int level = SceneManager.GetActiveScene().buildIndex;

        foundTextComponent.text = GetTextForScene(level);
        foundImageComponent.texture = GetImageForScene(level);
    }

    // Call this method to get a specific image from the image list
    public Texture2D GetImageForScene(int sceneIndex)
    {
        switch (sceneIndex)
        {
            case 3:
                return imageList[0];
            case 4:
                return imageList[1];
            case 6:
                return imageList[2];
            case 8:
                return imageList[3];
            case 10:
                return imageList[4];
            case 7:
                return imageList[5];
            case 11:
                return imageList[6];
            case 5:
                return imageList[7];
            case 12:
                return imageList[8];
            case 13:
                return imageList[9];
            default:
                return null;
        }
    }

    public string GetTextForScene(int sceneIndex)
    {
        string text;

        switch (sceneIndex)
        {
            case 3:
                text = "Collected the Top Hat! Race in elegance and style!";
                break;
            case 4:
                text = "Collected the Crown! Rule the track with regal flair!";
                break;
            case 6:
                text = "Got the Party Hat! Bring the celebration to the race!";
                break;
            case 8:
                text = "Picked up the Cactus hat! Watch out for the prickly racer!";
                break;
            case 10:
                text = "Earned the Halo hat! Radiate angelic vibes as you race!";
                break;
            case 7:
                text = "Grabbed the R-Key hat! Embrace your inner gamer!";
                break;
            case 11:
                text = "Snagged the Antenna hat! Tune into maximum racing vibes!";
                break;
            case 5:
                text = "Rock the Mohawk hat! Stand out with rebellious style!";
                break;
            case 12:
                text = "Crashed into enough cows to earn the Cow hat! Moo-velous addition to your race!";
                break;
            case 13:
                text = "Collided with so many sheep, you found the Sheep hat! Embrace the fluffiness on the track!";
                break;
            case 19:
                text = "";
                break;
            default:
                text = "";
                break;
        }

        return text;
    }
}
