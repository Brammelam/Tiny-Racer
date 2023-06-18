using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject guestscreen, loginscreen, startscreen, registerscreen, resetscreen, loginbutton;
    [SerializeField] WhiteLabelManager whiteLabelManager;
    private bool animationComplete;

    // Start is called before the first frame update
    void Start()
    {
        animationComplete = false;
    }

    public void OnClickGuestScreen()
    {
        animationComplete = false;
        animator.Play("buttonclick");

        StartCoroutine(ClickGuestScreen());
    }

    public void OnClickLoginScreen()
    {
        animationComplete = false;
        animator.Play("buttonclick");

        StartCoroutine(ClickLoginScreen());

    }

    public void OnClickRegisterScreen()
    {
        animationComplete = false;
        animator.Play("buttonclick");

        StartCoroutine(ClickRegisterScreen());
    }

    public void OnClickResetScreen()
    {
        animationComplete = false;
        animator.Play("buttonclick");

        StartCoroutine(ClickResetScreen());
    }

    public void OnClickGuest()
    {
        animationComplete = false;
        animator.Play("buttonclick");

        StartCoroutine(ClickGuest());
    }

    public void OnClickLogin()
    {
        animationComplete = false;
        animator.Play("buttonclick");

        StartCoroutine(ClickLogin());
    }

    public void OnClickRegister()
    {
        animationComplete = false;
        animator.Play("buttonclick");

        StartCoroutine(ClickRegister());
    }

    public void OnClickBackFromLogin()
    {
        animationComplete = false;
        animator.Play("buttonclick");

        StartCoroutine(ClickBackFromLogin());
    }

    public void OnClickBackFromRegister()
    {
        animationComplete = false;
        animator.Play("buttonclick");

        StartCoroutine(ClickBackFromRegister());
    }

    public void OnClickBackFromReset()
    {
        animationComplete = false;
        animator.Play("buttonclick");

        StartCoroutine(ClickBackFromReset());
    }

    private IEnumerator ClickBackFromLogin()
    {
        while (!animationComplete) yield return null;

        startscreen.SetActive(true);
        loginscreen.SetActive(false);

    }

    private IEnumerator ClickBackFromRegister()
    {
        while (!animationComplete) yield return null;

        startscreen.SetActive(true);
        registerscreen.SetActive(false);

    }

    private IEnumerator ClickBackFromReset()
    {
        while (!animationComplete) yield return null;

        startscreen.SetActive(true);
        resetscreen.SetActive(false);

    }

    private IEnumerator ClickBackFromGuest()
    {
        while (!animationComplete) yield return null;

        startscreen.SetActive(true);
        guestscreen.SetActive(false);
    }

    private IEnumerator ClickGuestScreen()
    {
        while (!animationComplete) yield return null;

        guestscreen.SetActive(true);
        startscreen.SetActive(false);
    }


    private IEnumerator ClickLoginScreen()
    {
        while (!animationComplete) yield return null;

        whiteLabelManager.AutofillLogin();

        loginscreen.SetActive(true);
        startscreen.SetActive(false);

    }

    private IEnumerator ClickRegisterScreen()
    {
        while (!animationComplete) yield return null;

        whiteLabelManager.AutofillLogin();

        registerscreen.SetActive(true);
        startscreen.SetActive(false);

    }

    private IEnumerator ClickResetScreen()
    {
        while (!animationComplete) yield return null;

        resetscreen.SetActive(true);
        startscreen.SetActive(false);

    }

    private IEnumerator ClickGuest()
    {
        while (!animationComplete) yield return null;

        whiteLabelManager.GuestSession();
    }

    private IEnumerator ClickLogin()
    {
        while (!animationComplete) yield return null;

        whiteLabelManager.Login();
    }

    private IEnumerator ClickRegister()
    {
        while (!animationComplete) yield return null;

        whiteLabelManager.NewUser();
    }

    public void AnimationComplete()
    {
        animationComplete = true;
    }
}
