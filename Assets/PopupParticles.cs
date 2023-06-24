using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupParticles : MonoBehaviour
{
    private ParticleSystem currentParticleSystem;
    private Transform cameraTransform;

    private void Start()
    {
        currentParticleSystem = GetComponent<ParticleSystem>();
        cameraTransform = Camera.main.transform;

        // Start the particle animation
        StartCoroutine(PlayParticles());
    }

    private System.Collections.IEnumerator PlayParticles()
    {
        // Play the particle animation
        currentParticleSystem.Play();

        while (currentParticleSystem.isPlaying)
        {
            // Update the particle system's position to match the camera's position
            currentParticleSystem.transform.position = cameraTransform.position;
            currentParticleSystem.transform.rotation= cameraTransform.rotation;


            yield return null;
        }

        // Stop and disable the particle system
        currentParticleSystem.Stop();
        currentParticleSystem.gameObject.SetActive(false);
    }
}