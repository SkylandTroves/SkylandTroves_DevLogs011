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
    }
}