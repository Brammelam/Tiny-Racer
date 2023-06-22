using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadScript : MonoBehaviour
{
    [SerializeField]
    Texture2D[] ls;
    [SerializeField] Image backgroundColor;

    // Start is called before the first frame update
    void Start()
    {
        int i = Random.Range(0, 1);

        this.GetComponent<RawImage>().texture = ls[i];

        switch (i)
        {
            case 0:
                backgroundColor.color = new Color32(156, 222, 243, 255);
                break;
            case 1:
                backgroundColor.color = new Color32(122, 209, 255, 255);
                break;
        }

    }
}