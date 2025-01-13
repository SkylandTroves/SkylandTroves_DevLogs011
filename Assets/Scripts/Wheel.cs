/*
Full Name: Aliya Rafei
Student ID:  2391746
rafei@chapman.edu
GAME 340 - 01
Assignment:  Final Project Submission
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
   [SerializeField] private List<MovingPlatform> movingPlatforms;
   [SerializeField] private PlayerController player;
   //[SerializeField] private PickUpController pickUps; // use if not using a list 
   
   private bool turnRequested = false;
   private bool isTurningObject = false;
   private float distancePlayerAndWheel;
   private float distanceToWheel = 2f; // Maximum distance to allow wheel hold
   
   
   private void Update()
   {
      StoreDistanceToWheel();
      HandleWheelScroll(distancePlayerAndWheel);
   }
   
   private void OnMouseDown()
   {
      Debug.Log("You are now clicking on the wheel ");
      turnRequested = true;
      //player.AddToMethodsToCallWhenReachDestination(TurnObject);
      
   }
   
   private void HandleWheelScroll(float distancePlayerAndWheel)
   {
      if (turnRequested && distancePlayerAndWheel <= distanceToWheel)
      {
         //Debug.Log("You can turn the wheel");
         isTurningObject = true; // The player is allowed to turn the wheel
         
         // Detect mouse scroll input
         float scrollInput = Input.GetAxis("Mouse ScrollWheel");

         if (scrollInput != 0)
         {
            foreach (MovingPlatform movingPlatform in movingPlatforms)
            {
               // Adjust wheel progress based on scroll input
               float currentProgress = movingPlatform.GetWheelProgress();
               float newProgress = Mathf.Clamp(currentProgress + scrollInput, 0, 1);
               
               // Update the wheel progress in the MovingPlatform
               movingPlatform.SetWheelProgress(newProgress);
               TurnWheelObject(newProgress);
               
               Debug.Log("wheel is turning "+ newProgress );
            }
            
            
         }
      }
   }

   private void TurnWheelObject(float scrollAmount)
   {
      // Get the current rotation
      Vector3 currentRotation = transform.localEulerAngles;

      // Add to the Z-axis based on scroll amount
      float newZRotation = currentRotation.z + scrollAmount * 20f; // Scale scroll amount

      // Apply the new rotation
      transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, newZRotation);
   }

   public void StoreDistanceToWheel()
   {
      distancePlayerAndWheel = Vector3.Distance(player.transform.position, transform.position);
   }
   
   
   

}
