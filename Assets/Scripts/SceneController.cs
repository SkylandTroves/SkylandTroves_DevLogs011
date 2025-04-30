using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Game gameController;
    [SerializeField] private float startButtonDelay = 1f;
    [SerializeField] private float musicVolumeFactor = 0.25f;
    //public Canvas SkipButtonCanvas;
    private static SceneController instance = null;
    private static int CurrentLevel;

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GoToNextLevel(CurrentLevel);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GoToPreviousLevel(CurrentLevel);
        }
    }
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
            //SkipButtonCanvas.gameObject.SetActive(false);
            StartWindEffectsForCurrentLevel();
        }
        else
        {
            //SkipButtonCanvas.gameObject.SetActive(false);
        }
        PlayMusicForCurrentLevel();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        UpdateCurrentLevel();
        StartWindEffectsForCurrentLevel();
        PlayMusicForCurrentLevel();
    }

    private IEnumerator DelayedStartGame()
    {
        if (gameController != null && gameController.crossFade != null)
        {
            gameController.crossFade.SetTrigger("Start");
        }
        
        yield return new WaitForSeconds(startButtonDelay);
        
        GoToNewScene("ST_Level_01");
        //SkipButtonCanvas.gameObject.SetActive(true);
    }

    private void PlayMusicForCurrentLevel()
    {
        if (SoundController.instance == null) return;
        
        bool withTransition = CurrentLevel >= 1 && SceneManager.GetActiveScene().name != "StartEndMenus";
        bool withFadeIn = true; 
        float volume = 1.0f * musicVolumeFactor;
        
        switch (CurrentLevel)
        {
            case 0: 
                SoundController.instance.PlayTitleScreenMusic(volume, withFadeIn);
                break;
            case 1:
                SoundController.instance.PlayLevel1Music(volume, withTransition, withFadeIn);
                break;
            case 2:
                SoundController.instance.PlayLevel2Music(volume, withTransition, withFadeIn);
                break;
            case 3:
                SoundController.instance.PlayLevel3Music(volume, withTransition, withFadeIn);
                break;
            case 4:
                SoundController.instance.PlayLevel4Music(volume, withTransition, withFadeIn);
                break;
            case 5:
                SoundController.instance.PlayLevel5Music(volume, withTransition, withFadeIn);
                break;
            case 6:
                SoundController.instance.PlayLevel5Music(volume, withTransition, withFadeIn);
                break;
            default:
               
                SoundController.instance.StopLevelMusic();
                break;
        }
    }

    private void StartWindEffectsForCurrentLevel()
    {
        if (SoundController.instance != null)
        {
            SoundController.instance.StopLoopingSound("LevelWind");
        }
        
        if (CurrentLevel >= 1 && CurrentLevel <= 6)
        {
            PlayWindSoundForLevel(CurrentLevel);
        }
    }

    private void PlayWindSoundForLevel(int level)
    {
        if (SoundController.instance == null) return;

        AudioClip windClip;
        float volume = 0.8f; 

        switch (level)
        {
            case 1:
                windClip = SoundController.instance.WindOneSFX;
                break;
            case 2:
                windClip = SoundController.instance.WindOneSFX;
                break;
            case 3:
                windClip = SoundController.instance.WindTwoSFX;
                volume = 0.9f;
                break;
            case 4:
                windClip = SoundController.instance.WindThreeSFX;
                volume = 0.95f; 
                break;
            case 5:
                windClip = SoundController.instance.WindFourSFX;
                volume = 0.95f; 
                break;
            case 6:
                windClip = SoundController.instance.WindFiveSFX;
                volume = 0.95f;
                break;
            default:
                windClip = SoundController.instance.WindOneSFX;
                break;
        }


        SoundController.instance.PlayLoopingSound(windClip, transform, "LevelWind", volume);
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
        if (gameController != null && gameController.crossFade != null)
        {
            StartCoroutine(CrossfadeAndLoadScene(sceneName));
        }
        else
        {
            LoadNewScene(sceneName);
        }
    }
    
    public void GoToNewScene(int buildIndex)
    { 
        if (gameController != null && gameController.crossFade != null)
        {
            StartCoroutine(CrossfadeAndLoadScene(buildIndex));
        }
        else
        {
            LoadNewScene(buildIndex);
        }
    }

    private IEnumerator CrossfadeAndLoadScene(string sceneName)
    {
        gameController.crossFade.SetTrigger("Start");
        yield return new WaitForSeconds(gameController.crossFadeTime);
        SceneManager.LoadScene(sceneName);
    }
    
    private IEnumerator CrossfadeAndLoadScene(int buildIndex)
    {
        gameController.crossFade.SetTrigger("Start");
        yield return new WaitForSeconds(gameController.crossFadeTime);
        SceneManager.LoadScene(buildIndex);
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
                    GoToEndScreen();
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
        if (SoundController.instance != null)
        {
            SoundController.instance.StopLevelMusicWithFade(() => {
                GoToNewScene("ST_Level_01");
                //SkipButtonCanvas.gameObject.SetActive(true);
            });
        }
        else
        {
            GoToNewScene("ST_Level_01");
            //SkipButtonCanvas.gameObject.SetActive(true);
        }
    }

    public void GoToMainMenu()
    {
        print("***clicking button");
        if (SoundController.instance != null)
        {
            SoundController.instance.StopLoopingSound("LevelWind");
        }
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

    public void GoToEndScreen()
    {
        GoToNewScene("ST_EndScreen");
    }

    public void OnQuitButtonQuitGame()
    {
        QuitGame();
    }

    public static void QuitGame()
    {
        if (SoundController.instance != null)
        {
            SoundController.instance.StopAllLoopingSounds();
            SoundController.instance.StopLevelMusic(); // Add this line
        }
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}