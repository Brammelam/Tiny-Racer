using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupParticles : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private Transform cameraTransform;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        cameraTransform = Camera.main.transform;

        // Start the particle animation
        StartCoroutine(PlayParticles());
    }

    private System.Collections.IEnumerator PlayParticles()
    {
        // Play the particle animation
        particleSystem.Play();

        while (particleSystem.isPlaying)
        {
            // Update the particle system's position to match the camera's position
            particleSystem.transform.position = cameraTransform.position;
            particleSystem.transform.rotation= cameraTransform.rotation;


            yield return null;
        }

        // Stop and disable the particle system
        particleSystem.Stop();
        particleSystem.gameObject.SetActive(false);
    }
}