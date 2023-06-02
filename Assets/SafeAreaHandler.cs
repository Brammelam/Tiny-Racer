using UnityEngine;

public class SafeAreaHandler : MonoBehaviour
{
    public RectTransform[] uiElements;

    private void Start()
    {
        // Get the safe area of the screen
        Rect safeArea = Screen.safeArea;

        // Adjust the UI elements based on the safe area
        foreach (RectTransform uiElement in uiElements)
        {
            // Calculate the offset of the UI element based on the safe area
            float offsetX = safeArea.x / Screen.width;
            float offsetY = safeArea.y / Screen.height;

            // Calculate the width and height of the UI element relative to the safe area
            float widthRatio = safeArea.width / Screen.width;
            float heightRatio = safeArea.height / Screen.height;

            // Set the new position and size of the UI element
            uiElement.anchorMin = new Vector2(offsetX, offsetY);
            uiElement.anchorMax = new Vector2(offsetX + widthRatio, offsetY + heightRatio);
        }
    }
}