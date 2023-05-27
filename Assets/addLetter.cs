using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class addLetter : MonoBehaviour
{
    public TMP_InputField playernameInputfield;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(AddLetter);
       
        if (playernameInputfield == null)
        {
            playernameInputfield = GameObject.FindWithTag("inputfieldwelcome").GetComponent<TMP_InputField>();
        }
    }

    public void AddLetter()
    {
        if (this.name != "del")
            playernameInputfield.text += this.name;
        else
            if (playernameInputfield.text != "")
                playernameInputfield.text = playernameInputfield.text.Remove(playernameInputfield.text.Length - 1);
    }
}
