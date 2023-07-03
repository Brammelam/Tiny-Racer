using Coherence.UI;
using UnityEngine;
using TMPro;
using Coherence.Toolkit;

// Creates a TextMeshPro Object from a Prefab.
// Finds a canvas in the Scene and attaches the TextMeshPro Object to it.
// Looks for the player name in World or Room ConnectDialog.
// Updates the position of the name tag in update.

// nameTagPrefab can be just a default TextMeshPro Object.

public class onlineName : MonoBehaviour
{
    public GameObject nameTagPrefab;
    public TextMeshPro text;
    public Vector2 offset = new Vector2(0, 2);
    public string playerName; // Remember to sync this variable! 

    CoherenceSync sync;

    void Start()
    {
        sync = GetComponent<CoherenceSync>();

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) Debug.Log("couldn't find a canvas to parent NameTag to");
        if (nameTagPrefab == null) Debug.Log("NameTag prefab is null!");

        // Only get player name if we have Authority, which gets synced with other Clients. 
        if (sync.HasStateAuthority)
        {
            playerName = NetworkDialog.PlayerName;
            text.text = playerName;
        }
    }

    void Update()
    {
        nameTagPrefab.transform.LookAt(Camera.main.transform);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        // Set the object's rotation to match the camera's rotation
        nameTagPrefab.transform.rotation = cameraRotation;

        if (text.text != playerName) text.text = playerName;
    }

    private void OnDestroy()
    {
        if (text != null) Destroy(text.gameObject);
    }
}