using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStates : MonoBehaviour
{
    [SerializeField] private Animator stAnimator;
    private int currentState;
    private const string stateTagAnimator = "currentState";
    private float animSpeed;

    private void Start()
    {
        animSpeed = stAnimator.speed;
    }

    public void UpdateState(PlayerStateType stateType)
    {
        stAnimator.SetInteger(stateTagAnimator, (int)stateType);
        //Debug.Log(" *** UpdateState: INT " + (int)stateType);
    }

    public void ChangeToPickUp(bool isOrbOnPodium)
    {
        //stAnimator.SetInteger(stateTagAnimator, (int)PlayerStateType.PickupOrb);
        print(" *** ChangeToPickUp: pick up anim");
        StartCoroutine(PlayPickupAnimation(isOrbOnPodium));
    }
    
    IEnumerator PlayPickupAnimation(bool isOrbOnPodium)
    {
        if (isOrbOnPodium)
        {
            print(" *** PlayPickupAnimation: PICKING UP FROM PODIUM");
            UpdateState(PlayerStateType.NextToPodiumWithoutOrb);
        }
        else
        {
            print(" *** PlayPickupAnimation: PICKING UP FROM GROUND");
            UpdateState(PlayerStateType.PickupOrb);
        }
        yield return new WaitForSeconds(1f); // Adjust this to match the pick-up animation duration
        
        UpdateState(PlayerStateType.IdleWithOrb);
        
    }

    public void pauseAnimation()
    {
        stAnimator.speed = 0f;
    }

    public void resumeAnimation()
    {
        stAnimator.speed = animSpeed;
    }

    public void setPauseAnimation(bool isPaused)
    {
        if (isPaused)
        {
            stAnimator.speed = 0f;
        }
        else
        {
            stAnimator.speed = animSpeed;
        }
    }
    public bool isPaused()
    {
        return (animSpeed != 0);
    }
    
}
