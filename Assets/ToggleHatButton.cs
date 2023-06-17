using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHatButton : MonoBehaviour
{

    private Image buttonImage;
    [SerializeField] private Sprite lockedSprite;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
        if (!PlayerPrefs.HasKey(this.name))
        {
            Debug.Log("Player has not unlocked: " + this.name);
            GetComponent<Button>().interactable = false;
            buttonImage.sprite = lockedSprite;
        }
        
    }
}

