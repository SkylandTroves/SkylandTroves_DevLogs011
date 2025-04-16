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
    
    private GameStates gameState;
    
    public enum GameStates
    {
        GamePlaying,
        GameWon,
        GameLost
    };
    
    public static int MaxCollectiblesCount;

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
    
}
