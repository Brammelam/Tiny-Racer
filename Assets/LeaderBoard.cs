using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using TMPro;


public class LeaderBoard : MonoBehaviour
{

    public int[] leaderboardIds =
    {
        8492, 8493, 8656, 8660, 8661, 8662, 8663, 8664, 8665
    };

    public List<string> leaderboardNames;
    public List<string> leaderboardScores;

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
        string playerID = PlayerPrefs.GetString("PlayerID");

        LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, leaderboardIds[level].ToString(), (response) =>
        {
            if (response.success)
            {
                done = true;
            } else
            {
                Debug.Log("Failed" + response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator FetchHighscores()
    {

        
        leaderboardNames = new List<string>();
        leaderboardScores = new List<string>();

        foreach (int i in leaderboardIds)
        {
            bool done = false;
            LootLockerSDKManager.GetScoreList(i, 1, 0, (response) =>
           {
               if (response.success)
               {
                   string tempPlayerNames = "";
                   string tempPlayerScores = "";


                   LootLockerLeaderboardMember[] members = response.items;

                   for (int i = 0; i < members.Length; i++)
                   {
                       tempPlayerNames += members[i].rank + ". ";
                       if (members[i].player.name != "")
                       {
                           tempPlayerNames += members[i].player.name;
                       }
                       else { tempPlayerNames += members[i].player.id; }
                       tempPlayerScores += members[i].score;
                   }

                   done = true;
                   //leaderboardNames.Add(tempPlayerNames);
                   //leaderboardScores.Add(tempPlayerScores);
                   pm.leaderboardNames.Add(tempPlayerNames);
                   pm.leaderboardScores.Add(tempPlayerScores);

               }
               else
               {
                   Debug.Log("Failed" + response.Error);
                   done = true;
               }
           });
            pm.SetSO();
            yield return new WaitWhile(() => done == false);
        }
    }
}
