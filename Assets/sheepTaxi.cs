using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sheepTaxi : MonoBehaviour
{
    private TaxiGameHandler checkShit;
    public bool hasTriggered;

    private void Start()
    {
        hasTriggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.gameObject.CompareTag("Player"))
        {
            hasTriggered = true;
            this.GetComponent<Collider>().isTrigger = false;
            Rigidbody rb = this.GetComponent<Rigidbody>();

            Vector3 direction = other.transform.position - transform.position;
            direction.Normalize();

            checkShit = FindObjectOfType<TaxiGameHandler>();

            checkShit.FlipCar();

        }
    }
}
