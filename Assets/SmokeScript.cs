using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeScript : MonoBehaviour
{
    private ParticleSystem currentParticleSystem;
    private float fadeDuration = 5f;
    private float currentFadeTime = 0f;

    private void Start()
    {
        currentParticleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        currentFadeTime += Time.deltaTime;

        // Calculate the alpha value based on the current fade time and duration
        float alpha = 1f - (currentFadeTime / fadeDuration);
        alpha = Mathf.Clamp01(alpha); // Ensure alpha is between 0 and 1

        // Set the alpha value for all particle materials
        ParticleSystemRenderer renderer = currentParticleSystem.GetComponent<ParticleSystemRenderer>();
        Material[] materials = renderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            Color color = materials[i].color;
            color.a = alpha;
            materials[i].color = color;
        }

        // Check if the fade is complete and stop the particle system
        if (currentFadeTime >= fadeDuration)
        {
            currentParticleSystem.Stop();
            Destroy(gameObject, currentParticleSystem.main.duration); // Destroy the GameObject after the particle system finishes playing
        }
    }
}
