using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSource carAudioSource;
    public AudioSource ambientMusicSource;
    public AudioSource cityMusicSource;
    public AudioSource snowMusicSource;
    public AudioSource mainThemeSource;
    public ScoresSO scoresSO;
    public checkShit check;
    public PlayerManager pm;
    public speedTracker speedTracker;
    private int sceneIndex;
    [SerializeField]
    private float speed;

    private void Awake()
    {
        pm = FindObjectOfType<PlayerManager>();
        if (pm != null) scoresSO = pm.leaderboardSO;

        AssignAudioSources();
        PlayInitialMusic();
    }

    public void Initialize()
    {
        scoresSO = Resources.Load<ScoresSO>("SO/ScoresSO");
        pm = GameObject.FindObjectOfType<PlayerManager>();
        check = GameObject.FindObjectOfType<checkShit>();
        FindMainCamera();

        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        Debug.Log("THE CURRENT SCENE IS " + sceneIndex);

        // Subscribe to the check variable's change event
        if (check != null)
        {
            check.TippedChanged += HandleCheckChanged;
        }
        if (scoresSO != null) scoresSO.LevelIndexChanged += HandleLevelChanged;

    }

    private void OnDestroy()
    {
        // Unsubscribe from the check variable's change event
        if (check != null) check.TippedChanged -= HandleCheckChanged;

        if (scoresSO != null) scoresSO.LevelIndexChanged -= HandleLevelChanged;
    }

    private void HandleCheckChanged(bool tipped)
    {
        if (tipped)
        {
            Instantiate(Resources.Load("carCrashSound") as GameObject);
            SlowCarMusic(); // fades out
        }
    }

    private void HandleLevelChanged(int level)
    {
        StopMusic();
        if (sceneIndex == 0) return;
        if (level < 3 || level == 7 || level == 8 || level == 9)
        {

            PlayAmbientMusic();
            ambientMusicSource.volume = 0.5f;

            if (level == 7 || level == 8)
            {
                ambientMusicSource.pitch = 0.5f;
            }
        }
        else if (level == 3)
        {
            PlayCityMusic();
            cityMusicSource.volume = 0.5f;
        }
        else if (level > 3 && level != 7)
        {
            PlaySnowMusic();
            snowMusicSource.volume = 0.5f;
        }
    }

    private void AssignAudioSources()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource.CompareTag("Music"))
            {
                // Assign the theme music audio source
                mainThemeSource = audioSource;
            }
            else if (audioSource.CompareTag("engineNoise"))
            {
                // Assign the car audio source
                carAudioSource = audioSource;
            }
            else if (audioSource.CompareTag("Ambient"))
            {
                // Assign the ambient music audio source
                ambientMusicSource = audioSource;
            }
            else if (audioSource.CompareTag("City"))
            {
                // Assign the city music audio source
                cityMusicSource = audioSource;
            }
            else if (audioSource.CompareTag("Snow"))
            {
                // Assign the snow music audio source
                snowMusicSource = audioSource;
            }
        }
    }

    internal void FindMainCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Attach the audio manager script to the main camera's AudioListener component
            AudioListener audioListener = mainCamera.GetComponent<AudioListener>();
            if (audioListener != null)
            {
                audioListener.gameObject.AddComponent<AudioManager>();
            }
            else
            {
                Debug.LogWarning("Main camera does not have an AudioListener component!");
            }
        }
        else
        {
            Debug.LogWarning("Main camera not found! Make sure the main camera is tagged as 'MainCamera'.");
        }
    }

    private void PlayInitialMusic()
    {
        StopMusic();

        if (scoresSO != null)
        {
            int currentLevel = scoresSO.CurrentLevel;

            if (sceneIndex == 0) PlayMainTheme(); else StopMainTheme();

            if (sceneIndex > 0)
            {
                carAudioSource.Play();
                // Music Logic
                if (currentLevel < 3 || currentLevel == 7 || currentLevel == 8 || currentLevel == 9)
                {
                    PlayAmbientMusic();
                    if (currentLevel == 7 || currentLevel == 8)
                    {
                        ambientMusicSource.pitch = 0.5f;
                    }
                }
                else if (currentLevel == 3)
                {
                    PlayCityMusic();
                    StopMusic();
                }
                else if (currentLevel > 3 && currentLevel != 7)
                {
                    PlaySnowMusic();
                }
            }
        }
    }

    public void PlayMainTheme()
    {
        mainThemeSource.Play();
    }

    public void StopMainTheme()
    {
        mainThemeSource.Stop();
    }

    public void PlayAmbientMusic()
    {
        ambientMusicSource.Play();
    }

    public void PlayCityMusic()
    {
        cityMusicSource.Play();
    }

    public void PlaySnowMusic()
    {
        snowMusicSource.Play();
    }

    public void StopMusic()
    {
        ambientMusicSource.pitch = 1f;
        ambientMusicSource.volume = 1.0f;
        cityMusicSource.volume = 1.0f;
        snowMusicSource.volume = 1.0f;

        ambientMusicSource.Stop();
        cityMusicSource.Stop();
        snowMusicSource.Stop();
    }

    public void SlowCarMusic()
    {
        float timeElapsed = 0;

        while (timeElapsed < 2f)
        {
            float pitch = Mathf.Lerp(1f, 0.0001f, timeElapsed / 2f);
            carAudioSource.pitch = pitch;
            timeElapsed += Time.deltaTime;
        }

        StopCarMusic();
    }

    public void StopCarMusic()
    {
        carAudioSource.Stop();
    }
}