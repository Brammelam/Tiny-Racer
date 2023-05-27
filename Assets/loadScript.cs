using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadScript : MonoBehaviour
{
    [SerializeField]
    Sprite[] ls;

    // Start is called before the first frame update
    void Awake()
    {
        int i = Random.Range(0, 2);
        Debug.Log(i);
        this.GetComponent<Image>().sprite = ls[i];

    }

}
