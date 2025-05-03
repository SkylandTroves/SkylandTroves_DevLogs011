using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController instance;
    private Dictionary<string, AudioSource> activeLoopingSounds = new Dictionary<string, AudioSource>();
    [SerializeField] private AudioSource soundFXObject;
    
    [SerializeField] private Transform playerTransform;
    
    //balls
    [SerializeField] private AudioClip pickUpBallSFX;
    [SerializeField] private AudioClip dropBallSFX;
    
    [Header("Level Music")]
    [SerializeField] private AudioClip titleScreenMusic;
    [SerializeField] private AudioClip level1Music;
    [SerializeField] private AudioClip level2Music;
    [SerializeField] private AudioClip level3Music;
    [SerializeField] private AudioClip level4Music;
    [SerializeField] private AudioClip level5Music;
    [SerializeField] private AudioClip levelTransitionSFX;
    [SerializeField] private AudioSource transitionSFXSource;
    
    [SerializeField] private float musicFadeDuration = 0.5f;
    private AudioSource currentLevelMusicSource;
    private Coroutine currentMusicFadeCoroutine;
    //click
    [SerializeField] private AudioClip clickSFX;

    //moving platforms
    [SerializeField] private AudioSource SMALLmovingPlatformSFX;
    [SerializeField] private AudioSource MEDmovingPlatformSFX;
    [SerializeField] private AudioSource BIGmovingPlatformSFX;

    //wind
    [SerializeField] private AudioClip WindONESFX;
    [SerializeField] private AudioClip WindTWOSFX;
    [SerializeField] private AudioClip WindTHREESFX;
    [SerializeField] private AudioClip WindFOURSFX;
    [SerializeField] private AudioClip WindFIVESFX;

    //wheel
    [SerializeField] private AudioClip WHEELSFX;
    [SerializeField] private AudioClip isBoatWHEELSFX;
    
    [SerializeField] private float wheelSoundFadeDuration = 0.5f;
    private Dictionary<string, Coroutine> activeFadeoutCoroutines = new Dictionary<string, Coroutine>();
    
    private bool isInTransition = false;
    
    // Getters
    public AudioClip PickUpBallSFX => pickUpBallSFX;
    public AudioClip DropBallSFX => dropBallSFX;
    public AudioClip ClickSFX => clickSFX; 
    
    public AudioSource SmallMovingPlatformSFX => SMALLmovingPlatformSFX;
    public AudioSource MediumMovingPlatformSFX => MEDmovingPlatformSFX;
    public AudioSource BigMovingPlatformSFX => BIGmovingPlatformSFX;
    
    public AudioClip WindOneSFX => WindONESFX;
    public AudioClip WindTwoSFX => WindTWOSFX;
    public AudioClip WindThreeSFX => WindTHREESFX;
    public AudioClip WindFourSFX => WindFOURSFX;
    public AudioClip WindFiveSFX => WindFIVESFX;

    public AudioClip WheelSFX => WHEELSFX;
    public AudioClip BoatWheelSFX => isBoatWHEELSFX;

    public AudioClip TitleScreenMusic => titleScreenMusic;
    public AudioClip Level1Music => level1Music;
    public AudioClip Level2Music => level2Music;
    public AudioClip Level3Music => level3Music;
    public AudioClip Level4Music => level4Music;
    public AudioClip Level5Music => level5Music;
    public AudioClip LevelTransitionSFX => levelTransitionSFX;
    
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeTransitionSource();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip audioClip, Transform spawnTransform, float volume = 1f)
    {
        // spawn in the gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        // assign the clip
        audioSource.clip = audioClip;
        // assign volume
        audioSource.volume = volume;
        // play the sound
        audioSource.Play();
        // get length of sound FX clip
        float clipLength = audioSource.clip.length;
        // destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayPlatformSound(string platformType, Transform platformTransform)
    {
        AudioSource prefab = null;
        
        switch (platformType)
        {
            case "SmallPlatform":
                prefab = SMALLmovingPlatformSFX;
                break;
            case "MediumPlatform":
                prefab = MEDmovingPlatformSFX;
                break;
            case "BigPlatform":
                prefab = BIGmovingPlatformSFX;
                break;
            default:
                Debug.LogWarning("Unknown platform type: " + platformType);
                return;
        }
        
        AudioSource source = Instantiate(prefab, platformTransform.position, Quaternion.identity);
        source.transform.SetParent(platformTransform);       
        source.Play();
        
        string soundID = platformType + "_" + platformTransform.GetInstanceID();
        activeLoopingSounds[soundID] = source;
    }
    
    public void StopPlatformSound(string platformType, int platformID)
    {
        string soundID = platformType + "_" + platformID;
        if (activeLoopingSounds.TryGetValue(soundID, out AudioSource source))
        {
            if (source != null)
            {
                source.Stop();
                Destroy(source.gameObject);
            }
            activeLoopingSounds.Remove(soundID);
        }
    }
    
    public void StopAllLoopingSounds()
    {
        foreach (var coroutine in activeFadeoutCoroutines.Values)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        activeFadeoutCoroutines.Clear();
        
        foreach (var audioSource in activeLoopingSounds.Values)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                Destroy(audioSource.gameObject);
            }
        }
        activeLoopingSounds.Clear();
    }

    public AudioSource PlayLoopingSound(AudioClip clip, Transform sourceTransform, string soundID, float volume = 1f)
    {
        StopLoopingSound(soundID);
        
        AudioSource audioSource = Instantiate(soundFXObject, sourceTransform.position, Quaternion.identity);
        
        if (sourceTransform != null)
        {
            audioSource.transform.SetParent(sourceTransform);
        }
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = true;
        
        audioSource.Play();
        
        activeLoopingSounds[soundID] = audioSource;
        
        return audioSource;
    }

    public void StopLoopingSound(string soundID)
    {
        if (activeLoopingSounds.TryGetValue(soundID, out AudioSource audioSource))
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                Destroy(audioSource.gameObject);
            }
            activeLoopingSounds.Remove(soundID);
        }
        
        if (activeFadeoutCoroutines.TryGetValue(soundID, out Coroutine coroutine))
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            activeFadeoutCoroutines.Remove(soundID);
        }
    }

    public void PlayWheelTurningSound(Transform wheelTransform, float volume = 1f)
    {
        string soundID = "wheel_" + wheelTransform.GetInstanceID();
        
        if (activeFadeoutCoroutines.TryGetValue(soundID, out Coroutine fadeCoroutine))
        {
            StopCoroutine(fadeCoroutine);
            activeFadeoutCoroutines.Remove(soundID);
        }
        
        if (activeLoopingSounds.TryGetValue(soundID, out AudioSource existingSource) && existingSource != null)
        {
            existingSource.volume = volume;
            return;
        }

        Wheel wheelComponent = wheelTransform.GetComponent<Wheel>();
        AudioClip clipToPlay = WHEELSFX;
        
        if (wheelComponent != null && wheelComponent.IsBoatWheel())
        {
            clipToPlay = isBoatWHEELSFX;
        }
        
        AudioSource audioSource = PlayLoopingSound(clipToPlay, wheelTransform, soundID, volume);
        
        if (audioSource != null && audioSource.clip != null) 
        {
            float randomStartTime = UnityEngine.Random.Range(0f, audioSource.clip.length * 0.8f);
            audioSource.time = randomStartTime;
        }
    }

    public void StopWheelTurningSound(Transform wheelTransform)
    {
        string soundID = "wheel_" + wheelTransform.GetInstanceID();
        
        if (activeLoopingSounds.TryGetValue(soundID, out AudioSource audioSource) && audioSource != null)
        {
            if (activeFadeoutCoroutines.TryGetValue(soundID, out Coroutine coroutine))
            {
                StopCoroutine(coroutine);
            }
            
            Coroutine fadeoutCoroutine = StartCoroutine(FadeOutWheelSound(soundID, audioSource));
            activeFadeoutCoroutines[soundID] = fadeoutCoroutine;
        }
    }
    
    private IEnumerator FadeOutWheelSound(string soundID, AudioSource audioSource)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;
        
        while (elapsedTime < wheelSoundFadeDuration && audioSource != null)
        {
            elapsedTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, 0f, elapsedTime / wheelSoundFadeDuration);
            audioSource.volume = newVolume;
            yield return null;
        }
        
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource.gameObject);
        }
        
        activeLoopingSounds.Remove(soundID);
        activeFadeoutCoroutines.Remove(soundID);
    }

    public void PlayLevelMusic(AudioClip musicClip, float volume = 1f, bool withTransition = false, bool withFadeIn = true)
    {
        if (currentLevelMusicSource != null)
        {
            if (withTransition)
            {
                StopLevelMusicWithFade(() => {
                    if (withFadeIn)
                        PlayLevelMusicWithFadeIn(musicClip, volume, musicFadeDuration);
                    else
                        StartNewLevelMusic(musicClip, volume);
                }, withTransition);
                return;
            }
            else
            {
                StopLevelMusic();
            }
        }
        
        if (withFadeIn)
            PlayLevelMusicWithFadeIn(musicClip, volume, musicFadeDuration);
        else
            StartNewLevelMusic(musicClip, volume);
    }
    private void StartNewLevelMusic(AudioClip musicClip, float volume)
    {
        if (musicClip == null) return;
        
        currentLevelMusicSource = Instantiate(soundFXObject, transform.position, Quaternion.identity);
        currentLevelMusicSource.transform.SetParent(transform);
        
        currentLevelMusicSource.clip = musicClip;
        currentLevelMusicSource.volume = volume;
        currentLevelMusicSource.loop = true;
        currentLevelMusicSource.Play();
    }
    
    public void StopLevelMusic()
    {
        if (currentMusicFadeCoroutine != null)
        {
            StopCoroutine(currentMusicFadeCoroutine);
            currentMusicFadeCoroutine = null;
        }
        
        if (currentLevelMusicSource != null)
        {
            currentLevelMusicSource.Stop();
            Destroy(currentLevelMusicSource.gameObject);
            currentLevelMusicSource = null;
        }
    }
    
    public void StopLevelMusicWithFade(System.Action onComplete = null, bool playTransitionSFX = false)
    {
        if (playTransitionSFX && levelTransitionSFX != null)
        {
            if (transitionSFXSource == null)
            {
                InitializeTransitionSource();
            }
            
            transitionSFXSource.Stop();
            
            transitionSFXSource.clip = levelTransitionSFX;
            transitionSFXSource.volume = 0.25f;
            transitionSFXSource.Play();
            
            StartCoroutine(CleanupTransitionSource(levelTransitionSFX.length + 0.5f));
        }
        
        if (currentLevelMusicSource != null)
        {
            if (currentMusicFadeCoroutine != null)
            {
                StopCoroutine(currentMusicFadeCoroutine);
            }
            
            currentMusicFadeCoroutine = StartCoroutine(FadeOutLevelMusic(onComplete));
        }
        else if (onComplete != null)
        {
            onComplete();
        }
    }
    
    private IEnumerator CleanupTransitionSource(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (transitionSFXSource != null && !transitionSFXSource.isPlaying)
        {
            transitionSFXSource.Stop();
            transitionSFXSource.clip = null;
        }
    }

    private IEnumerator FadeOutLevelMusic(System.Action onComplete)
    {
        if (currentLevelMusicSource == null) yield break;
        
        float startVolume = currentLevelMusicSource.volume;
        float elapsedTime = 0f;
        
        while (elapsedTime < musicFadeDuration && currentLevelMusicSource != null)
        {
            elapsedTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, 0f, elapsedTime / musicFadeDuration);
            currentLevelMusicSource.volume = newVolume;
            yield return null;
        }
        
        if (currentLevelMusicSource != null)
        {
            currentLevelMusicSource.Stop();
            Destroy(currentLevelMusicSource.gameObject);
            currentLevelMusicSource = null;
        }
        
        currentMusicFadeCoroutine = null;
        
        if (onComplete != null)
        {
            onComplete();
        }
    }
    
    public void PlayTitleScreenMusic(float volume = 1f, bool withFadeIn = true)
    {
        PlayLevelMusic(titleScreenMusic, volume, false, withFadeIn);
    }

    public void PlayLevel1Music(float volume = 1f, bool withTransition = false, bool withFadeIn = true)
    {
        PlayLevelMusic(level1Music, volume, withTransition, withFadeIn);
    }

    public void PlayLevel2Music(float volume = 1f, bool withTransition = false, bool withFadeIn = true)
    {
        PlayLevelMusic(level2Music, volume, withTransition, withFadeIn);
    }

    public void PlayLevel3Music(float volume = 1f, bool withTransition = false, bool withFadeIn = true)
    {
        PlayLevelMusic(level3Music, volume, withTransition, withFadeIn);
    }

    public void PlayLevel4Music(float volume = 1f, bool withTransition = false, bool withFadeIn = true)
    {
        PlayLevelMusic(level4Music, volume, withTransition, withFadeIn);
    }

    public void PlayLevel5Music(float volume = 1f, bool withTransition = false, bool withFadeIn = true)
    {
        PlayLevelMusic(level5Music, volume, withTransition, withFadeIn);
    }
    
    public void PlayEndScreenMusic(float volume = 1f, bool withTransition = false, bool withFadeIn = true)
    {
        PlayLevelMusic(titleScreenMusic, volume, withTransition, withFadeIn);
    }

    public void PlayLevelMusicWithFadeIn(AudioClip musicClip, float targetVolume = 1f, float fadeDuration = 0.5f)
    {
        StopLevelMusic();
        
        currentLevelMusicSource = Instantiate(soundFXObject, transform.position, Quaternion.identity);
        currentLevelMusicSource.transform.SetParent(transform);
        
        currentLevelMusicSource.clip = musicClip;
        currentLevelMusicSource.volume = 0f; 
        currentLevelMusicSource.loop = true;
        currentLevelMusicSource.Play();
        
        StartCoroutine(FadeInLevelMusic(targetVolume, fadeDuration));
    }

    private IEnumerator FadeInLevelMusic(float targetVolume, float fadeDuration)
    {
        if (currentLevelMusicSource == null) yield break;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration && currentLevelMusicSource != null)
        {
            elapsedTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(0f, targetVolume, elapsedTime / fadeDuration);
            currentLevelMusicSource.volume = newVolume;
            yield return null;
        }
        
        if (currentLevelMusicSource != null)
        {
            currentLevelMusicSource.volume = targetVolume;
        }
    }

    private void InitializeTransitionSource()
    {
        if (transitionSFXSource == null)
        {
            GameObject transitionSourceObj = new GameObject("TransitionSFXSource");
            transitionSFXSource = transitionSourceObj.AddComponent<AudioSource>();
            transitionSFXSource.playOnAwake = false;
            transitionSFXSource.loop = false;
            transitionSFXSource.spatialBlend = 0f; 
            transitionSFXSource.priority = 0;
            DontDestroyOnLoad(transitionSourceObj);
            transitionSourceObj.transform.SetParent(transform);
        }
    }
}