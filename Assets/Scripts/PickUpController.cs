/*
Full Name: Aliya Rafei
Student ID:  2391746
rafei@chapman.edu
GAME 340 - 01
Assignment:  Final Project Submission
*/

using System;
using System.Collections;
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
    private float distancePlayerAndOrb;
    private Vector3 startingPosition;
    private bool isCharged;

    
    private Rigidbody rigBod;

    
    public virtual void PickUpObject()
    {
        player.HandlePickUpOrbStart();
        
        // Parent the orb to the player's hand front position
        transform.SetParent(handFrontPosition);
        transform.localPosition = new Vector3(0f, 0f, .13f); // Reset local position to place the orb exactly in the hand's front position

        // Disable physics while holding
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // Disable the collider to prevent blocking ray casts
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        isHoldingObject = true; // Now the orb is being held
    }

    public void putOrbInHands()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;

        transform.SetParent(handFrontPosition);
        transform.localPosition = new Vector3(0f, 0f, .13f);
        
        
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // Disable the collider to prevent blocking ray casts
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
    
    public void DropObject()
    {
        
        player.HandlePickUpOrbEnd();
        transform.SetParent(null); // Un-parent the object from the hand
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = true; // make it fall down to floor again
            rb.isKinematic = false; // Enable physics again
        }

        // Re-enable the collider
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        isHoldingObject = false;
    }

    public void StoreDistanceToPickUp()
    {
        distancePlayerAndOrb = Vector3.Distance(player.transform.position, transform.position);
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
        if (isHoldingObject)
            return;
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
        
        player.HandleResetOrb(transform.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ChargeArea"))
        {
            isCharged = true;
        }
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