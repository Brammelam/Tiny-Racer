using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Navigate : MonoBehaviour
{
    public Button _button;
    //public highscorescript hs;


    public void NavigateToSettings()
    {

        SceneManager.LoadScene(0);

    }

    public void ResetSettings()
    {
        PlayerPrefs.DeleteAll();
    }
}
