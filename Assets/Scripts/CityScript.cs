using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityScript : MonoBehaviour
{
    public AudioSource _audioSourceCity;

    public GameObject _audio;
    public CityScript musicClass;

    public void Awake()
    {


        if (GameObject.FindGameObjectsWithTag("City").Length == 1) DontDestroyOnLoad(gameObject);

        else Destroy(gameObject);



    }


    public void PlayCityMusic()
    {
        if (_audioSourceCity.isPlaying) return;
        _audioSourceCity.Play();

    }


    public void StopCityMusic()
    {
        _audioSourceCity.Stop();
    }

}