using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class starHandler : MonoBehaviour
{
    public Animator starAnimator;

    // Start is called before the first frame update
    void Start()
    {
        int crashCount = PlayerPrefs.GetInt("crashed");
        int starCount = Mathf.Clamp(5 - crashCount, 1, 5);

        starAnimator.SetInteger("starCount", starCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
