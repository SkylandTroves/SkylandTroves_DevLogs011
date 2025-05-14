using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Game gameController;
    [SerializeField] private float startButtonDelay = 1f;
    [SerializeField] private float musicVolumeFactor = 0.25f;
    public Canvas SkipButtonCanvas;
    private static SceneController instance = null;
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
  //          SkipButtonCanvas.gameObject.SetActive(true);
            StartWindEffectsForCurrentLevel();
        }
        else
        {
//            SkipButtonCanvas.gameObject.SetActive(false);
            StartWindEffectsForCurrentLevel();
        }
        PlayMusicForCurrentLevel(false); // Don't play transition sound when initializing a scene
        
        // Important: Ensure we do an initial fade-in for every scene
        StartCoroutine(InitialFadeIn());
    }
    
    private IEnumerator InitialFadeIn()
    {
        yield return new WaitForSeconds(0.1f); // Small delay to ensure everything is initialized
        
        // Find Game controller in current scene
        Game currentGameController = FindObjectOfType<Game>();
        if (currentGameController != null && currentGameController.crossFade != null)
        {
            // Set initial state to black (fully faded)
            currentGameController.crossFade.SetTrigger("End");
            
            // For video player scenes specifically, add a bit more delay
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "StartEndMenus" || currentScene == "ST_EndScreen")
            {
                yield return new WaitForSeconds(0.5f);
            }
            
            // Trigger the fade out (revealing the scene)
            currentGameController.crossFade.SetTrigger("End");
        }
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        UpdateCurrentLevel();
        StartWindEffectsForCurrentLevel();
        // Play music with transition sound when moving between levels
        // Check if we're coming from a game level (not menu/start screen)
        bool playTransitionSound = oldScene.buildIndex >= 1 && oldScene.buildIndex <= 6 &&
                                    newScene.buildIndex >= 1 && newScene.buildIndex <= 6;
        PlayMusicForCurrentLevel(playTransitionSound);
    }

    private IEnumerator DelayedStartGame()
    {
        Game currentGameController = FindObjectOfType<Game>();
        if (currentGameController != null && currentGameController.crossFade != null)
        {
            currentGameController.crossFade.SetTrigger("Start");
        }
        
        yield return new WaitForSeconds(startButtonDelay);
        
        GoToNewScene("ST_Level_01");
        SkipButtonCanvas.gameObject.SetActive(true);
    }

    private void PlayMusicForCurrentLevel(bool playTransitionSound = true)
    {
        if (SoundController.instance == null) return;
        
        bool withTransition = playTransitionSound && CurrentLevel >= 1 && SceneManager.GetActiveScene().name != "StartEndMenus";
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
                SoundController.instance.PlayEndScreenMusic(volume, withTransition, withFadeIn);
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
        
        if (CurrentLevel >= 0 && CurrentLevel <= 7)
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
            case 0: 
                windClip = SoundController.instance.WindOneSFX;
                volume = 0.6f;
                break;
            case 1:
                windClip = SoundController.instance.WindOneSFX;
                break;
            case 2:
                windClip = SoundController.instance.WindTwoSFX;
                break;
            case 3:
                windClip = SoundController.instance.WindThreeSFX;
                volume = 0.9f;
                break;
            case 4:
                windClip = SoundController.instance.WindFourSFX;
                volume = .95f; 
                break;
            case 5:
                windClip = SoundController.instance.WindFiveSFX;
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

    public void GoToNewScene(string sceneName)
    {
        // Always find the current Game controller in the active scene
        Game currentGameController = FindObjectOfType<Game>();
        
        if (currentGameController != null && currentGameController.crossFade != null)
        {
            StartCoroutine(CrossfadeAndLoadScene(sceneName, currentGameController));
        }
        else
        {
            LoadNewScene(sceneName);
        }
    }
    
    public void GoToNewScene(int buildIndex)
    { 
        // Always find the current Game controller in the active scene
        Game currentGameController = FindObjectOfType<Game>();
        
        if (currentGameController != null && currentGameController.crossFade != null)
        {
            StartCoroutine(CrossfadeAndLoadScene(buildIndex, currentGameController));
        }
        else
        {
            LoadNewScene(buildIndex);
        }
    }

    private IEnumerator CrossfadeAndLoadScene(string sceneName, Game currentGameController)
    {
        // Trigger fade to black
        currentGameController.crossFade.SetTrigger("Start");
        
        // Wait for crossfade animation
        yield return new WaitForSeconds(currentGameController.crossFadeTime);
        
        // Check if we're loading level 3
        if (sceneName == "ST_Level_03")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // Load the new scene
        SceneManager.LoadScene(sceneName);
    }
    
    private IEnumerator CrossfadeAndLoadScene(int buildIndex, Game currentGameController)
    {
        // Trigger fade to black
        currentGameController.crossFade.SetTrigger("Start");
        
        // Wait for crossfade animation
        yield return new WaitForSeconds(currentGameController.crossFadeTime);
        
        // Load the new scene
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

    public void GoToNextLevel(int currentLevel, bool playTransitionSound = true)
    {
        if (SoundController.instance != null)
        {
            // Play transition sound only if explicitly requested
            SoundController.instance.StopLevelMusicWithFade(() => {
                switch (CurrentLevel)
                {
                    case 1:
                        GoToLevelTwo();
                        break;
                    case 2:
                        GoToLevelThree();
                        break;
                    case 3:
                        GoToLevelFour();
                        break;
                    case 4:
                        GoToLevelFive();
                        break;
                    case 5:
                        GoToEndScreen();
                        break;
                }
            }, playTransitionSound); // Use the passed parameter here
        }
        else
        {
            // Fallback if SoundController is not available
            switch (CurrentLevel)
            {
                case 1:
                    GoToLevelTwo();
                    break;
                case 2:
                    GoToLevelThree();
                    break;
                case 3:
                    GoToLevelFour();
                    break;
                case 4:
                    GoToLevelFive();
                    break;
                case 5:
                    GoToEndScreen();
                    break;
            }
        }
    }

    public void GoToPreviousLevel(int currentLevel)
    {
        if (SoundController.instance != null)
        {
            // Play transition sound for all level changes
            SoundController.instance.StopLevelMusicWithFade(() => {
                switch (CurrentLevel)
                {
                    case 1:
                        // do nothing
                        break;
                    case 2:
                        GoToLevelOne();
                        break;
                    case 3:
                        GoToLevelTwo();
                        break;
                    case 4:
                        GoToLevelThree();
                        break;
                    case 5:
                        GoToLevelFour();
                        break;
                    case 6:
                        GoToLevelFive();
                        break;
                }
            }, true); // true = play transition SFX
        }
        else
        {
            // Fallback if SoundController is not available
            switch (CurrentLevel)
            {
                case 1:
                    // do nothing
                    break;
                case 2:
                    GoToLevelOne();
                    break;
                case 3:
                    GoToLevelTwo();
                    break;
                case 4:
                    GoToLevelThree();
                    break;
                case 5:
                    GoToLevelFour();
                    break;
                case 6:
                    GoToLevelFive();
                    break;
            }
        }
    }

    public void OnClickStartGame()
    {
        if (SoundController.instance != null)
        {
            // Play transition sound when explicitly exiting menu to first level
            SoundController.instance.StopLevelMusicWithFade(() => {
                GoToNewScene("ST_Level_01");
  //              SkipButtonCanvas.gameObject.SetActive(true);
            }, true); // true = play transition SFX
        }
        else
        {
            GoToNewScene("ST_Level_01");
//            SkipButtonCanvas.gameObject.SetActive(true);
        }
    }

    public void GoToMainMenu()
    {
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
            SoundController.instance.StopLevelMusic();
        }
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}