using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeMaterial : MonoBehaviour
{
    [SerializeField]
    List<Material> mymaterials;
    [SerializeField]
    List<Material> newmaterials;

    public Material black;
    MeshRenderer myRend;
    MeshRenderer newRend;
    [SerializeField]
    public PlayerManager pm;



    public int unlockedCheck;

    IEnumerator coroutine;
    // Start is called before the first frame update
    void Awake()
    {
        //black = Resources.Load("black") as Material;

        myRend = gameObject.GetComponentInChildren<MeshRenderer>();
        newRend = gameObject.GetComponentInChildren<MeshRenderer>();
        newRend = myRend;
        newRend.materials = myRend.materials;
        mymaterials = new List<Material>(myRend.materials);

        newmaterials = new List<Material>(newRend.materials);

    }

    public void SetLockedCars()
    {
        int index = transform.GetSiblingIndex() + 1;
        string gotcar = "gotcar";
        string gotcarindex = gotcar + index;

        if (!PlayerPrefs.HasKey(gotcarindex) && (index > 1))
        {
            Material[] mat = newmaterials.ToArray();

            for (int i = 0; i < newmaterials.Count; i++)
            {
                mat[i] = black;
            }

            newRend.materials = mat;
        }       
    }

     // Check which cars are unlocked by the player, defaults to no
     public void Unlock()
    {      

        coroutine = beNormal(0.1f, newmaterials);
        StartCoroutine(coroutine);


    }


    // Sets the material for all LOCKED cars to BLACK
    IEnumerator beBlack(float waitTime, List<Material> materials)
    {
        Material[] mat = materials.ToArray();
        
        for (int i = 0; i < materials.Count; i++)
        {

            mat[i] = black;
        }

        myRend.materials = mat;

        yield return new WaitForSeconds(waitTime);

    }

    // Sets the material for all UNLOCKED cars to NORMAL
    IEnumerator beNormal(float waitTime, List<Material> materials)
    {
        Material[] mat = materials.ToArray();

        for (int i = 0; i < materials.Count; i++)
        {
            mat[i] = materials[i];
        }

        myRend.materials = mat;

        //also update the carsettings
        Color _body = mat[0].color;
        Color _window = mat[1].color;
        int _car = PlayerPrefs.GetInt("car");
        pm.oldcolors[0, _car] = _body;
        pm.oldcolors[1, _car] = _window;

        yield return new WaitForSeconds(waitTime);

    }

}
