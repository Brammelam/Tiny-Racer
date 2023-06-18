using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class EditorTimeScaleFix : MonoBehaviour
{
    private static bool isTimePaused = false;

    // Called when the Editor starts or when Play mode is entered/exited
    static EditorTimeScaleFix()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    // Handles the Play mode state change event
    private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
    {
        // Reset the time scale to normal when entering or exiting Play mode
        if (stateChange == PlayModeStateChange.ExitingEditMode || stateChange == PlayModeStateChange.EnteredEditMode)
        {
            Time.timeScale = 1f;
            isTimePaused = false;
        }
    }

    // Pauses or resumes the Editor's time scale
    [MenuItem("Custom/Editor Time/Toggle Pause")]
    private static void ToggleTimePause()
    {
        isTimePaused = !isTimePaused;
        Time.timeScale = isTimePaused ? 0f : 1f;
        Debug.Log("Editor Time Scale Paused: " + isTimePaused);
    }
}
