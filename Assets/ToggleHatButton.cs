using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHatButton : MonoBehaviour
{
    public Color normalColor; // The normal color of the button
    public Color highlightColor; // The highlighted color of the button

    private bool isPressed;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Toggle);
    }

    private void Toggle()
    {
        isPressed = !isPressed;

        // Update the button visuals
        UpdateButtonVisuals();

        // Disable other ToggleButtons in the scene
        ToggleHatButton[] otherToggleButtons = FindObjectsOfType<ToggleHatButton>();
        foreach (ToggleHatButton otherButton in otherToggleButtons)
        {
            if (otherButton != this)
            {
                otherButton.isPressed = false;
                otherButton.UpdateButtonVisuals();
            }
        }
    }

    private void UpdateButtonVisuals()
    {
        // Set the button's color based on its pressed state
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = isPressed ? highlightColor : normalColor;
        button.colors = colorBlock;
    }
}

