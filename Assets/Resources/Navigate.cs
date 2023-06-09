using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Navigate : MonoBehaviour
{
    public Button _button;

    public void NavigateToSettings()
    {

        SceneManager.LoadScene(0);

    }

    public void ResetSettings()
    {
        PlayerPrefs.DeleteAll();
    }
}
