using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowCollision : MonoBehaviour
{
    private checkShit checkShit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            Rigidbody rb = this.GetComponent<Rigidbody>();

            Vector3 direction = other.transform.position - transform.position;
            direction.Normalize();


            checkShit = FindObjectOfType<checkShit>();
            checkShit.FlipCar();
            rb.isKinematic = false;
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            rb.AddForce(direction * 10f, ForceMode.Impulse);

            GetComponentInParent<CowScript>().enabled = false;
            GetComponentInParent<Animator>().enabled = false;
        }
    }

}
