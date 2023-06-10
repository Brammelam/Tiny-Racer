using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCar : MonoBehaviour
{
    Transform caringarage;
    // Start is called before the first frame update
    void Start()
    {
        caringarage = GetComponentInChildren<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        caringarage.Rotate(0, -0.9f, 0);
    }
}
