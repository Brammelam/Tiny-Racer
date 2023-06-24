using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LootLocker.Requests;
using LootLocker;
using TMPro;
using System;

public class WhiteLabelManager : MonoBehaviour
{
    [Header("Guest Session")]
    public Button guestButton;
    public TMP_InputField guestSessionInputField;

    [Header("New User")]
    public TMP_InputField newuserEmailInputField;
    public TMP_InputField newuserPasswordInputField;
    public TMP_InputField nicknameInputField;
    public Button registerButton;

    [Header("Existing User")]
    public TMP_InputField existinguserEmailInputField;
    public TMP_InputField existinguserPasswordInputField;
    public Button loginButton;

    [Header("Reset password")]
    public TMP_InputField resetPasswordInputField;
    public Button resetPasswordButton;

    [Header("Remember me")]
    public Toggle rememberMeToggle;
    private int rememberMe;

    [Header("Managers")]
    public PlayerManager playerManager;

    IEnumerator AnimateButton(Button _button, string _s1, string _s2)
    {
        yield return StartCoroutine(FirstFunction(_button, _s1));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(SecondFunction(_button, _s2));
    }

    IEnumerator FirstFunction(Button _button, string _s)
    {
        // Code for the first function
        _button.GetComponentInChildren<Text>().text = _s;
        yield return null;
    }

    IEnumerator SecondFunction(Button _button, string _s)
    {
        // Code for the second function
        _button.GetComponentInChildren<Text>().text = _s;
        yield return null;
    }

    public void Start()
    {
        LootLockerServerManager serverManager = GameObject.FindObjectOfType<LootLockerServerManager>();

        if (serverManager != null)
        {
            Debug.Log("We are already playing, skipping stuff..");
            playerManager.ReturnToMenu();
        }
        else
        {
            rememberMe = PlayerPrefs.GetInt("rememberMe", 0);
            if (rememberMe == 0) rememberMeToggle.isOn = false;
            else rememberMeToggle.isOn = true;
        }
    }

    public void GuestSession()
    {

        guestButton.interactable = false;
        string guestName = guestSessionInputField.text;
        LootLockerSDKManager.StartGuestSession(response =>
        {
            if (!response.success) { 
                
                Debug.Log("Failed to start guest session!");
                StartCoroutine(AnimateButton(guestButton, ":(", "START"));
            }
            else
            {
                playerManager.playerId = response.player_id;
                PlayerPrefs.SetInt("playerid", response.player_id);
                PlayerPrefs.Save(); 
                if (guestName != "")
                {
                    
                    LootLockerSDKManager.SetPlayerName(guestName, (response) =>
                    {
                        if (response.success)
                        {

                            playerManager.playerName.text = "Welcome back, " + response.name.ToString() + "!";
                            playerManager.playerNameString = response.name.ToString();
                            
                            PlayerPrefs.SetString("name", guestName);
                            PlayerPrefs.Save();
                        }
                        else
                        {
                            Debug.Log("Setting player name failed: " + response.Error);
                        }
                    });
                }
                playerManager.Setup();
            }
        });
    }

    public void AutofillLogin()
    {
        string email = PlayerPrefs.GetString("email", "");
        string password = PlayerPrefs.GetString("password", "");
        if ((email != "") && (password != ""))
        {
            existinguserEmailInputField.text = email;
            existinguserPasswordInputField.text = password;
        }
    }

