using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour
{
    public Text loadingText;
    private bool isCycling = true;

    private void Start()
    {
        StartCoroutine(CycleLoadingText());
    }

    private IEnumerator CycleLoadingText()
    {
        while (isCycling)
        {
            loadingText.text = "Loading.";
            yield return new WaitForSeconds(0.5f);

            loadingText.text = "Loading..";
            yield return new WaitForSeconds(0.5f);

            loadingText.text = "Loading...";
            yield return new WaitForSeconds(0.5f);

            loadingText.text = "Loading..";
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(CycleLoadingText());
    }
}
