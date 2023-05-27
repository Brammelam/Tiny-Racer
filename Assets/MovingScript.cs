using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingScript : MonoBehaviour
{
    Vector3 _v = new Vector3(1f, -1f, 0f);
    [SerializeField]
    private float _angle;

    private void Update()
    {
        this.transform.Translate(-Vector3.right*0.2f, Space.Self);
        if (this.transform.position.x <= -4000)
            this.transform.localPosition += new Vector3(8000, 0, 0);

    }
}
