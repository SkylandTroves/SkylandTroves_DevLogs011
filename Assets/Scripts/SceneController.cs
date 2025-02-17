using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public Canvas SkipButtonCanvas;
    [SerializeField] private SceneController sceneController;
    private static SceneController instance = null; // Singleton instance
    private static int CurrentLevel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void Start()
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

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        UpdateCurrentLevel();
    }

    private void UpdateCurrentLevel()
    {
        CurrentLevel = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Current Scene Index: " + CurrentLevel);
    }

    // public void LoadNextLevel()
    // {
    //     StartCoroutine(PauseBeforeLoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    // }

    // private IEnumerator PauseBeforeLoadLevel(int levelIndex)
    // {
    //     crossFade.SetTrigger("Start");
    //     yield return new WaitForSeconds(crossFadeTime);
    //     SceneManager.LoadScene(levelIndex);
    // }

    // public void SetStartScene()
    // {
    //     if (SceneManager.GetActiveScene().buildIndex != 0)
    //     {
    //         LoadScene(0);
    //     }
    // }
    //
    // public void GoBackToOldScene(string oldSceneName)
    // {
    //     SceneManager.LoadScene(oldSceneName);
    // }

    public void GoToNewScene(string sceneName)
    {
        LoadNewScene(sceneName);
    }

    public void GoToNewScene(int buildIndex)
    { 
        LoadNewScene(buildIndex);
    }
    
    public void LoadNewScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
    
    public void LoadNewScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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

    public void OnQuitButtonQuitGame()
    {
        QuitGame();
    }

    public static void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}