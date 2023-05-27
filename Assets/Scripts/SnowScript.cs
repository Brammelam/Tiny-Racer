using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowScript : MonoBehaviour
{
    public AudioSource _audioSourceSnow;

    public GameObject _audio;
    public SnowScript musicClass;

    public void Awake()
    {


        if (GameObject.FindGameObjectsWithTag("Snow").Length == 1) DontDestroyOnLoad(gameObject);

        else Destroy(gameObject);



    }


    public void PlaySnowMusic()
    {
        if (_audioSourceSnow.isPlaying) return;
        _audioSourceSnow.Play();

    }


    public void StopSnowMusic()
    {
        _audioSourceSnow.Stop();
    }

}