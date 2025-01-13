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
//  * * * *  UNUSED SCRIPT * * * * 

public class ignoreRaycast : MonoBehaviour
{
    private void Update()
    {
        
        int layerMask = 1 << LayerMask.NameToLayer("Player");
        layerMask = ~layerMask; // invert the mask to exclude teh player 
        
    }
    
    
}
