using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class prefabscriptls : MonoBehaviour
{
    List<GameObject> prefabList = new List<GameObject>();
    [SerializeField]
    private GameObject Prefab1;
    [SerializeField] 
    public GameObject Prefab2;
    [SerializeField] 
    public GameObject Prefab3;

    public void Start()
    {
        prefabList.Add(Prefab1);
        prefabList.Add(Prefab2);
        prefabList.Add(Prefab3);

        int prefabIndex = Random.Range(0, 3);

        Instantiate(prefabList[prefabIndex], this.transform);

    }
}
