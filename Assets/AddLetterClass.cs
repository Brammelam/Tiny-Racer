using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AddLetterClass : MonoBehaviour
{
    [Header("Variables")]
    public int assignedInputFieldIndex;
    public TMP_InputField playernameInputField;

    [SerializeField]
    private bool isShiftPressed;

    private void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(AddLetter);
    }

    public int GetAssignedInputFieldIndex()
    {
        return assignedInputFieldIndex;
    }

    public void SetInputField(TMP_InputField inputField)
    {
        playernameInputField = inputField;
    }

    public void UpdateTextCasing(bool isShiftPressed)
    {
        this.isShiftPressed = isShiftPressed;
        string _text = this.GetComponentInChildren<Text>().text;

        if (isShiftPressed)
        {
            
            if (name != null && name != "del" && name != "DEL" && name != "shift" && name != "SHIFT")
            {
                name = name.ToUpper();
                this.GetComponentInChildren<Text>().text = _text.ToUpper();
            }
        }
        else
        {
            if (name != null && name != "del" && name != "DEL" && name != "shift" && name != "SHIFT")
            {
                name = name.ToLower();
                this.GetComponentInChildren<Text>().text = _text.ToLower();
            }
        }
    }

    public void AddLetter()
    {
        if (playernameInputField != null)
        {
            if (name != "del" && name != "shift")
            {
                string letter = name;
                if (isShiftPressed)
                {
                    letter = letter.ToUpper();
                    

                }

                playernameInputField.text += name;
            }
        }
        else
        {
            Debug.LogError("No player name input field reference assigned to the AddLetter script!");
        }
    }

    public void RemoveLetter()
    {
        if (playernameInputField != null)
        {
            if (playernameInputField.text.Length > 0)
                playernameInputField.text = playernameInputField.text.Remove(playernameInputField.text.Length - 1);
        }
    }
}