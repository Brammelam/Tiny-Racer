using UnityEngine;
using UnityEngine.SceneManagement;

public class CarMusicClass : MonoBehaviour
{
    public AudioSource _audioSource;
    public GameObject _audio;
    public checkShit _check;
    private newAI2 player;

    public float _pitch;
    public float timeElapsed = 0;
    
    
    public void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("engineNoise").Length == 1) DontDestroyOnLoad(gameObject);
        else Destroy(gameObject);        
    }

    public void Start()
    {
        player = FindObjectOfType<newAI2>();
    }


    public void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex > 1 ) {
            if (_check == null) _check = GameObject.FindGameObjectWithTag("GameController").GetComponent<checkShit>();
            if (player == null) player = FindObjectOfType<newAI2>();

            if (_check.tipped && _check != null)
            {
                SlowCarMusic();
            } else
            {
                if (player != null)
                {
                    float normalizedSpeed = Mathf.InverseLerp(0f, 80f, player.speed);
                    float translatedValue = Mathf.Lerp(0.6f, 1.2f, normalizedSpeed);
                    _audioSource.pitch = _pitch = translatedValue;

                    PlayCarMusic();
                }
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
