using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientClass : MonoBehaviour
{
    public AudioSource _audioSourceAmbient;

    public GameObject _audio;
    public AmbientClass musicClass;

    public void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("Ambient").Length == 1) DontDestroyOnLoad(gameObject);

        else Destroy(gameObject);


    }


    public void PlayAmbientMusic()
    {
        if (_audioSourceAmbient.isPlaying) return;
        _audioSourceAmbient.Play();

    }


    public void StopAmbientMusic()
    {
        _audioSourceAmbient.Stop();
    }

}