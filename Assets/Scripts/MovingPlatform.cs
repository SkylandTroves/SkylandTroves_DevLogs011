/*
Full Name: Aliya Rafei
Student ID:  2391746
rafei@chapman.edu
GAME 340 - 01
Assignment:  Final Project Submission
*/
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MovingPlatform : MonoBehaviour
{
    public float maxSpeed = 2f;
    public bool UsesWheel = false;
    public Transform positionA;
    public Transform positionB;
    [SerializeField] private float SecondsToMove;
    [SerializeField] private bool isBoat;
    
    // DO NOT EDIT IN EDITOR -- only public for debugging reasons
    public float wheelProgress;
    public float platformProgress;
    public bool WasActivated = false;
    // - - - - - - - - - - - - - - - - - - -
    
    private string platformType;
    
    private void Awake()
    {
        if (gameObject.CompareTag("SmallPlatform"))
        {
            platformType = "SmallPlatform";
        }
        else if (gameObject.CompareTag("MediumPlatform"))
        {
            platformType = "MediumPlatform";
        }
        else if (gameObject.CompareTag("BigPlatform"))
        {
            platformType = "BigPlatform";
        }
        else
        {
            platformType = "Unknown";
        }
    }
    
    private void OnDestroy()
    {
        if (SoundController.instance != null)
        {
            SoundController.instance.StopPlatformSound(platformType, gameObject.GetInstanceID());
        }
    }

    private void Update()
    {
        //UpdateMyPositionWithWheel();
    }

    private void FixedUpdate()
    {
        UpdateMyPositionWithWheel();
    }

    protected virtual void UpdateMyPositionWithWheel()
    {
        if (!UsesWheel)
            return;
        
        float smoothedProgress = Mathf.Lerp(platformProgress,wheelProgress,0.2f);
        
        platformProgress = smoothedProgress;
        Vector3 positionC =  Vector3.Lerp(positionA.position, positionB.position, smoothedProgress);
        transform.position = positionC;
    }

    public void StartMoving()
    {
        if (isBoat && gameObject.CompareTag("movingPlatform"))
        {
            Boat boat = GetComponent<Boat>();  
            if (boat != null)  
            {
                boat.OnMovingPlatformStarted();  
            }
            // do nothing if platform is not a boat
        }
        
        PlayPlatformSoundEffect();
        
        StartCoroutine(Move());
    }

    private void PlayPlatformSoundEffect()
    {
        if (SoundController.instance != null && platformType != "Unknown")
        {
            SoundController.instance.PlayPlatformSound(platformType, transform);
        }
    }
        
    IEnumerator Move()
    {
        float timeElapsed = 0f;

        while (timeElapsed < SecondsToMove)
        {
            Vector3 newPosition = Vector3.Lerp(positionA.position, positionB.position, timeElapsed / SecondsToMove);
            transform.position = newPosition;

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = positionB.position;
        
        if (SoundController.instance != null)
        {
            SoundController.instance.StopPlatformSound(platformType, gameObject.GetInstanceID());
        }
    }

    public void StartMovingBackwards()
    {
        PlayPlatformSoundEffect();
        
        StartCoroutine(MoveBackwards());
    }

    IEnumerator MoveBackwards()
    {
        float timeElapsed = 0f;

        while (timeElapsed < SecondsToMove)
        {
            Vector3 newPosition = Vector3.Lerp(positionB.position, positionA.position, timeElapsed / SecondsToMove);
            transform.position = newPosition;

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = positionA.position;
        
        if (SoundController.instance != null)
        {
            SoundController.instance.StopPlatformSound(platformType, gameObject.GetInstanceID());
        }
    }
    
    //getters and setters
    public float GetWheelProgress()
    {
        return wheelProgress;
    }

    public void SetWheelProgress(float progress)
    {
        wheelProgress = progress;
    }

    public Vector3 GetPositionA()
    {
        return positionA.transform.position;
    }
    
    public Vector3 GetPositionB()
    {
        return positionB.transform.position;
    }

    private Rigidbody orbRigidBody;
    private FixedJoint joint;
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "energyOrbCharged" || other.gameObject.tag == "energyOrbUncharged")
        {
            if(transform.tag == "Mover")
            {
                other.gameObject.transform.parent = transform;
            }

            if (transform.tag == "Pusher")
            {
                joint = transform.AddComponent<FixedJoint>();
                orbRigidBody = other.gameObject.GetComponent<Rigidbody>();
                joint.connectedBody = orbRigidBody;
                joint.breakForce = 100f;     // Tweak this value
                joint.breakTorque = 100f;

            }
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (other.rigidbody == orbRigidBody && joint != null)
        {
            Destroy(joint);
            joint = null;
            Debug.Log("Orb detached from platform.");
        }
    }
    
    void OnJointBreak(float breakForce)
    {
        Debug.Log("Joint broke! Force: " + breakForce);
        joint = null;
    }

}