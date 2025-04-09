using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController instance;
    
    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioSource loopedSFXObject;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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

    public AudioSource PlayLoopingSFXInstance(AudioClip clip, Transform spawnTransform, float volume = 1f)
    {
        AudioSource audioSource = Instantiate(loopedSFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();
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
    