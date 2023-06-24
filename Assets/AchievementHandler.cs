using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LootLocker.Requests;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Linq;


public class AchievementHandler : MonoBehaviour
{  
    private static AchievementHandler instance;
    [SerializeField] private GameObject achievementPrefab;
    [SerializeField] private Text achievementName, achievementText, achievementProgress;
    [SerializeField] private Animator anim;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        anim = GetComponent<Animator>();
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }


    private void SetAnimationDetails(string achievement)
    {
        Text[] textComponents = GetComponentsInChildren<Text>(true);

        foreach (Text text in textComponents)
        {
            if (text.gameObject.name == "AchievementTitle") achievementName = text;
            if (text.gameObject.name == "AchievementDescription") achievementText = text;
            if (text.gameObject.name == "AchievementProgress") achievementProgress = text;
        }

        // Some achievements will have step-progress like 5-10-15 which will be implemented later
        switch (achievement)
        {
            case "a1":
                achievementName.text = "Stunt pilot";
                achievementText.text = "Flip the car like a true stunt pilot!";
                achievementProgress.text = "1 / 1";
                break;
            case "a2":
                achievementName.text = "Customization Connoisseur";
                achievementText.text = "Unleash your creativity by customizing a car in the Garage!";
                achievementProgress.text = "1 / 1";
                break;
            case "a3":
                achievementName.text = "Car Enthusiast";
                achievementText.text = "Claim a fancy, new car!";
                achievementProgress.text = "1 / 1";
                break;
            case "a4":
                achievementName.text = "Untamed Perfectionist";
                achievementText.text = "Complete a level withouth a single crash!";
                achievementProgress.text = "1 / 1";
                break;
            case "a5":
                achievementName.text = "Fashionista";
                achievementText.text = "Equip a stylish hat and show off your fashion sense!";
                achievementProgress.text = "1 / 1";
                break;
            case "a6":
                achievementName.text = "Beat a global leaderboard score!";
                achievementProgress.text = "1 / 1";
                break;
            case "a7":
                achievementName.text = "";
                achievementProgress.text = "1 / 1";
                break;
            case "a8":
                achievementName.text = "";
                achievementProgress.text = "1 / 1";
                break;
            case "a9":
                achievementName.text = "";
                achievementProgress.text = "1 / 1";
                break;
            case "a10":
                achievementName.text = "";
                achievementProgress.text = "1 / 1";
                break;
        }

        PlayAnimationClip();
    }

    private void PlayAnimationClip()
    {
        anim.Play("achievementanimation");
    }

    public IEnumerator LoadJSONData()
    {
        TaskCompletionSource<bool> requestCompletedTask = new TaskCompletionSource<bool>();
        int achievementId = PlayerPrefs.GetInt("achievements");

        LootLockerSDKManager.GetPlayerProgressions(achievementId, (response) =>
        {
            if (response.success)
            {
                foreach (var achievement in response.items)
                {
                    int points = (int)achievement.points; // Converts ulong to int
                    PlayerPrefs.SetInt(achievement.progression_key, points); // Sets achievement progress to PlayerPrefs by "progression_key
                    if ((int)achievement.step > 0)
                    {
                        PlayerPrefs.SetInt(achievement.progression_key + "_completed", 1); // Sets all completed achievements to 1                        
                    }
                }
            }
            else
            {
                Debug.Log("Error when fetching Player Progression (Achievements)! " + response.Error);
            }

            requestCompletedTask.SetResult(true);

        }); yield return new WaitUntil(() => requestCompletedTask.Task.IsCompleted);
    }

    public IEnumerator UpdateAchievementProgression(string achievementKey)
    {
        TaskCompletionSource<bool> requestCompletedTask = new TaskCompletionSource<bool>();

        LootLockerSDKManager.AddPointsToPlayerProgression(achievementKey, 1, (response) =>
        {
            if (!response.success) Debug.Log("Error adding to achievement progress!");

            if (response.awarded_tiers.Any())
            {
                foreach (var awardedTier in response.awarded_tiers)
                {
                    foreach (var assetReward in awardedTier.rewards.asset_rewards)
                    {
                        SetAnimationDetails(achievementKey);
                    }
                }
            } requestCompletedTask.SetResult(true);

        }); yield return new WaitUntil(() => requestCompletedTask.Task.IsCompleted);
    }
}