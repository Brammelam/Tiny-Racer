using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowScript : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        this.transform.position += transform.forward/10f;
        if (Mathf.Abs(this.transform.position.z) >= 50f)
        {
            if (this.transform.position.z > 0)
                this.transform.position -= transform.forward * 50f;
            else
                this.transform.position += transform.forward * 50f;
        }

    }

}
