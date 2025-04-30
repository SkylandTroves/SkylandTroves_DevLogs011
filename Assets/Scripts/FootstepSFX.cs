using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSFX : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    private int index = 0;

    public void PlayFootstep()
    {
        //UnityEngine.Random.Range(0, footstepClips.Length);
        footstepSource.PlayOneShot(footstepClips[index]);
        index++;
        if (index == footstepClips.Length - 1)
        {
            index = 0;
        }
    }
}
