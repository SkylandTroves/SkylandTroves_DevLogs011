using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public bool isPaused = false;
    public Canvas mainCanvas;
    private GraphicRaycaster graphicRaycaster;
    public Collider[] allColliders;
    void Start()
    {
        pauseMenuUI.SetActive(false);
        graphicRaycaster = mainCanvas.GetComponent<GraphicRaycaster>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (Cursor.lockState != CursorLockMode.Locked))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        graphicRaycaster.enabled = false;
        allColliders = FindObjectsOfType<Collider>();
        foreach (Collider col in allColliders)
        {
            col.enabled = false;
        }
        pauseMenuUI.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        graphicRaycaster.enabled = true;
        foreach (Collider col in allColliders)
        {
            col.enabled = true;
        }
        pauseMenuUI.SetActive(false);
    }

    public bool GetIsPaused()
    {
        return isPaused;
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
                Application.Quit();
    }
}