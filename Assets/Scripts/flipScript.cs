using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipScript : MonoBehaviour
{
    public Rigidbody rb2;
    public GameObject gh;
    public checkShit check;


	void Start()
	{
        gh = GameObject.Find("GameHandler");
        check = gh.GetComponent<checkShit>();
        rb2 = GetComponent<Rigidbody>();
        AddForce();
    }

    void AddForce()
    {
        rb2.AddForce(transform.forward*15f, ForceMode.VelocityChange);
        rb2.AddForce(Vector3.up * 8f, ForceMode.VelocityChange);
        if (check.rt.angle > 0)
        {
            rb2.AddTorque(-check.direction * 40f, ForceMode.VelocityChange);
            rb2.AddForce(transform.right * 10f, ForceMode.VelocityChange);
        }
        if (check.rt.angle < 0)
        {
            rb2.AddTorque(check.direction * 40f, ForceMode.VelocityChange);
            rb2.AddForce(-transform.right * 10f, ForceMode.VelocityChange);
        }


    }

}
