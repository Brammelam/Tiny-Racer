using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class checkName : MonoBehaviour
{
    [SerializeField]
    public GameObject textMessage;
    [SerializeField]
    public TMP_InputField input;
    public PlayerManager pm;

    public void GetText()
    {

    }

    public void UpdateLog()
    {
        textMessage.SetActive(true);
        if (input.text == "")
            textMessage.GetComponent<Text>().text = "Name can not be empty!";
        else
        {
            pm.SetPlayerName();
            textMessage.GetComponent<Text>().text = "Name changed!";
        }
    }
    
}
