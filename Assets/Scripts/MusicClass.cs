using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicClass : MonoBehaviour
{
    public AudioSource _audioSource;
    public GameObject _audio;
    public MusicClass musicClass;

    public void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("Music").Length == 1 ) DontDestroyOnLoad(gameObject);

        else Destroy(gameObject);
        
    }


    public void PlayMusic()
    {
        if (_audioSource.isPlaying) return;
        _audioSource.Play();
        
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }
}
