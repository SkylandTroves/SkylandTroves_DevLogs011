/*
Full Name: Aliya Rafei
Student ID:  2391746
rafei@chapman.edu
GAME 340 - 01
Assignment:  Final Project Submission
*/
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class PickUpController : MonoBehaviour
{
    public float checkWhenToRestOrbValue;
    [SerializeField] protected PlayerController player;  
    [SerializeField] private Transform handFrontPosition; 
    
    private bool isHoldingObject = false; // Track whether the object is currently being held
    private bool pickUpRequested = false;
    private float distancePlayerAndOrb;
    private float maxDistanceToOrb = 2.5f; // Maximum distance to allow pick up 
    private Vector3 startingPosition; 
    private bool isCharged;
    
    public void DropObject()
    {
        if (isHoldingObject)
        {
            transform.SetParent(null); // Un-parent the object from the hand
            Rigidbody rb = GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                rb.useGravity = true; // make it fall down to floor again
                rb.isKinematic = false; // Enable physics again
            }

            isHoldingObject = false; // The object is no longer being held
        }
        
    }

    public bool GetIsHoldingObject()
    {
        return isHoldingObject; 
    }

    // awake goes before start 
    private void Awake()
    {
        startingPosition = transform.position;
    }

    private void Update()
    {
        CheckIfOrbFellOfMap();
    }
    
   
    private void OnMouseDown()
    {
        Debug.Log("You are now clicking on the orb");
        pickUpRequested = true;
        player.AddToMethodsToCallWhenReachDestination(PickUpObject);
        
    }

    private void CheckIfOrbFellOfMap()
    {
        if (transform.position.y < checkWhenToRestOrbValue)
        {
            ResetObject();
        }
            
    }

    private void ResetObject()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero; // Vector3.zero = (0,0,0)
        rb.angularVelocity = Vector3.zero;
        transform.position = startingPosition;
        
    }
    
    public virtual void PickUpObject()
    {
        StoreDistanceToPickUp();
        Debug.Log("player is  " + distancePlayerAndOrb + " units away from orb");
        if (distancePlayerAndOrb > maxDistanceToOrb)
            return;
        
        Debug.Log("You can pick up the orb");

    
        // Parent the orb to the player's hand front position
        transform.SetParent(handFrontPosition);
        transform.localPosition = Vector3.zero; // Reset local position to place the orb exactly in the hand's front position

        // Disable physics while holding
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        isHoldingObject = true; // Now the orb is being held
        player.SetHeldOrb(gameObject); // Set this orb as the currently held one
        
    }
    
    public void StoreDistanceToPickUp()
    {
        distancePlayerAndOrb = Vector3.Distance(player.transform.position, transform.position);
    }
    
    
    
    // GETTERS AND SETTERS 

    public PlayerController GetPlayerController()
    {
        return player;
    }
    
    public Transform GetHandFrontPosition()
    {
        return handFrontPosition;
    }

    public bool GetIsChargedVar()
    {
        return isCharged;
    }
    

    public void SetPlayerController(PlayerController player)
    {
        this.player = player;
    }
    
    public void SetHandFrontPosition(Transform transform)
    {
        handFrontPosition = transform;
    }

    public void SetIsChargedVar(bool isCharged)
    {
        this.isCharged = isCharged;
    }
}
