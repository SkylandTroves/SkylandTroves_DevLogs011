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
using UnityEngine.SceneManagement;


public class LevelSwapping : MonoBehaviour
{
    private static LevelSwapping levelSwapping = null;
    public Canvas SkipButtonCanvas;
    private static int CurrentLevel;
     
    void Start()
    {
        UpdateCurrentLevel();
        if (CurrentLevel >= 1)
        {
            SkipButtonCanvas.gameObject.SetActive(true); 
        }
        else
        {
            SkipButtonCanvas.gameObject.SetActive(false); 
        }
    }
    
    public void Awake()
    {
        // this ensures that we only have one active version of this script 
        if (levelSwapping == null)
        {
            levelSwapping = this;
            DontDestroyOnLoad(gameObject);
            //dont destroy the game object that this script is attached to, this is built in function
        }
        else if (levelSwapping != this)
        {
            Destroy(gameObject);
        }
        SceneManager.activeSceneChanged += OnSceneChanged;

    }
    
    public void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    public void OnQuitButtonQuitGame()
    {
        GameController.QuitGame();
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
    
    public static void GoToNewScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }


    // Call this method to go back to the old scene
    public static void GoBackToOldScene(string oldSceneName)
    {
        SceneManager.LoadScene(oldSceneName);
    }

    public void OnClickStartGame()
    {
        GoToNewScene("ST_Level_01");
        SkipButtonCanvas.gameObject.SetActive(true); 
    }
    
    public void GoToMainMenu()
    {
        GoToNewScene("StartEndMenus");
    }
    
    public void GoToLevelOne()
    {
        GoToNewScene("ST_Level_01");
    }

    public void GoToLevelTwo()
    {
        GoToNewScene("ST_Level_02");
    }
    
    public void GoToLevelThree()
    {
        GoToNewScene("ST_Level_03");
    }
    
    public void GoToLevelFour()
    {
        GoToNewScene("ST_Level_04");
    }

    public void GoToLevelFive()
    {
        GoToNewScene("ST_Level_05");
    }
    
    public void GoToLevelSix()
    {
        GoToNewScene("ST_Level_06");
    }

    public void GoToNextLevel(int currentLevel)
    {
        switch (CurrentLevel)
        {
            case 1:
                if (CurrentLevel == 1)
                {
                    GoToLevelTwo();
                }
                break;
            case 2:
                if (CurrentLevel == 2)
                {
                    GoToLevelThree();
                }
                break;
            case 3:
                if (CurrentLevel == 3)
                {
                    GoToLevelFour();
                }
                break;
            case 4:
                if (CurrentLevel == 4)
                {
                    GoToLevelFive();
                }
                break;
            case 5:
                if (CurrentLevel == 5)
                {
                    GoToLevelSix();
                }
                break;
                    
        }
    }
    
    public void GoToPreviousLevel(int currentLevel)
    {
        switch (CurrentLevel)
        {
            case 1:
                if (CurrentLevel == 1)
                {
                   // do nothing
                }
                break;
            case 2:
                if (CurrentLevel == 2)
                {
                    GoToLevelOne();
                }
                break;
            case 3:
                if (CurrentLevel == 3)
                {
                    GoToLevelTwo();
                }
                break;
            case 4:
                if (CurrentLevel == 4)
                {
                    GoToLevelThree();
                }
                break;
            case 5:
                if (CurrentLevel == 5)
                {
                    GoToLevelFour();
                }
                break;
            case 6:
                if (CurrentLevel == 6)
                {
                    GoToLevelFive();
                }
                break;
        }
    }
    
    private void UpdateCurrentLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        CurrentLevel = currentSceneIndex;
        Debug.Log("Current Scene Index: " + CurrentLevel);
    }
    
    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        UpdateCurrentLevel();  // Update the level after the scene has changed
    }
    
    
}
