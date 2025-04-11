using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundController : MonoBehaviour
{
    public static SoundController instance;
    [SerializeField] private AudioSource ClickSFXObject;
    [SerializeField] private AudioSource chargeAreaSFXObject;
    [SerializeField] private AudioClip pickUpBallSFX;
    [SerializeField] private AudioClip dropBallSFX;

    public AudioClip PickUpBallSFX => pickUpBallSFX;
    public AudioClip DropBallSFX => dropBallSFX;

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
        AudioSource audioSource = Instantiate(ClickSFXObject, spawnTransform.position, Quaternion.identity);
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
        AudioSource audioSource = Instantiate(chargeAreaSFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();
        DontDestroyOnLoad(audioSource.gameObject);
        
        return audioSource;
    }
    
    public void StopLoopingSFX()
    {
        if (chargeAreaSFXObject != null)
        {
            chargeAreaSFXObject.Stop();
        }
        else
        {
            Debug.LogError("LoopedSFXObject is not assigned.");
        }
    }
}