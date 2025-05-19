using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
   [SerializeField] private List<MovingPlatform> movingPlatforms;
   //[SerializeField] private PickUpController pickUps; // use if not using a list 
   public bool IsBoatWheel()
   {
       if (movingPlatforms == null || movingPlatforms.Count == 0)
           return false;
           
       foreach (MovingPlatform platform in movingPlatforms)
       {
           if (platform != null && platform.gameObject.CompareTag("Boat"))
           {
               return true;
           }
       }
       
       return false;
   }
   public void HandleWheelScroll()
   {
         // Detect mouse scroll input
         float scrollInput = Input.GetAxis("Mouse ScrollWheel");

         if (scrollInput != 0)
         {
            foreach (MovingPlatform movingPlatform in movingPlatforms)
            {
               // Adjust wheel progress based on scroll input with speed restriction
               float currentProgress = movingPlatform.GetWheelProgress();
               float maxChange = 0.02f; // Maximum allowed change per frame
               float clampedScrollInput = Mathf.Clamp(scrollInput, -maxChange, maxChange);
               float newProgress = Mathf.Clamp(currentProgress + clampedScrollInput, 0, 1);

               // Update the wheel progress in the MovingPlatform
               movingPlatform.SetWheelProgress(newProgress);
               TurnWheelObject(newProgress);
            }
      }
   }

   private void TurnWheelObject(float scrollAmount)
   {
      // Get the current rotation
      Vector3 currentRotation = transform.localEulerAngles;

      // Add to the Z-axis based on scroll amount
      float newYRotation = currentRotation.y + scrollAmount * 20f; // Scale scroll amount

      // Apply the new rotation
      transform.localEulerAngles = new Vector3(currentRotation.x, newYRotation, currentRotation.z);
   }

}