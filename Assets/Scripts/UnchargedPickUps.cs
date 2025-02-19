/*
Full Name: Aliya Rafei
Student ID:  2391746
rafei@chapman.edu
GAME 340 - 01
Assignment:  Final Project Submission
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnchargedPickUps : PickUpController
{
    [SerializeField] private GameObject chargedOrbPrefab; // Reference to the charged orb prefab
    private bool isOrbCharged = false;

    // Override the PickUpObject method from the base class if needed
    public override void PickUpObject()
    {
        base.PickUpObject(); // Call the base class implementation for picking up the object
        // You can add additional behavior here if needed
    }

    public bool GetIsChargedVar()
    {
        return isOrbCharged;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ChargeArea"))
        {
            Debug.Log("Orb entered charge area, switching to charged version...");
            SwitchToChargedOrb(); // Switch to the charged orb version when in the charge area
        }
    }

    private void SwitchToChargedOrb()
    {
        if (chargedOrbPrefab != null)
        {
            // Instantiate the charged orb at the current position and rotation of the uncharged orb
            GameObject newOrb = Instantiate(chargedOrbPrefab, transform.position, transform.rotation);

            PickUpController pickUpController = newOrb.GetComponent<PickUpController>();
            pickUpController.SetPlayerController(GetPlayerController());
            pickUpController.SetHandFrontPosition(GetHandFrontPosition());

            PlayerController playerController = GetPlayerController();
            playerController.SetPickUpController(pickUpController);

            isOrbCharged = true;
            pickUpController.SetIsChargedVar(isOrbCharged);

            GetUnchargedPodium(newOrb);
            
            
            /*  - - - - - - - - - - alternative method - by Jon - - - - - - - - - - - 
            GameObject gameObject = GameObject.Find("Player");
            PlayerController pc = gameObject.GetComponent<PlayerController>();
            pickUpController.SetPlayerController(pc);
           */ // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            
            
            player.SetHeldOrb(newOrb);
            //destroy the uncharged orb (or deactivate it)
            Destroy(gameObject); //ameObject.SetActive(false) //to disable instead
            
            
            Rigidbody newOrbRB = newOrb.GetComponent<Rigidbody>();
            if (newOrbRB != null)
            {
                newOrbRB.useGravity = true;
                newOrbRB.isKinematic = false;
            }
            
            // Check if the uncharged orb was being held before calling PickUpObject
            if (GetIsHoldingObject())
            {
                pickUpController.PickUpObject();
            }
            
        }
        else
        {
            Debug.LogError("Charged orb prefab is not assigned!");
        }
    }

    private void GetUnchargedPodium(GameObject newOrb)
    {
        List<PodiumController> unChargedPodiums = new List<PodiumController>();
        
        // Find all GameObjects with the tag "podiumUncharged"
        GameObject[] podiums = GameObject.FindGameObjectsWithTag("podiumUncharged");

        // Iterate over all podiums and add them to the list
        foreach (GameObject podium in podiums)
        {
            PodiumController podiumController = podium.GetComponent<PodiumController>();

            if (podiumController != null)
            {
                unChargedPodiums.Add(podiumController);
                podiumController.SetOrb(newOrb);
            }
        }
        
    }

   
    
}

