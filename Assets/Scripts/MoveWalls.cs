using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWalls : MonoBehaviour
{
    [SerializeField] private List<MovingPlatform> movingPlatforms; // List of MovingPlatform objects
    // in the editor say how many platforms are effected then wire up their game objects
    private void MessageTargetObjects()
    {
        foreach (MovingPlatform targetObject in movingPlatforms)
        {
            targetObject.WasActivated = true;
            targetObject.StartMovingBackwards();
        }
    }
    
    public void OnTriggerEnter(Collider other)
    {
        print("DEBUG COLLIS" + other.gameObject.tag);
        if (other.gameObject.CompareTag("Player"))
        {
            print("MOVING BACWARDS");
            MessageTargetObjects();
        }
    }
    
    
}
