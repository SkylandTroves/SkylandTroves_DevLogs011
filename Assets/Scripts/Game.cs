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
        // Ensure crossFade is initialized
        if (crossFade == null)
        {
            Debug.LogError("CrossFade animator is not assigned in Game controller!");
        }
    }
    
    void Start()
    {
        Debugger.Enable();
        
        // Only activate player if it exists (it won't in menu/end scenes)
        if (player != null)
        {
            StartCoroutine(PauseBeforeLoadPlayer());
        }
        
        // Don't initialize with a fade-in - this is now handled by SceneController
    }
    
    public void LoadNextLevel()
    {
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        // Play transition SFX using the SceneController if available
        if (sceneController != null)
        {
            sceneController.GoToNextLevel(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            // Fallback to the original implementation
            StartCoroutine(PauseBeforeLoadLevel(nextLevelIndex));
        }
    }

    IEnumerator PauseBeforeLoadLevel(int levelIndex)
    {
        // Play fade-to-black animation
        crossFade.SetTrigger("Start");
        
        // Wait for animation to finish
        yield return new WaitForSeconds(crossFadeTime);

        // Load scene via scene controller
        if (sceneController != null)
        {
            sceneController.GoToNewScene(levelIndex);
        }
        else
        {
            // Fallback if scene controller not found
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