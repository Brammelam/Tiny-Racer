using UnityEngine;
using UnityEngine.SceneManagement;

public class CarMusicClass : MonoBehaviour
{
    public AudioSource _audioSource;
    public GameObject _audio;
    public checkShit _check;

    public float _pitch;
    public float timeElapsed = 0;
    
    
    public void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("engineNoise").Length == 1) DontDestroyOnLoad(gameObject);
        else Destroy(gameObject);        
    }

    

    public void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex > 1 ) {
            if (_check == null) _check = GameObject.FindGameObjectWithTag("GameController").GetComponent<checkShit>();

            if (_check.tipped)
            {

                SlowCarMusic();
            } else
            {
                _pitch = _check.currentSpeed / 95;
                _audioSource.pitch = _pitch;
                Debug.Log("pitch is " + _pitch);
            }

            
        }

    }

    public void PlayCarMusic()
    {
        if (_audioSource.isPlaying) return;
        _audioSource.Play();

    }

    public void StopCarMusic()
    {
        _audioSource.Stop();
    }

    public void SlowCarMusic()
    {
        
        if (timeElapsed < 2f)
        {
            _audioSource.pitch = Mathf.Lerp(1f, 0.0001f, timeElapsed / 2f);
            timeElapsed += Time.deltaTime;
        }
        else StopCarMusic();
    }
}
