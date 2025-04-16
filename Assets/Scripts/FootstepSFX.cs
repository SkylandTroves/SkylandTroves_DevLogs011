using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSFX : MonoBehaviour
{
    // Start is called before the first frame update
    /*
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;

    public void PlayFootstep()
    {
        int index = UnityEngine.Random.Range(0, footstepClips.Length);
        footstepSource.PlayOneShot(footstepClips[index]);
    }*/
    public AudioSource audioSource; // Drag the AudioSource in here via Inspector
    public AudioClip footstepClip;  // Drag the footstep sound here

    // This method name must match the name you enter in the Animation Event
    public void PlayFootstep()
    {
        if (audioSource != null && footstepClip != null)
        {
            audioSource.PlayOneShot(footstepClip);
        }
    }
}
