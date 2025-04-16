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

public class ObservedSubject : MonoBehaviour
{
    // list of observers in the subject 
    private List<IObserver> observers = new List<IObserver>();
    // could also be a dict??

    public void AddObserver(IObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers()
    {
        observers.ForEach((observer) => {observer.OnNotify();});
    }
    // pass in some sort of data or event call in .OnNotify( here )
    // then need to add params to method signature 
    
    
    // do i need the virtual keyword here?? to override in the subject class
    
    
}
