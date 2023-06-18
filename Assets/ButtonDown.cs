using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonDown : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    public void OnPointerDown(PointerEventData eventData)
    {
        Destroy(canvas);
    }
}
