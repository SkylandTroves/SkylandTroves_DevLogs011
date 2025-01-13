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

public class Game : MonoBehaviour
{
    public Animator crossFade;
    public GameObject player;
    [SerializeField] private float crossFadeTime = 1f;
    
    public void GoToMainMenu()
    {
        //SceneController.GoToNewScene("ST_MainMenu");
    }
    
    public void GoToLevelOne()
    {
        SceneController.GoToNewScene("ST_Level_01");
    }
    
    public void GoToLevelTwo()
    {
        SceneController.GoToNewScene("ST_Level_02");
    }
    
    public void GoToLevelThree()
    {
        SceneController.GoToNewScene("ST_Level_03");
    }

    public void LoadNextLevel()
    {
        StartCoroutine(PauseBeforeLoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }
    
    IEnumerator PauseBeforeLoadLevel(string levelName)
    {
        // play animation
        crossFade.SetTrigger("Start");
        
        // wait for animation to finish
        yield return new WaitForSeconds(crossFadeTime);

        // load scene
        SceneController.GoToNewScene(levelName);

    }
    
    IEnumerator PauseBeforeLoadLevel(int levelIndex)
    {
        // play animation
        crossFade.SetTrigger("Start");
        
        // wait for animation to finish
        yield return new WaitForSeconds(crossFadeTime);

        // load scene
        SceneController.GoToNewScene(levelIndex);

    }

    void Start()
    {
        StartCoroutine(PauseBeforeLoadPlayer());
    }
    
    IEnumerator PauseBeforeLoadPlayer()
    {
        yield return new WaitForSeconds(0.5f);
        player.SetActive(true);
    }

    
    
}
