
using UnityEngine;
using UnityEngine.UI;
public class backScript : MonoBehaviour
{

    public Button back;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("escape")) back.onClick.Invoke();
    }
}
