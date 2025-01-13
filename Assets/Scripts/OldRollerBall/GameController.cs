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
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public enum GameStates
    {
        GamePlaying,
        GameWon,
        GameLost
    };
    public static int MaxCollectiblesCount;
    
    private GameStates gameState;

    private void Start()
    {
        gameState = GameStates.GamePlaying;
    }
    
    void Update()
    {
        CheckForUserInput();
    }
    
    private void OnGameWon()
    {
        gameState = GameStates.GameWon;
    }
    
    
    public static void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
    
    public void CheckForUserInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }


    public void OnGameLost()
    {
        gameState = GameStates.GameLost;
        
    }

    public void StateUpdate(GameStates newState)
    {
        //Exit condition- if the game is not in play, we cannot advance to win or lose
        if (gameState != GameStates.GamePlaying)
        {
            return;
        }
        
        switch (newState)
        {
            case GameStates.GamePlaying:
                break;
            case GameStates.GameWon:
                gameState = GameStates.GameWon;
                OnGameWon();
                break;
            case GameStates.GameLost:
                gameState = GameStates.GameLost;
                OnGameLost();
                break;
        }
    }
    
    
}
