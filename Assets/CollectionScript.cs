using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionScript : MonoBehaviour
{
    [SerializeField] GameObject collectionCanvas;
    PlayerManager pm;
    // Start is called before the first frame update
    void Start()
    {
        // Remove collectable if Player has already collected this hat
        if (PlayerPrefs.HasKey(name)) Destroy(gameObject);
    }

    void Update()
    {
        // Keep looking for the PlayerManager until it is found
        pm ??= FindObjectOfType<PlayerManager>();
    }

    // Freeze time and display the CollectionCanvas
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pm.TriggerEvent(name);
            collectionCanvas.SetActive(true);
            Time.timeScale = 0;
            PlayerPrefs.SetInt(name, 1);
            Destroy(gameObject);
        }
    }

    // Ensures time returns to normal
    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
