using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private Slider progressBar;

    private void Start()
    {
        progressBar = this.GetComponent<Slider>();
        playerManager.OnProgressUpdate += UpdateProgressBar;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerManager = GameObject.FindObjectOfType<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.OnProgressUpdate += UpdateProgressBar;

        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (playerManager != null)
        {
            playerManager.OnProgressUpdate -= UpdateProgressBar;
        }
    }

    private void UpdateProgressBar(float progress)
    {
        // Update the progress bar using the received progress value
        progressBar.value = progress;
    }
}
