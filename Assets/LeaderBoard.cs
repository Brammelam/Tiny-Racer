using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LootLocker.Requests;
using TMPro;
using System.Linq;

public class LeaderBoard : MonoBehaviour
{
    private static LeaderBoard instance;
    public static LeaderBoard Instance { get { return instance; } }

    public List<string> leaderboardIds;

    public List<string> leaderboardNames;
    public List<int> leaderboardScores;
    public List<int> leaderboardPlayerScores;

    public string currentScore;

    public TextMeshProUGUI playerNames;
    public TextMeshProUGUI playerScores;

    public PlayerManager pm;

    public ScoresSO leaderboardSO;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneChanged;
    }

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        leaderboardIds = new List<string>()
        {
            "hs11", "hs12", "hs13", "hs14", "hs15", "hs16", "hs17", "hs18", "hs19", "hs110", "hs111", "hs112"
        };

        leaderboardNames = new List<string>(new string[leaderboardIds.Count]);
        leaderboardScores = new List<int>(new int[leaderboardIds.Count]);
        leaderboardPlayerScores = new List<int>(new int[leaderboardIds.Count]);

        DontDestroyOnLoad(this);
        if (pm == null) pm = GameObject.FindObjectOfType<PlayerManager>();
    }

    private void Update()
    {
        if (pm == null) pm = GameObject.FindObjectOfType<PlayerManager>();
    }

    private void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        if (ScoresSO.Instance == null)
        {
            Debug.LogWarning("ScoresSO instance is null. Make sure it is properly set up.");
        }
    }

    public void HandlePlayerValuesChanged(int newScore, int index, bool global)
    {       
        string playerName = PlayerPrefs.GetString("name", "Anonymous");

        leaderboardPlayerScores[index] = newScore;
        
        if (global)
        {
            leaderboardScores[index] = newScore;
            leaderboardNames[index] = playerName;
        }
    }

    public IEnumerator SubmitScoreCoroutine(int scoreToUpload, int level, bool isGlobalScore)
    {
        bool done = false;
        string playerID = PlayerPrefs.GetInt("playerid").ToString();
        string levelID = leaderboardIds[level];

        LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, levelID, (response) =>
        {
            if (!response.success)
            {
                Debug.Log("Failed to upload score!");

            }
            
            done = true;
        });

        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator FetchPlayerScores()
    {

        int _numberOfLeaderboards = leaderboardIds.Count;
        int _playerId = PlayerPrefs.GetInt("playerid");
        
        Dictionary<string, int> tempPlayerScores = new Dictionary<string, int>(_numberOfLeaderboards);        
        Dictionary<string, int> leaderboardPlayerScoresDict = new Dictionary<string, int>(_numberOfLeaderboards);

        TaskCompletionSource<bool> requestCompletedTask = new TaskCompletionSource<bool>();

        LootLockerSDKManager.GetAllMemberRanks(_playerId, _numberOfLeaderboards, (response) =>
        {
            if (response.statusCode == 200)
            {
                foreach (var leaderboard in response.leaderboards)
                {
                    string leaderboardKey = leaderboard.leaderboard_key;
                    int score = leaderboard.rank.score;
                    if (score <= 1) score = 99999;

                    tempPlayerScores.Add(leaderboardKey, score);
                }

                foreach (var leaderboardKey in leaderboardIds)
                {
                    if (tempPlayerScores.TryGetValue(leaderboardKey, out int score))
                    {
                        leaderboardPlayerScoresDict.Add(leaderboardKey, score);
                    }
                }
            }

            requestCompletedTask.SetResult(true);
        });

        yield return new WaitUntil(() => requestCompletedTask.Task.IsCompleted);

        leaderboardPlayerScores = leaderboardPlayerScoresDict.Values.ToList();

    }

    public IEnumerator FetchHighscores()
    {

        int _numberOfLeaderboards = leaderboardIds.Count;
        int _playerId = PlayerPrefs.GetInt("playerid");

        Dictionary<string, (int, string)> tempPlayerScores = new Dictionary<string, (int, string)>(_numberOfLeaderboards);
        Dictionary<string, (int, string)> leaderboardScoresDict = new Dictionary<string, (int, string)>(_numberOfLeaderboards);

        foreach (string leaderboardKey in leaderboardIds)
        {
            TaskCompletionSource<bool> requestCompletedTask = new TaskCompletionSource<bool>();

            LootLockerSDKManager.GetScoreList(leaderboardKey, 1, 0, (response) =>
            {

                if (response.success && response.items.Length > 0)
                {
                    var item = response.items[0];
                    string _name = "Anonymous";
                    int _score = 99999;
                    if (item.player.name.Length > 0) 
                        _name = item.player.name;

                    _score = item.score;

                    leaderboardScoresDict.Add(leaderboardKey, (_score, _name));
                }

                requestCompletedTask.SetResult(true); 
            });

            yield return new WaitUntil(() => requestCompletedTask.Task.IsCompleted);
        }

        leaderboardScores = leaderboardScoresDict.Values.Select(tuple => tuple.Item1).ToList();
        leaderboardNames = leaderboardScoresDict.Values.Select(tuple => tuple.Item2).ToList();            
    }
}
