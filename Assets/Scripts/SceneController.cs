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
using UnityEngine.SceneManagement;

public static class SceneController
{
    // This method checks if the current scene is not scene 1, then it loads scene 1
    public static void SetStartScene()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0) // Assuming scene 1 is at index 0
        {
            LoadScene(0); // Load scene 1 (index 0)
        }
    }


    // Function to load a scene by build index
    public static void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
  
    public static void GoToNewScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public static void GoToNewScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }


    // Call this method to go back to the old scene
    public static void GoBackToOldScene(string oldSceneName)
    {
        SceneManager.LoadScene(oldSceneName);
    }

    public static void GoToNextLevel(int buildIndex)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    
  
}

