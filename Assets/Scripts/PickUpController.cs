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
    private bool pickUpRequested = false;
    private float distancePlayerAndOrb;
    private float maxDistanceToOrb = 2.5f; // Maximum distance to allow pick up
    private Vector3 startingPosition;
    private bool isCharged;

    
    private Rigidbody rigBod;
    private bool hasLanded = false;
    private bool isHeldBeforeOffEdge = false;

    /*void Start()
    {
        rigBod = GetComponent<Rigidbody>();
    }*/
    private void OnCollisionEnter(Collision collision)
    {
        /*if (!hasLanded && collision.gameObject.CompareTag("mapGround"))
        {
            hasLanded = true;
            rigBod.constraints = RigidbodyConstraints.FreezeAll;
            rigBod.velocity = Vector3.zero; // Optional: stop any remaining motion
        }*/
        /*if (collision.gameObject.CompareTag("mapGround"))
        {
            startingPosition = transform.position;
            isHeldBeforeOffEdge = false;
        }*/
    }
    
    public virtual void PickUpObject()
    {
        isHeldBeforeOffEdge = true;
        /*StoreDistanceToPickUp();
        Debug.Log("player is  " + distancePlayerAndOrb + " units away from orb");
        if (distancePlayerAndOrb > maxDistanceToOrb)
            return;*/
        player.HandlePickUpOrbStart();
        print(" *** PickUpObject: picking up object and setting parent");
        
        //putOrbInHands();
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
        
        print(" *** PickUpObject: setting isHoldingObject to TRUE");

        isHoldingObject = true; // Now the orb is being held
        //player.SetHeldOrb(gameObject); // Set this orb as the currently held one
    }

    public void putOrbInHands()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;

        print(transform.name);
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
        player.ToggleWristCollider();
    }
    /*private float lerpSpeed = 5f; // Speed at which the orb moves to the hand
    private Vector3 targetPosition;
    public virtual void PickUpObject()
    {
        print(" *** PickUpObject: picking up object and setting parent");
    
        // Store the target position where the orb should go (hand's front position)
        targetPosition = handFrontPosition.position + handFrontPosition.TransformDirection(new Vector3(0f, 0f, 0.13f)); // Adjust local position slightly to match your original offset
    
        // Parent the orb to the player's hand front position
        transform.SetParent(handFrontPosition);

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
    
        print(" *** PickUpObject: setting isHoldingObject to TRUE");
    
        isHoldingObject = true; // Now the orb is being held

        // Start the smooth movement to the target position
        StartCoroutine(MoveOrbToTarget());
    }

// Coroutine to smoothly move the orb to the hand position
    private IEnumerator MoveOrbToTarget()
    {
        float timeElapsed = 0f;
        Vector3 initialPosition = transform.position;

        while (timeElapsed < 1f)
        {
            // Smoothly interpolate the position of the orb towards the target position
            transform.position = Vector3.Lerp(initialPosition, targetPosition, timeElapsed);

            // Increment timeElapsed based on lerpSpeed
            timeElapsed += Time.deltaTime * lerpSpeed;
        
            // Yield to wait until next frame
            yield return null;
        }

        // Ensure that the orb ends exactly at the target position
        transform.position = targetPosition;
    }*/

    public void DropObject()
    {
        
        player.HandlePickUpOrbEnd();
        print(transform.gameObject.name);
        print(transform.parent.name);
        print("DROPPING ***** ");
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
        /*player.ToggleWristCollider();
        StartCoroutine(ReenableWristTrigger(1f));*/
        isHoldingObject = false;
    }
    private IEnumerator ReenableWristTrigger(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.ToggleWristCollider();
        transform.GetChild(1).GetComponent<Collider>().isTrigger = true;
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

        Debug.Log("You are now clicking on the orb");
        pickUpRequested = true;
        //player.AddToMethodsToCallWhenReachDestination(PickUpObject);
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
        /*transform.position = startingPosition;
        transform.parent = null;*/
        
        /*if (isHeldBeforeOffEdge)
        {*/
            // Return to player’s hand
            /*player.AnimationStates.UpdateState(PlayerStateType.PickupOrb);
            player.HandlePickupState(transform.gameObject);
            PickUpObject();*/
            player.HandleResetOrb(transform.gameObject);
        /*}
        else
        {
            // Normal fallback reset
            transform.position = startingPosition;
            transform.parent = null;
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ChargeArea"))
        {
            isCharged = true;
            Debug.Log("The orb is now charged.");
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