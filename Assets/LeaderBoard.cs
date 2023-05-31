using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using TMPro;


public class LeaderBoard : MonoBehaviour
{

    public List<int> leaderboardIds = new List<int>()
    {
        8492, 8493, 8656, 8660, 8661, 8662, 8663, 8664, 8665
    };

    //public List<string> leaderboardNames;
    //public List<string> leaderboardScores;
    //public List<string> leaderboardPlayerScores;

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
            if (response.success)
            {
                Debug.Log("Success when uploading score to level " + level);
                
            } else
            {
                Debug.Log("Failed" + response.Error);
                
            }
        });
        done = true;
        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator FetchPlayerScores()
    {
        int count = 0;

        List<string> tempPlayerScores = new List<string>(9);

        foreach (int i in leaderboardIds)
        {

            LootLockerSDKManager.GetMemberRank(i, pm.playerId, (response) =>
            {
                if (response.statusCode == 200)
                {
                    string playerScore = (response.score == 0) ? "0" : response.score.ToString();

                    tempPlayerScores.Add(playerScore);
                    
                }

                else
                {
                    Debug.Log("failed getting highscoredata for player with ID: " + pm.playerId + ",  " + response.Error);                   
                }

                count++;
            });

        }

        yield return new WaitUntil(() => count == leaderboardIds.Count);
        pm.leaderboardPlayerScores = tempPlayerScores;
    }

    public IEnumerator FetchHighscores()
    {
        List<string> tempPlayerNames = new List<string>(9);
        List<string> tempPlayerScores = new List<string>(9);
        int count = 0;

        foreach (int i in leaderboardIds)
        {
            Debug.Log("Now trying to find global highscores for leaderboard  " + i);
            LootLockerSDKManager.GetScoreList(i, 1, 0, (response) =>
            {
                if (response.success)
                {
                    Debug.Log("234234234" + response.items);
                    
                    LootLockerLeaderboardMember[] members = response.items;

                    if (members.Length == 0 )
                    {
                        tempPlayerNames.Add("No score");
                        tempPlayerScores.Add("0");
                    }
                    else
                    {
                        string playerName = members[0].player.name != "" ? members[0].player.name : Convert.ToString(members[0].player.id);
                        string playerScore = Convert.ToString(members[0].score);
                        tempPlayerNames.Add(playerName);
                        tempPlayerScores.Add(playerScore);
                    }
                    
                }
                else
                {
                    Debug.Log("Failed getting global highscores: " + response.Error);
                }

                count++; // Increment the count after each response
            });
        }

        yield return new WaitUntil(() => count == leaderboardIds.Count);

        pm.leaderboardNames = tempPlayerNames;
        pm.leaderboardScores = tempPlayerScores;
    }
}
