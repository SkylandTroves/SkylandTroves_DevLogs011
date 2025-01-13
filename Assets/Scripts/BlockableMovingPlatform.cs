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
using UnityEngine;

public class BlockableMovingPlatform : MovingPlatform
{
    [SerializeField] private MovingPlatform blockerPlatform;
    [SerializeField] private float maxBlockerProgress;
    // wheel progress of the blocking platform when its at a position to block the object with this script
    
    [SerializeField] private float maxBlockedProgress; 
    // wheel progress of how much the platform can move before it would be blocked by the other platform 

    //public float cappedProgress; //  << VAR FOR DEBUGGING - do not delete yet 
    
    protected override void UpdateMyPositionWithWheel()
    {
        if (!UsesWheel)
            return;
        
        wheelProgress = GetMaxMovementAllowed(wheelProgress);
        
        // Smoothly transition to the capped progress
        float smoothedProgress = Mathf.Lerp(platformProgress, wheelProgress, Time.deltaTime * 5f); // Adjust smoothing speed
        platformProgress = smoothedProgress;
    
        Vector3 positionC = Vector3.Lerp(positionA.position, positionB.position, smoothedProgress);
        transform.position = positionC;
        
    }

    private float GetMaxMovementAllowed(float progress)
    {
        float maxSelf = 1f; // Default maximum progress
        bool forceChangeWheelProgress = false;

        if (!blockerPlatform.UsesWheel)
        {
            while (progress > maxBlockedProgress && (!blockerPlatform.WasActivated))
            {
                progress = maxBlockedProgress;
            }
            
        }
        else
        {
            // Check if blocker exceeds its allowed progress
            if (blockerPlatform.platformProgress > maxBlockerProgress)
            {
                maxSelf = maxBlockedProgress;
                forceChangeWheelProgress = true;
            }
        }

        // Smoothly adjust the progress to avoid jumps
        progress = Mathf.Min(progress, maxSelf);
        if (forceChangeWheelProgress)
        {
            wheelProgress = Mathf.Lerp(wheelProgress, progress, Time.deltaTime * 5f);
        }

        //cappedProgress = progress;
        return progress;
        
    }
    
    
    
    
    
}
