using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHatButton : MonoBehaviour
{

    private RawImage buttonImage;
    [SerializeField] private Texture2D lockedSprite;

    private void Start()
    {
        buttonImage = GetComponent<RawImage>();
        if (!PlayerPrefs.HasKey(this.name))
        {           
            GetComponent<Button>().interactable = false;
            buttonImage.texture = lockedSprite;
        }        
    }
}

