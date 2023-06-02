using UnityEngine;
using System.Collections;
using System;

public class PlayerManagerManager : MonoBehaviour
{
    public static PlayerManager ActiveInstance { get; private set; }

    [SerializeField]
    private PlayerManager playerManager;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        if (playerManager == null)
        {
            Debug.LogError("No PlayerManager found in the scene!");
        }
        else
        {
            if (ActiveInstance != null)
            {
                if (playerManager != ActiveInstance)
                {
                    Destroy(ActiveInstance.gameObject);
                }
            }

            ActiveInstance = playerManager;

            DontDestroyOnLoad(playerManager.gameObject);       
        }
    }
}