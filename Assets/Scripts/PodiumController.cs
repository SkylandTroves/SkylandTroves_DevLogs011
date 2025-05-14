using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// observer subject extends monobehavior
public class PodiumController : ObservedSubject
{
    public GameObject Orb;
    [SerializeField] private List<MovingPlatform> movingPlatforms; // List of MovingPlatform objects
    [SerializeField] private Transform orbPositionOnPodium;
    [SerializeField] private float shakeIntensity = 0f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float activationDelay = 1.0f;
    
    private CameraController cameraController;
    void Start()
    {
       cameraController = Camera.main.GetComponent<CameraController>();
    }
    private void MessageTargetObjects()
    {
        // foreach (MovingPlatform targetObject in movingPlatforms)
        // {
        //     targetObject.WasActivated = true;
        //     targetObject.StartMoving();
        // }
        StartCoroutine(ActivatePlatformsWithDelay());
    }
    private IEnumerator ActivatePlatformsWithDelay()
    {
        // Apply screen shake
        if (cameraController != null)
        {
            cameraController.ShakeCamera(shakeIntensity, shakeDuration);
        }
        
        yield return new WaitForSeconds(activationDelay);
        
        foreach (MovingPlatform targetObject in movingPlatforms)
        {
            targetObject.WasActivated = true;
            targetObject.StartMoving();
        }
    }
    public virtual void SnapOrb()
    {
        print("*** SnapOrb: PODIUM CONTROLLER SNAPPING ORB");
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
        if (Orb.CompareTag("energyOrbCharged"))
        {
            Orb.tag = "usedOrb";
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
    /*public virtual void CollisionWithOrb(GameObject other)
    {
        MessageTargetObjects();
        SnapOrb(other);
    }*/
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
    public float GetShakeIntensity()
    {
        return shakeIntensity;
    }

    public float GetShakeDuration()
    {
        return shakeDuration;
    }

    public float GetActivationDelay()
    {
        return activationDelay;
    }

    public void SetShakeIntensity(float intensity)
    {
        shakeIntensity = intensity;
    }

    public void SetShakeDuration(float duration)
    {
        shakeDuration = duration;
    }

    public void SetActivationDelay(float delay)
    {
        activationDelay = delay;
    }
}