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
    [Header("New User")]
    public TMP_InputField newuserEmailInputField;
    public TMP_InputField newuserPasswordInputField;
    public TMP_InputField nicknamelInputField;
    public Button registerButton;

    [Header("Existing User")]
    public TMP_InputField existinguserEmailInputField;
    public TMP_InputField existinguserPasswordInputField;
    public Button loginButton;

    [Header("Reset password")]
    public TMP_InputField resetPasswordInputField;

    [Header("Remember me")]
    public Toggle rememberMeToggle;
    private int rememberMe;

    [SerializeField]
    public PlayerManager playerManager;
    public LootLockerServerManager serverManager;



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

        string email = existinguserEmailInputField.text;
        string password = existinguserPasswordInputField.text;

        LootLockerSDKManager.WhiteLabelLogin(email, password, Convert.ToBoolean(rememberMe), response =>
        {
            if (!response.success)
            {
                loginButton.GetComponentInChildren<Text>().text = ":(";
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
                    // Error while starting session
                    Debug.Log("Error starting LootLocker session");
                    return;
                }
                else
                {
                    Debug.Log("Session started successfully!");
                    
                    playerManager.playerId = response.player_id;
                    playerManager.Setup();
                }
            });
        });
    }

    public void NewUser()
    {
        string email = newuserEmailInputField.text;
        string password = newuserPasswordInputField.text;
        string nickname = nicknamelInputField.text;

        void Error(string error)
        {
            Debug.Log(error);
            registerButton.GetComponentInChildren<Text>().text = ":(";
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
                        playerManager.playerId = Convert.ToInt32(response.public_uid);
                        playerManager.Setup();
                    });
                });
            }
        });
    }

    public void Start()
    {
        var existingObj = FindObjectOfType<LootLockerServerManager>();
        // Skip login if we already are playing, and returning to menu
        if (existingObj != null)
        {
            Debug.Log("We are already playing, skipping stuff..");
            playerManager.Setup();
        }

        rememberMe = PlayerPrefs.GetInt("rememberMe", 0);
        if (rememberMe == 0) rememberMeToggle.isOn = false;
        else rememberMeToggle.isOn = true;

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
