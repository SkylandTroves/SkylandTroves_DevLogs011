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
    
    //click
    [SerializeField] private AudioClip clickSFX;

    //moving platforms - these are now just references to prefabs you'll assign in inspector
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
    
    // Fade duration for wheel sounds (in seconds)
    [SerializeField] private float wheelSoundFadeDuration = 0.5f;
    private Dictionary<string, Coroutine> activeFadeoutCoroutines = new Dictionary<string, Coroutine>();
    
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
}