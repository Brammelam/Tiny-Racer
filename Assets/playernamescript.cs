using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playernamescript : MonoBehaviour
{

    public TMP_Text inputText;
    public string playerName;
    public GameObject playerInput;
    //public highscorescript hs;

    public void Awake()
    {
        /*if (hs.username.Length > 1)
        {
            hs.entered = 1;
            this.enabled = false;
        }
        else TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        */
    }

    public void OpenTouchScreen()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }
    // Update is called once per frame
    public void Update()
    {
        /*
        if (hs.username.Length <= 1)
        {
            hs.entered = 0;
        }
        else
        {
            hs.entered = 1;
        }


        hs.username = inputText.text;
        */


    }

}
