using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Navigate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Button _button;
    //public highscorescript hs;
    public Animator buttonAnimator;
    public string hoverAnimationName = "settingsopen";
    public string pressAnimationName = "settingsopen";

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonAnimator.Play(hoverAnimationName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonAnimator.Play(hoverAnimationName + "Reverse");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonAnimator.Play(pressAnimationName);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonAnimator.Play(pressAnimationName + "Reverse");
    }

    public void NavigateToSettings()
    {

        SceneManager.LoadScene(0);

    }

    public void ResetSettings()
    {
        PlayerPrefs.DeleteAll();
    }
}
