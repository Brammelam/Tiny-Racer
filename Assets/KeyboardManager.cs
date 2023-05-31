using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyboardManager : MonoBehaviour
{
    [Header("Inputfields")]
    public List<TMP_InputField> inputFields;
    public int currentInputField;

    private void Start()
    {
        inputFields = new List<TMP_InputField>();

        TMP_InputField[] tmpInputFields = FindObjectsOfType<TMP_InputField>();
        foreach (TMP_InputField inputField in tmpInputFields)
        {
            string inputFieldTag = inputField.gameObject.tag;
            if (!string.IsNullOrEmpty(inputFieldTag))
            {
                inputFields.Add(inputField);
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

        }
    }

    private void UpdateIndex(int _i)
    {
        AddLetterClass[] addLetterScripts = GetComponentsInChildren<AddLetterClass>();
        for (int i = 0; i < addLetterScripts.Length; i++)
        {
            AddLetterClass addLetterScript = addLetterScripts[i];

            addLetterScript.SetInputField(inputFields[_i]);

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
}