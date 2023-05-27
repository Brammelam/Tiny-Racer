using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [SerializeField]
    public Button button1;
    [SerializeField]
    public Button button2;
    [SerializeField]
    public GameObject bodySettings;
    [SerializeField]
    public GameObject windowSettings;
    [SerializeField]
    public Text editingText;
    private bool isEditingBody = true;

    // Start is called before the first frame update
    void Start()
    {
        // Set initial state
        button1.interactable = false;
        button2.interactable = true;
        editingText.text = "Now editing body";

        // Subscribe to button click events
        button1.onClick.AddListener(ToggleEditing);
        button2.onClick.AddListener(ToggleEditing);

        // Set sliders
        bodySettings.SetActive(true);
        windowSettings.SetActive(false);
    }

    void ToggleEditing()
    {
        isEditingBody = !isEditingBody;

        button1.interactable = !isEditingBody;
        button2.interactable = isEditingBody;

        if (isEditingBody)
        {
            editingText.text = "Now editing body";
            bodySettings.SetActive(true);
            windowSettings.SetActive(false);
        }
        else
        {
            editingText.text = "Now editing windows";
            bodySettings.SetActive(false);
            windowSettings.SetActive(true);
        }
    }
}
