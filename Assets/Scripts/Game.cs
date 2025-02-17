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
    [SerializeField] private SceneController sceneController;  // Add [SerializeField]

    public void LoadNextLevel()
    {
        StartCoroutine(PauseBeforeLoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator PauseBeforeLoadLevel(int levelIndex)
    {
        // play animation
        crossFade.SetTrigger("Start");
        
        // wait for animation to finish
        yield return new WaitForSeconds(crossFadeTime);

        // load scene
        sceneController.GoToNewScene(levelIndex);

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
