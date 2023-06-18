using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownTime : MonoBehaviour
{
    public float slowdownDuration = 1f; // Duration in seconds for the time slowdown
    public float targetTimeScale = 0.5f; // The target time scale to reach

    private bool isSlowingDown = false; // Flag to track if time is currently being slowed down
    private float originalTimeScale; // Original time scale before slowing down
    private float timeElapsed; // Time elapsed since the slowdown started

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isSlowingDown)
        {
            originalTimeScale = Time.timeScale;
            timeElapsed = 0f;
            StartCoroutine(SlowTimeCoroutine());
        }
    }

    private IEnumerator SlowTimeCoroutine()
    {
        isSlowingDown = true;

        while (timeElapsed < slowdownDuration)
        {
            Time.timeScale = Mathf.Lerp(originalTimeScale, targetTimeScale, timeElapsed / slowdownDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Time.timeScale = targetTimeScale;
        isSlowingDown = false;
    }
}
