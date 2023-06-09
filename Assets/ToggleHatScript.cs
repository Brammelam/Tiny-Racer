using UnityEngine;

public class ToggleHatScript : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToToggle;

    public void ToggleDisplay()
    {
        objectToToggle.SetActive(!objectToToggle.activeSelf);
    }
}