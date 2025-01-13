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

public class UnchargedPodium : PodiumController
{
    [SerializeField] private GameObject chargedPodiumPrefab; // Reference to the charged orb prefab

    public override void OnCollisionEnter(Collision other)
    {
        bool isCharged = other.gameObject.CompareTag("energyOrbCharged");
        if (other.gameObject == Orb && isCharged)
        {
            SwitchToChargedPodium();
        }
        else if (other.gameObject == Orb &&!isCharged)
        {
            Debug.Log("put uncharged on podium");
            PutUnChargedOnPodium();
        }
        
    }

    public override void SnapOrb()
    {
        base.SnapOrb();
    }

    private void SwitchToChargedPodium()
    {
        if (chargedPodiumPrefab != null)
        {
            // Instantiate the charged podium at the current position and rotation of the uncharged pofium
            GameObject newPodium = Instantiate(chargedPodiumPrefab, transform.position, transform.rotation);

            PodiumController podiumController = newPodium.GetComponent<PodiumController>();
            podiumController.SetOrb(GetOrb());
            podiumController.SetMovingPlatforms(GetMovingPlatformsList());
            podiumController.transform.SetParent(transform.parent);
            
            
            GetOrb().GetComponent<Rigidbody>().useGravity = false;
            GetOrb().GetComponentInChildren<Collider>().enabled = false; 
            GetOrb().transform.SetParent(podiumController.GetOrbPositionOnPodium());
            GetOrb().transform.localPosition = Vector3.zero;
            
            
            //destroy the uncharged podium (or deactivate it)
            Destroy(gameObject); //ameObject.SetActive(false) //to disable instead
        }
        else
        {
            Debug.LogError("Charged podium prefab is not assigned!");
        }
    }

    private void PutUnChargedOnPodium()
    {
        base.SnapOrb();
    }
    
     
    
    
}
