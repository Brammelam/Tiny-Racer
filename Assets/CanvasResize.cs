using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasResize : MonoBehaviour
{
    public RectTransform canvasRect;
    public float widthPercentage = 0.33f;
    public float height = 300f;

    private void Start()
    {
        // Calculate the desired width based on the screen size and percentage
        float screenWidth = Screen.width;
        float desiredWidth = screenWidth * widthPercentage;

        // Set the width of the Canvas to the desired value
        canvasRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, desiredWidth);

        // Set the height of the Canvas
        canvasRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        // Set the anchor to bottom right
        canvasRect.anchorMin = new Vector2(1f, 0f);
        canvasRect.anchorMax = new Vector2(1f, 0f);

        // Set the position to the bottom right corner
        canvasRect.anchoredPosition = new Vector2(-desiredWidth, 0f);
    }
}
