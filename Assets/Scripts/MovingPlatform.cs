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
        // go from this number (a) to this number(b), amount per frame (per times we call it)
        
        platformProgress = smoothedProgress;
        Vector3 positionC =  Vector3.Lerp(positionA.position, positionB.position, smoothedProgress);
        transform.position = positionC;
        
    }

    public void StartMoving()
    {
        if (isBoat)
        {
            Boat boat = GetComponent<Boat>();  
            if (boat != null)  
            {
                boat.OnMovingPlatformStarted();  
            }
            // do nothing if platform is not a boat
        }
        
        StartCoroutine(Move());
    }
    
    public float maxSpeed = 2f;

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
    }
    
    
    // GETTERS + SETTERS

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
    
    
    
    
    
    
    
    
    
    
    
}
