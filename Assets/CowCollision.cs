using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowCollision : MonoBehaviour
{
    private checkShit checkShit;
    [SerializeField] GameObject collectionCanvas;
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
            BoxCollider collider = GetComponent<BoxCollider>();
            collider.isTrigger = false;

            Rigidbody rb = this.GetComponent<Rigidbody>();

            Vector3 direction = other.transform.position - transform.position;
            direction.Normalize();

            checkShit = FindObjectOfType<checkShit>();

            // Check if we have collided 3 times and if so, unlock the hat
            string animal = name;
            string animalCollision = animal + "collision";
            int numberOfCollisions = PlayerPrefs.GetInt(animalCollision, 0);
            numberOfCollisions += 1;

            PlayerPrefs.SetInt(animalCollision, numberOfCollisions);
            PlayerPrefs.Save();
            //Debug.Log("Collided with " + animal + " " + numberOfCollisions + " times!");

            if (numberOfCollisions == 5)
            {
                collectionCanvas.SetActive(true);
                checkShit.pm.TriggerEvent(animal);
                PlayerPrefs.SetInt(animal, 1);
                PlayerPrefs.Save();
            }
            
            checkShit.FlipCar();

            GetComponentInParent<CowScript>().enabled = false;
           
        }
    }
}
