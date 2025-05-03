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
    [SerializeField] public float crossFadeTime = 2.0f;
    [SerializeField] private SceneController sceneController;
    
    private void Awake()
    {
        if (crossFade == null)
        {
            Debug.LogError("CrossFade animator is not assigned in Game controller!");
        }
    }
    
    void Start()
    {
        Debugger.Enable();
        
        if (player != null)
        {
            StartCoroutine(PauseBeforeLoadPlayer());
        }
        
    }
    
    public void LoadNextLevel()
    {
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (sceneController != null)
        {
            sceneController.GoToNextLevel(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            StartCoroutine(PauseBeforeLoadLevel(nextLevelIndex));
        }
    }

    IEnumerator PauseBeforeLoadLevel(int levelIndex)
    {
        crossFade.SetTrigger("Start");
        
        yield return new WaitForSeconds(crossFadeTime);

        if (sceneController != null)
        {
            sceneController.GoToNewScene(levelIndex);
        }
        else
        {
            SceneManager.LoadScene(levelIndex);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    IEnumerator PauseBeforeLoadPlayer()
    {
        yield return new WaitForSeconds(0.5f);
        player.SetActive(true);
    }
}