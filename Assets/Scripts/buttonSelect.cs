using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class buttonSelect : MonoBehaviour
{
    public Button playButton;
    public Button carButton;
    public Button exitButton;
    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(PlayGame);
        carButton.onClick.AddListener(SelectCar);
        exitButton.onClick.AddListener(ExitGame);




    }

    void PlayGame()
    {
        SceneManager.LoadScene(2);
    }

    void SelectCar()
    {
        SceneManager.LoadScene(1);
    }

    void ExitGame()
    {
        Application.Quit();
    }
}
