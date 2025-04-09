using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStates : MonoBehaviour
{
    [SerializeField] private Animator stAnimator;
    private int currentState;
    private const string stateTagAnimator = "currentState";

    public void UpdateState(PlayerStateType stateType)
    {
        stAnimator.SetInteger(stateTagAnimator, (int)stateType);
        //Debug.Log(" *** UpdateState: INT " + (int)stateType);
    }

    public void ChangeToPickUp()
    {
        //stAnimator.SetInteger(stateTagAnimator, (int)PlayerStateType.PickupOrb);
        print(" *** ChangeToPickUp: pick up anim");
        StartCoroutine(PlayPickupAnimation());
    }
    
    IEnumerator PlayPickupAnimation()
    {
        print(" *** PlayPickupAnimation: PICKING UP");
        UpdateState(PlayerStateType.PickupOrb);

        yield return new WaitForSeconds(1f); // Adjust this to match the pick-up animation duration
        
        UpdateState(PlayerStateType.IdleWithOrb);
        
    }
    
}
