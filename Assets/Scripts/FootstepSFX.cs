using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSFX : MonoBehaviour
{
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    private int index = 0;

    public void PlayFootstep()
    {
        footstepSource.PlayOneShot(footstepClips[index]);
        index++;
        if (index == footstepClips.Length - 1)
        {
            index = 0;
        }
    }
}
