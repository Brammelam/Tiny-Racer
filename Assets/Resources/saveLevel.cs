using System.Collections.Generic;
using UnityEngine;

public class saveLevel : MonoBehaviour
{
    public List<float> lap = new List<float>();
    public bool lapCompleted = false;
    public int saveIndex = 0;
    private static string saveGhost = "saveGhost";
    public int currentLevel = 0;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Update()
    {
        if (lapCompleted)
        {
            if (saveIndex < lap.Count)
            {
                PlayerPrefs.SetFloat(saveGhost + saveIndex + currentLevel, lap[saveIndex]);
                PlayerPrefs.Save();
                saveIndex++;
                if (saveIndex == lap.Count)
                {
                    PlayerPrefs.SetFloat(saveGhost + saveIndex + currentLevel, -1);
                    PlayerPrefs.Save();
                    lapCompleted = false;

                    lap = new List<float>();
                    Destroy(gameObject);
                }
            }
        }
    }
}