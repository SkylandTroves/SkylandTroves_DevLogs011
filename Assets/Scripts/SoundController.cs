using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundController : MonoBehaviour
{
    public static SoundController instance;
    private Dictionary<string, AudioSource> activeLoopingSounds = new Dictionary<string, AudioSource>();
    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioSource loopedSFXObject;

    //balls
    [SerializeField] private AudioClip pickUpBallSFX;
    [SerializeField] private AudioClip dropBallSFX;
    
    //click
    [SerializeField] private AudioClip clickSFX;

    //moving platforms
    [SerializeField] private AudioClip SMALLmovingPlatformSFX;
    [SerializeField] private AudioClip MEDmovingPlatformSFX;
    [SerializeField] private AudioClip BIGmovingPlatformSFX;

    //wind
    [SerializeField] private AudioClip WindONESFX;
    [SerializeField] private AudioClip WindTWOSFX;
    [SerializeField] private AudioClip WindTHREESFX;
    
    // [SerializeField] private AudioClip[] randomSoundsList;

    //getters

    public AudioClip PickUpBallSFX => pickUpBallSFX;
    public AudioClip DropBallSFX => dropBallSFX;
    public AudioClip ClickSFX => clickSFX; 
    
    
    public AudioClip SmallMovingPlatformSFX => SMALLmovingPlatformSFX;
    public AudioClip MediumMovingPlatformSFX => MEDmovingPlatformSFX;
    public AudioClip BigMovingPlatformSFX => BIGmovingPlatformSFX;

    
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
    
    // Future implementation (random sound from bucket)
    /*
    private AudioClip GetRandomSound(AudioClip[] soundArray, AudioClip fallback)
    {
        if (soundArray != null && soundArray.Length > 0)
        {
            int randomIndex = Random.Range(0, soundArray.Length);
            return soundArray[randomIndex];
        }
        return fallback;
    }
    
    public void PlayRandomSound(AudioClip[] soundArray, AudioClip fallback, Transform spawnTransform, float volume = 1f)
    {
        AudioClip soundToPlay = GetRandomSound(soundArray, fallback);
        PlaySFX(soundToPlay, spawnTransform, volume);
    }
    */

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

    //NEW (kevin)
    public AudioSource PlayLoopingSound(AudioClip clip, Transform spawnTransform, string soundID, float volume = 1f)
    {
        // If this soundID is already playing, stop it first
        StopLoopingSound(soundID);
        
        // Create and configure the new looping sound
        AudioSource audioSource = Instantiate(loopedSFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();
        
        // Store it in our active sounds dictionary
        activeLoopingSounds[soundID] = audioSource;
        DontDestroyOnLoad(audioSource.gameObject);
        
        return audioSource;
    }

    //NEW (Kevin)
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
    
    //NEW (Kevin)
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
    
    public AudioSource PlayLoopingSFXInstance(AudioClip clip, Transform spawnTransform, float volume = 1f)
    {    
        AudioSource audioSource = Instantiate(loopedSFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();
        DontDestroyOnLoad(audioSource.gameObject);
        
        return audioSource;
    }
    
    public void StopLoopingSFX()
    {
        if (loopedSFXObject != null)
        {
            loopedSFXObject.Stop();
        }
        else
        {
            Debug.LogError("LoopedSFXObject is not assigned.");
        }
    }
}