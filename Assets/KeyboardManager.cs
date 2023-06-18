using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyboardManager : MonoBehaviour
{
    [Header("Inputfields")]
    public List<TMP_InputField> inputFields;
    public int currentInputField;
    
    [SerializeField]
    private bool isShiftPressed;

    private void Awake()
    {
        inputFields = new List<TMP_InputField>();

        Transform parentTransform = transform.parent;
        foreach (Transform childTransform in parentTransform)
        {
            TMP_InputField tmpInputField = childTransform.GetComponent<TMP_InputField>();
            if (tmpInputField != null)
            {
                string inputFieldTag = tmpInputField.gameObject.tag;
                if (!string.IsNullOrEmpty(inputFieldTag))
                {
                    inputFields.Add(tmpInputField);
                }
            }
        }

        inputFields.Sort((a, b) => a.gameObject.tag.CompareTo(b.gameObject.tag));

        SetFirstIndex();



    }

    private void SetFirstIndex()
    {
        AddLetterClass[] addLetterScripts = GetComponentsInChildren<AddLetterClass>();
        for (int i = 0; i < addLetterScripts.Length; i++)
        {
            AddLetterClass addLetterScript = addLetterScripts[i];

            addLetterScript.SetInputField(inputFields[0]);
            addLetterScript.UpdateTextCasing(isShiftPressed);


        }
    }

    private void UpdateIndex(int _i)
    {
        AddLetterClass[] addLetterScripts = GetComponentsInChildren<AddLetterClass>();
        for (int i = 0; i < addLetterScripts.Length; i++)
        {
            AddLetterClass addLetterScript = addLetterScripts[i];

            addLetterScript.SetInputField(inputFields[_i]);
            addLetterScript.UpdateTextCasing(isShiftPressed);

        }
    }

    public void SelectNick()
    {
        currentInputField = 0;
        UpdateIndex(currentInputField);
    }
    public void SelectEmail()
    {
        currentInputField = 1;
        UpdateIndex(currentInputField);
    }
    public void SelectPw()
    {
        currentInputField = 2;
        UpdateIndex(currentInputField);
    }

    public void ToggleShift()
    {
        if (isShiftPressed) isShiftPressed = false;
        else isShiftPressed = true;

        AddLetterClass[] addLetterScripts = GetComponentsInChildren<AddLetterClass>();
        for (int i = 0; i < addLetterScripts.Length; i++)
        {
            AddLetterClass addLetterScript = addLetterScripts[i];
            addLetterScript.UpdateTextCasing(isShiftPressed);
        }
    }
}