using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public AnimationStates AnimationStates;
    private Vector3 playerPosition;
    private Dictionary<String, List<ObjectInformation>> objectTypeToPositions;
    
    private PlayerStateType currentState;
    private float maximumDistance = 3f; //max distance player can be from object for interaction

    private GameObject playerObject;
    public PlayerController playerController;
    public GameObject currentInteractable;
    public void Start()
    {
        playerObject = GameObject.FindWithTag("Player");
        playerController = playerObject.GetComponent<PlayerController>();
    }

    private void Update()
    {
        playerController.HandleCurrentState(currentState, currentInteractable);
//        StateDebugText.text = currentState.ToString();
    }

    // needs to know position of player and all objects on level: orbs, pedestals, wheels, swtiches, buttons
    // also needs to know if the player is carrying an orb
    public void UpdateStateOnStartMoving()
    {
        //Debug.Log("update state start moving");

        if (playerController.IsCarryingOrb())
        {
            //Debug.Log("current state: walkorb");
            currentState = PlayerStateType.WalkWithOrb;
        }
        else
        {
            //Debug.Log("current state: walk");
            currentState = PlayerStateType.WalkWithoutOrb;
        }
        //Debugger.UpdateState(currentState.ToString());
        AnimationStates.UpdateState(currentState);
    }

    public void UpdateStateOnStopMoving()
    {
        if (currentState == PlayerStateType.WalkWithOrb || currentState == PlayerStateType.WalkWithoutOrb 
                                                        || currentState == PlayerStateType.IdleWithOrb)
        {
            //Debug.Log("update state stop moving ");
            GetAllPositions();
            
            // get distances from player to all orbs, pedestals, wheels, switches, buttons
            foreach (KeyValuePair<string,List<ObjectInformation>> keyValuePair in objectTypeToPositions)
            {
                foreach (ObjectInformation objectInformation in keyValuePair.Value)
                {
                    objectInformation.Distance = Vector3.Distance(objectInformation.Position, playerPosition);
                }
            }
            
            ObjectInformation closestObject = FindClosestObject();

            
            // figure out which object we're closest to
            // if we're closest to a pedestal, if the pedestal has an orb on it, we're closest to the orb
            // if the smallest distance is too far to be triggering an object 
            if (closestObject != null)
            {
                print("*** UpdateStateOnStopMoving: closest object is " + closestObject.ObjectType);
                
                if (closestObject.Distance > maximumDistance)
                {
                    print("*** UpdateStateOnStopMoving: closest object was greater than max distance");
                    
                    currentInteractable = null;
                    if (playerController.IsCarryingOrb())
                    {
                        Debug.Log("current state set to: idle with orb");
                        currentState = PlayerStateType.IdleWithOrb;
                    }
                    else
                    {
                        Debug.Log("current state set to: idle");
                        currentState = PlayerStateType.IdleWithoutOrb;
                    }
                }
                else
                {
                    if (closestObject.ObjectType == "Orb")
                    {
                        Debug.Log("current state set to: pickup");
                        currentInteractable = closestObject.ObjectGameObject;
                        currentState = PlayerStateType.NextToOrb;
                    }
                    else if (closestObject.ObjectType == "Wheel")
                    {
                        Debug.Log("current state set to: next to wheel");
                        currentInteractable = closestObject.ObjectGameObject;
                        currentState = PlayerStateType.NextToWheel;
                    }

                    else if (closestObject.ObjectType == "Podium")
                    {
                        currentInteractable = closestObject.ObjectGameObject;
                        if (playerController.IsCarryingOrb())
                        {
                            Debug.Log("current state set to: next to podium with orb");
                            currentState = PlayerStateType.NextToPodiumWithOrb;
                        }
                        else
                        {
                            Debug.Log("current state set to: next to podium without orb");
                            currentState = PlayerStateType.NextToPodiumWithoutOrb;
                        }
                    }
                }
            }
            else
            {
                print("*** UpdateStateOnStopMoving: closest object was null");
            }
            
            if (playerController.IsCarryingOrb())
            {
                AnimationStates.UpdateState(PlayerStateType.IdleWithOrb);
                print("*** UPdate on Stop: is carrying orb ");
            }
            else
            {
                AnimationStates.UpdateState(PlayerStateType.IdleWithoutOrb);
                print("*** UPdate on Stop: is not carrying orb ");

            }
            
            //Debugger.UpdateState(currentState.ToString());
        }
    }

    private ObjectInformation FindClosestObject()
    {
        
        /*
         * Is carrying orb:
         *      ignore all orbs
         *      ignore all podiums that already have orbs on them
         * Not carrying orb:
         *      if closest is a podium and the podium has an orb on it, closest is the orb
         */

        if (playerController.IsCarryingOrb())
        {
            print("*** FindClosestObject: We're carrying an orb");
            objectTypeToPositions.Remove("Orb");

            foreach (var key in objectTypeToPositions.Keys.ToList())
            {
                // Check if the key is "Podium"
                if (key == "Podium")
                {
                    // Remove entries where PodiumHasOrb() is true
                    objectTypeToPositions[key].RemoveAll(objInfo => PodiumHasOrb(objInfo.ObjectGameObject));

                    // If the list is empty, remove the key from the dictionary
                    if (objectTypeToPositions[key].Count == 0)
                    {
                        objectTypeToPositions.Remove(key);
                    }
                }
            }
        }
        else
        {
            print("*** FindClosestObject: We're NOT carrying an orb");
        }

        // get the object closest to the player
        ObjectInformation closestObject = objectTypeToPositions
            .SelectMany(kv => kv.Value) // Flatten all ObjectInformation lists into a single collection
            .OrderBy(obj => obj.Distance) // Order by Distance
            .FirstOrDefault(); // Get the closest one or null if empty
        
        Debug.Log("*** FindClosestObject: closest object is "+ closestObject.ObjectType);

        if (!playerController.IsCarryingOrb())
        {
            print("*** FindClosestObject: We're NOT carrying an orb");
            
            // if the closest object is a podium and it has a child orb on it
            if (closestObject.ObjectType == "Podium" && PodiumHasOrb(closestObject.ObjectGameObject))
            {
                print(" *** FindClosestObject: found child orb on podium");
                // set closest object to closest orb 
                closestObject = objectTypeToPositions
                    .Where(kv => kv.Key == "Orb") // Filter only "Orb" objects
                    .SelectMany(kv => kv.Value) // Flatten the lists
                    .OrderBy(obj => obj.Distance) // Order by Distance
                    .FirstOrDefault();
            }
        }
        
        return closestObject;
    }

    private bool PodiumHasOrb(GameObject podiumObject)
    {
        if (podiumObject.transform.GetChild(0).transform.childCount > 0)
            return true;
        return false;
    }

    private GameObject GetOrbOnPodium(GameObject podiumObject)
    {
        if (podiumObject.transform.childCount == 0)
        {
            Debug.Log("Error: no orb on podium");
            return null;
        }

        return podiumObject.transform.GetChild(0).gameObject;
    }

    public PlayerStateType GetState()
    {
        return currentState;
    }

    private void GetAllPositions()
    {
        if (playerObject == null)
        {
            playerObject = GameObject.FindWithTag("Player");
        }
        
        playerPosition = playerObject.transform.position;
        objectTypeToPositions = new Dictionary<string, List<ObjectInformation>>();
        
        List<ObjectInformation> orbInformations = new List<ObjectInformation>();
        List<GameObject> orbObjects = GameObject.FindGameObjectsWithTag("energyOrbCharged").ToList();
        orbObjects.AddRange(GameObject.FindGameObjectsWithTag("energyOrbUncharged").ToList());
        if (orbObjects.Count > 0)
        {
            foreach (GameObject orbObject in orbObjects)
            {
                orbInformations.Add(new ObjectInformation("Orb", orbObject.transform.position, orbObject));
            }
        }
        objectTypeToPositions.Add("Orb", orbInformations);

        List<ObjectInformation> podiumInformations = new List<ObjectInformation>();
        List<GameObject> podiumObjects = GameObject.FindGameObjectsWithTag("podiumUncharged").ToList();
        podiumObjects.AddRange(GameObject.FindGameObjectsWithTag("podiumCharged").ToList());
        if (podiumObjects.Count > 0)
        {
            foreach (GameObject podiumObject in podiumObjects)
            {
                podiumInformations.Add(new ObjectInformation("Podium", podiumObject.transform.position, podiumObject));
            }
        }
        objectTypeToPositions.Add("Podium", podiumInformations);
        
        List<ObjectInformation> wheelInformations = new List<ObjectInformation>();
        List<GameObject> wheelObjects = GameObject.FindGameObjectsWithTag("Wheel").ToList();
        if (wheelObjects.Count > 0)
        {
            foreach (GameObject wheelObject in wheelObjects)
            {
                wheelInformations.Add(new ObjectInformation("Wheel", wheelObject.transform.position, wheelObject));
            }
        }
        objectTypeToPositions.Add("Wheel", wheelInformations);
        
        // do the same for all the other objects
    }
}