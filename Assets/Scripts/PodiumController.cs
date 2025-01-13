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

// observer subject extends monobehavior 
public class PodiumController : ObservedSubject
{
    public GameObject Orb;
    
    [SerializeField] private List<MovingPlatform> movingPlatforms; // List of MovingPlatform objects
    // in the editor say how many platforms are effected then wire up their game objects 
    
    [SerializeField] private Transform orbPositionOnPodium;
    
    void Start()
    {
        //NotifyObservers();
    }

    private void MessageTargetObjects()
    {
        foreach (MovingPlatform targetObject in movingPlatforms)
        {
            targetObject.WasActivated = true; 
            targetObject.StartMoving();
        }
    }
    
    public virtual void SnapOrb()
    {
        // Parent the orb to the podium
        Orb.transform.SetParent(orbPositionOnPodium);
        Orb.transform.localPosition = Vector3.zero; // Reset local position to place the orb exactly in the hand's front position

        // Disable physics on TriggerObject (the orb) while it's held
        Rigidbody orbRB = Orb.GetComponent<Rigidbody>();
        if (orbRB != null)
        {
            orbRB.useGravity = false;
            orbRB.isKinematic = true;
        }
    }
    
    public virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == Orb)
        {
            MessageTargetObjects();
            SnapOrb();
        }
    }
    
    // GETTERS AND SETTERS 
    public GameObject GetOrb()
    {
        return Orb;
    }
    
    public List<MovingPlatform> GetMovingPlatformsList()
    {
        List<MovingPlatform> myPlatforms = new List<MovingPlatform>();
        foreach (MovingPlatform platform in movingPlatforms)
        {
            myPlatforms.Add(platform);
        }
    
        return myPlatforms;
    }

    public Transform GetOrbPositionOnPodium()
    {
        return orbPositionOnPodium;
    }

    public void SetOrb(GameObject orb)
    {
        this.Orb = orb;
    }
    
    public void SetMovingPlatforms(List<MovingPlatform> movingPlatform)
    {
        movingPlatforms = new List<MovingPlatform>();
    
        foreach (MovingPlatform platform in movingPlatform)
        {
            movingPlatforms.Add(platform);
        }
    }

    public void SetOrbPositionOnPodium(Transform orbPosition)
    {
        this.orbPositionOnPodium = orbPosition;
    }
    
    
}
