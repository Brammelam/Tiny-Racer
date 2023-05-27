using System.Collections;

using UnityEngine;

public class horn : MonoBehaviour
{
    bool honkActive = false;
    public AudioSource honkSound;
    IEnumerator coroutine;

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            if (!honkActive)
            {
                coroutine = playHonk(0.5f);
                StartCoroutine(coroutine);
            }
        }
    }

    IEnumerator playHonk(float waitTime)
    {
        honkActive = true;
        honkSound.Play(0);
        yield return new WaitForSeconds(waitTime);
        honkActive = false;
    }
}
