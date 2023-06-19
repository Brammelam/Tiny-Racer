using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public GameObject player; // Reference to the player game object

    private void Update()
    {
        if (player == null) player = FindObjectOfType<checkShit>().GetComponent<GameObject>();
        // Check if the colliders of this game object and the player game object are overlapping
        if (CheckColliderOverlap())
        {
            Debug.Log("Colliders are overlapping!");
        }
    }

    private bool CheckColliderOverlap()
    {
        // Get the colliders of this game object and the player game object
        Collider thisCollider = GetComponent<Collider>();
        Collider playerCollider = player.GetComponent<Collider>();

        // Check if the bounds of the colliders intersect
        return thisCollider.bounds.Intersects(playerCollider.bounds);
    }
}