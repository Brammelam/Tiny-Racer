using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using TMPro;


public class LeaderBoard : MonoBehaviour
{

    public List<int> leaderboardIds;

    public List<string> leaderboardNames;
    public List<string> leaderboardScores;
    public List<string> leaderboardPlayerScores;

    public string currentScore;

    public TextMeshProUGUI playerNames;
    public TextMeshProUGUI playerScores;

    public PlayerManager pm;

    private void Awake()
    {
        int numLeaderBoards = FindObjectsOfType<LeaderBoard>().Length;
        if (numLeaderBoards != 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        leaderboardIds = new List<int>()
        {
            8492, 8493, 8656, 8660, 8661, 8662, 8663, 8664, 8665
        };

        DontDestroyOnLoad(this);
        if (pm == null) pm = GameObject.FindObjectOfType<PlayerManager>();
    }

    private void Update()
    {
        if (pm == null) pm = GameObject.FindObjectOfType<PlayerManager>();
    }

    public IEnumerator SubmitScoreCoroutine(int scoreToUpload, int level)
    {
        bool done = false;
        string playerID = pm.playerId.ToString();
        string levelID = leaderboardIds[level].ToString();

        LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, levelID, (response) =>
        {

        });
        done = true;
        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator FetchPlayerScores()
    {
        bool done = false;
        Dictionary<int, int> tempPlayerScores = new Dictionary<int, int>(leaderboardIds.Count);

        foreach (int i in leaderboardIds)
        {
            LootLockerSDKManager.GetAllMemberRanks(pm.playerId, 1, (response) =>
            {
                if (response.statusCode == 200)
                {
                    foreach (var leaderboard in response.leaderboards)
                    {
                        int leaderboardId = leaderboard.leaderboard_id;
                        int score = leaderboard.rank.score;

                        tempPlayerScores[leaderboardId] = score;
                    }

                    List<int> scoresList = new List<int>();

                    foreach (int leaderboardId in leaderboardIds)
                    {
                        // Get the score for the current leaderboard ID from the dictionary
                        int score = tempPlayerScores.ContainsKey(leaderboardId) ? tempPlayerScores[leaderboardId] : 0;

                        // Add the score to the list
                        scoresList.Add(score);
                    }

                    // Convert scoresList to a list of strings in the same index order
                    List<string> leaderboardPlayerScores = new List<string>();
                    for (int i = 0; i < leaderboardIds.Count; i++)
                    {
                        int leaderboardId = leaderboardIds[i];
                        int score = scoresList[i];
                        leaderboardPlayerScores.Add(score.ToString());
                    }

                    // Assign leaderboardPlayerScores to pm.leaderboardPlayerScores
                    pm.leaderboardPlayerScores = leaderboardPlayerScores;

                    done = true;
                }

                else
                {

                    done = true;
                }
            });

        }
        yield return new WaitUntil(() => done = true);

    }

    public IEnumerator FetchHighscores()
    {
        List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();

        int completedRequests = 0; // Counter to track completed requests

        foreach (int leaderboardId in leaderboardIds)
        {
            int? previousCursor = null;
            bool isRequestCompleted = false;

            LootLockerSDKManager.GetScoreList(leaderboardId, 1, 0, (response) =>
            {
                LeaderboardEntry entry = new LeaderboardEntry();
                entry.leaderboardId = leaderboardId;

                if (response.success && response.items.Length > 0)
                {
                    entry.fastestTime = response.items[0].score;
                    entry.playerName = response.items[0].player.name;
                }
                else
                {
                    entry.fastestTime = 99999; // Default value if no score is found
                    entry.playerName = "Anonymous"; // Default player name
                }

                leaderboardEntries.Add(entry);

                completedRequests++; // Increment completed requests counter

                isRequestCompleted = true; // Set the flag to indicate the request is completed
            });

            yield return new WaitUntil(() => isRequestCompleted);
        }

        // Wait until all requests have completed
        yield return new WaitUntil(() => completedRequests == leaderboardIds.Count);

        leaderboardEntries.Sort((x, y) => leaderboardIds.IndexOf(x.leaderboardId) - leaderboardIds.IndexOf(y.leaderboardId));

        // Extract the fastest times and player names in the correct order
        List<int> fastestTimes = new List<int>();
        foreach (LeaderboardEntry entry in leaderboardEntries)
        {
            fastestTimes.Add(entry.fastestTime);
        }

        // Convert fastestTimes and playerNames to lists of strings
        List<string> leaderboardFastestTimesAsString = fastestTimes.ConvertAll(time => time.ToString());
        List<string> leaderboardPlayerNames = leaderboardEntries.ConvertAll(entry => entry.playerName);

        // Assign leaderboardFastestTimesAsString and leaderboardPlayerNames to pm.leaderboardPlayerScores and pm.leaderboardNames respectively
        pm.leaderboardScores = leaderboardFastestTimesAsString;
        pm.leaderboardNames = leaderboardPlayerNames;
    }

    public class LeaderboardEntry
    {
        public int leaderboardId;
        public int fastestTime;
        public string playerName;
    }
}
