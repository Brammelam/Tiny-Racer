using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetCameraDistance : MonoBehaviour
{
    public Camera mainCamera { get; private set; }
    private Slider slider;

    public RectTransform inputRegion;

    public void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        float sliderValue = PlayerPrefs.GetFloat("cameradistance", 30f);
        slider = GetComponent<Slider>();
        slider.value = sliderValue;
    }
    public void UpdateCameraDistance(float _distance)
    {

        mainCamera.orthographicSize = _distance;
        PlayerPrefs.SetFloat("cameradistance", _distance);
        PlayerPrefs.Save();
    }
}
