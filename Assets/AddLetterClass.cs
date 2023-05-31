using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AddLetterClass : MonoBehaviour
{
    [Header("Variables")]
    public int assignedInputFieldIndex;
    public TMP_InputField playernameInputField;

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

    public void AddLetter()
    {
        if (playernameInputField != null)
        {
            if (name != "del")
                playernameInputField.text += name;
            else if (playernameInputField.text != "")
                playernameInputField.text = playernameInputField.text.Remove(playernameInputField.text.Length - 1);
        }
        else
        {
            Debug.LogError("No player name input field reference assigned to the AddLetter script!");
        }
    }
}