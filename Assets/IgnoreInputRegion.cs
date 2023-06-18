using UnityEngine;
using UnityEngine.EventSystems;

public class IgnoreInputRegion : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        // Prevent input events from being forwarded
        eventData.Use();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Prevent input events from being forwarded
        eventData.Use();
    }
}