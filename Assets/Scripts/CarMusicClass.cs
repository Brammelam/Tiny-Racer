using UnityEngine;
using UnityEngine.SceneManagement;

public class CarMusicClass : MonoBehaviour
{
    public AudioSource _audioSource;
    public GameObject _audio;
    public checkShit _check;
    private newAI2 player;
    private multiplayerScript mult;
    private multiplayerAI multiplayerScript;
    private TaxiGameHandler taxigh;
    private taxiAI taxiAI;

    public float _pitch;
    public float timeElapsed = 0;
    private bool multiplayer, taximode;
    
    public void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("engineNoise").Length == 1) DontDestroyOnLoad(gameObject);
        else Destroy(gameObject);        
    }

    public void Start()
    {
        multiplayer = false;
        taximode = false;
        if (SceneManager.GetActiveScene().name == "MultiplayerScene") multiplayer = true;

        player = FindObjectOfType<newAI2>();
    }


    public void Update()
    {

        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            if (SceneManager.GetActiveScene().name == "MultiplayerScene") multiplayer = true;
            if (SceneManager.GetActiveScene().name == "MoonScene") taximode = true;



            if (!multiplayer && !taximode)
            {
                if (_check == null) _check = GameObject.FindGameObjectWithTag("GameController").GetComponent<checkShit>();
                if (player == null) player = FindObjectOfType<newAI2>();

                if (_check.tipped && _check != null)
                {
                    SlowCarMusic();
                }
                else
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

            if (multiplayer)
            {
                if (mult == null) mult = GameObject.FindGameObjectWithTag("GameController").GetComponent<multiplayerScript>();
                if (multiplayerScript == null) multiplayerScript = FindObjectOfType<multiplayerAI>();

                if (mult.tipped && mult != null)
                {
                    SlowCarMusic();
                }
                else
                {
                    if (multiplayerScript != null)
                    {
                        float normalizedSpeed = Mathf.InverseLerp(0f, 80f, multiplayerScript.speed);
                        float translatedValue = Mathf.Lerp(0.6f, 1.2f, normalizedSpeed);
                        _audioSource.pitch = _pitch = translatedValue;

                        PlayCarMusic();
                    }
                }
            }

            if (taximode)
            {
                if (taxigh == null) taxigh = GameObject.FindGameObjectWithTag("GameController").GetComponent<TaxiGameHandler>();
                if (taxiAI == null) taxiAI = FindObjectOfType<taxiAI>();

                if (taxigh.tipped && taxigh != null)
                {
                    SlowCarMusic();
                }
                else
                {
                    if (taxiAI != null)
                    {
                        float normalizedSpeed = Mathf.InverseLerp(0f, 80f, taxiAI.speed);
                        float translatedValue = Mathf.Lerp(0.6f, 1.2f, normalizedSpeed);
                        _audioSource.pitch = _pitch = translatedValue;

                        PlayCarMusic();
                    }
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