    // Start is called before the first frame update
    public void Login()
    {
        loginButton.interactable = false;
        string email = existinguserEmailInputField.text;
        string password = existinguserPasswordInputField.text;

        LootLockerSDKManager.WhiteLabelLogin(email, password, Convert.ToBoolean(rememberMe), response =>
        {
            if (!response.success)
            {
                loginButton.interactable = true;
                StartCoroutine(AnimateButton(loginButton, ":(", "LOGIN"));
                Debug.Log("Error while logging in!");
                return;
            } else
            {
                if (Convert.ToBoolean(rememberMe))
                {
                    PlayerPrefs.SetString("email", email);
                    PlayerPrefs.SetString("password", password);
                }
                Debug.Log("Successfully logged in!");
            }

            LootLockerSDKManager.StartWhiteLabelSession((response) =>
            {
                if (!response.success)
                {
                    loginButton.interactable = true;
                    StartCoroutine(AnimateButton(loginButton, ":(", "REGISTER"));
                    // Error while starting session
                    Debug.Log("Error starting LootLocker session");
                    return;
                }
                else
                {
                    Debug.Log("Session started successfully!");
                    
                    playerManager.playerId = response.player_id;
                    PlayerPrefs.SetInt("playerid", response.player_id);
                    PlayerPrefs.Save();
                    playerManager.Setup();
                }
            });
        });
    }

    public void TriggerButtonWait()
    {
        StartCoroutine(ReEnableButtons());
    }

    
    IEnumerator ReEnableButtons()
    {
        yield return new WaitForSeconds(2f);
        registerButton.interactable = true;
        loginButton.interactable = true;
        resetPasswordButton.interactable = true;
        guestButton.interactable = true;
    }

    public void NewUser()
    {
        registerButton.interactable = false;
        string email = newuserEmailInputField.text;
        string password = newuserPasswordInputField.text;
        string nickname = nicknameInputField.text;

        void Error(string error)
        {
            Debug.Log(error);
            registerButton.interactable = true;
            StartCoroutine(AnimateButton(registerButton, ":(", "REGISTER"));

        }

        LootLockerSDKManager.WhiteLabelSignUp(email, password, (response) =>
        {
            if (!response.success)
            {
                Error(response.Error);
                return;
            }
            else
            {
                // Sign up successfull
                // Logging in player
                LootLockerSDKManager.WhiteLabelLogin(email, password, true, response =>
                {
                    if (!response.success)
                    {
                        Error(response.Error);
                        return;
                    }
                    // Start session
                    LootLockerSDKManager.StartWhiteLabelSession((response) =>
                    {
                        if (!response.success)
                        {
                            Error(response.Error);
                            return;
                        }
                        playerManager.playerId = response.player_id;
                        PlayerPrefs.SetInt("playerid", response.player_id);
                        PlayerPrefs.Save();
                        playerManager.Setup();
                    });
                });
            }
        });
    }

    public void ToggleRememberMe()
    {
        bool rememberMeBool = rememberMeToggle.isOn;
        rememberMe = Convert.ToInt32(rememberMeBool);
        Debug.Log("Remember me set to " + rememberMeBool);
        PlayerPrefs.SetInt("rememberMe", rememberMe);
    }

    public void AutoLogin()
    {
        // Does user want to log in automatically?
        if (Convert.ToBoolean(rememberMe) == true)
        {
            LootLockerSDKManager.CheckWhiteLabelSession(response =>
            {
                if (!response)
                {
                    // No valid session found
                    rememberMeToggle.isOn = false;
                }
                else
                {
                    // Valid session found, starting game session
                    LootLockerSDKManager.StartWhiteLabelSession((response) =>
                    {
                        if (response.success)
                        {
                            // Game session started successfully
                            playerManager.Setup();
                        }
                        else
                        {
                            // Failed to start game session
                            Debug.Log("Failed to start game session");
                            rememberMeToggle.isOn = false;

                            return;
                        }
                    });
                }
            });
        }
        else if (Convert.ToBoolean(rememberMe) == false) Debug.Log("Logging in as usual");
    }

    public void PasswordReset()
    {
        string email = resetPasswordInputField.text;
        LootLockerSDKManager.WhiteLabelRequestPassword(email, (response) =>
        {
            if (!response.success)
            {
                Debug.Log("Error requesting password reset!");
                return;
            }

            Debug.Log("Successfully requested password reset!");
        });
    }
}
