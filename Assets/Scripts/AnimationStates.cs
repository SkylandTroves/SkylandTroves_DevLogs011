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
    private PlayerState playerState;

    private void Start()
    {
        playerState = GetComponent<PlayerState>();
        animSpeed = stAnimator.speed;
    }

    public void UpdateState(PlayerStateType stateType)
    {
        stAnimator.SetInteger(stateTagAnimator, (int)stateType);
    }

    public void ChangeToPickUp(bool isOrbOnPodium)
    {
        StartCoroutine(PlayPickupAnimation(isOrbOnPodium));
    }
    
    IEnumerator PlayPickupAnimation(bool isOrbOnPodium)
    {
        if (isOrbOnPodium)
        {
            UpdateState(PlayerStateType.NextToPodiumWithoutOrb);
        }
        else
        {
            UpdateState(PlayerStateType.PickupOrb);
        }
        yield return new WaitForSeconds(1f); // Adjust this to match the pick-up animation duration

        if (playerState.getCurrentState() == PlayerStateType.NextToWheelWithoutOrb)
        {
            playerState.setCurrentState(PlayerStateType.NextToWheelWithOrb);
        }
        else
        {
            UpdateState(PlayerStateType.IdleWithOrb);
        }
    }

    public void pauseAnimation()
    {
        stAnimator.speed = 0f;
    }

    public void resumeAnimation()
    {
        stAnimator.speed = animSpeed;
    }
    
    public bool isPaused()
    {
        return (animSpeed != 0);
    }
    
}
