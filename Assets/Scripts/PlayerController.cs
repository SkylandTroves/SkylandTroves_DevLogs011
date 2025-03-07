/*
Full Name: Aliya Rafei
Student ID:  2391746
rafei@chapman.edu
GAME 340 - 01
Assignment:  Final Project Submission
*/
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem; // new input system 
using System.Collections;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	// character animation states  
	private enum PlayerState
	{
		Idle,
		Walk,
		PickUp,
		Drop,
		PushWallButton,
		PushFloorButton,
		PullLever,
		TurnWheel
	}
	
	//[SerializeField] private PickUpController pickUp;
	[SerializeField] private GameObject clickParticle;
	[SerializeField] private Animator stAnimator;
	
	private List<PickUpController> pickUps;
	private NavMeshAgent navAgent;
	private Action onDestinationReached;
	private GameObject currentHeldOrb;
	private bool isMoving = false;
	
	private const string isWalking = "IsWalking";
	private const string pickedUpOrb = "PickedUpOrb";
	private const string droppedOrb = "DroppedOrb";
	private const string hasPickedUpOrb = "HasPickedUpOrb";

	private List<Action> methodsToCallWhenReachDestination = new List<Action>();

	private void Awake()
	{
		navAgent = GetComponent<NavMeshAgent>();
	}

	void Start()
	{
		navAgent = GetComponent<NavMeshAgent>();
		navAgent.autoTraverseOffMeshLink = false;
		stAnimator.SetBool(hasPickedUpOrb, false);
	}

	void Update()
	{
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			GetPlayerInput();

			CheckArrivedAtDestination();

			SetAnimatorIsMoving();
			UpdateMovementState();

			if (navAgent.isOnOffMeshLink)
			{
				stAnimator.SetBool(isWalking, true);
				StartCoroutine(SmoothTraverse(navAgent));
			}
		}
		else
		{
			StopWalking();
		}
	}
	
	IEnumerator SmoothTraverse(NavMeshAgent agent)
	{
		if (!agent.isOnOffMeshLink) yield break;

		OffMeshLinkData linkData = agent.currentOffMeshLinkData;
		Vector3 startPos = agent.transform.position;
		Vector3 endPos = new Vector3(linkData.endPos.x, agent.transform.position.y, linkData.endPos.z); // Keep consistent Y level

		float duration = Vector3.Distance(startPos, endPos) / agent.speed;
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;

			// Create a smooth arc using a sine wave (optional)
			float height = Mathf.Sin(t * Mathf.PI) * 2f; // Adjust 2f to control arc height

			agent.transform.position = Vector3.Lerp(startPos, endPos, t);

			yield return null;
		}

		agent.transform.position = endPos; // Ensure final position is accurate
		agent.CompleteOffMeshLink();
	}



	private void CheckArrivedAtDestination()
	{
		if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
		{
			if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
			{
				onDestinationReached?.Invoke();
				onDestinationReached = null;
				
				isMoving = false;
				stAnimator.SetBool(isWalking, false);
			}
		}
	}

	public void GetPlayerInput()
	{
		int layerMask = 1 << LayerMask.NameToLayer("Player");
		layerMask = ~layerMask; // invert the mask to exclude the player 
		
		if (Input.GetMouseButtonDown(0)) // LEFT CLICK - - - - - - - - - - - -
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
			{
				navAgent.SetDestination(hit.point);
				onDestinationReached = OnDestinationReached;
				isMoving = true;
			}
			
			// Check if clickParticle is not null before instantiating
			if (clickParticle != null)
			{
				// instantiate instance of particle effect 
				Instantiate(clickParticle, hit.point, Quaternion.identity);
			}
			else
			{
				Debug.LogWarning("Click particle is not assigned!");
			}
			
		}

		if (Input.GetMouseButtonDown(1)) // RIGHT CLICK  - - - - - - - - - - - -
		{
			DropCurrentOrb();
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			QuitGame();
		}
		
		// - - - - following is for debugging only - - - - - 
		Ray ray02 = Camera.main.ScreenPointToRay(Input.mousePosition);
		Vector3 rayOrigin = ray02.origin;
		Vector3 rayDirection = Camera.main.transform.forward;
		float rayLength = 100f;
		Color rayColor = Color.red;
		
		Debug.DrawRay(rayOrigin, rayDirection * rayLength, rayColor);
		// - - - - - - - - - - - - - 
		
	}
	
	public static void QuitGame()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
		Application.Quit();
	}

	public void AddToMethodsToCallWhenReachDestination(Action method)
	{
		methodsToCallWhenReachDestination.Add(method);
	}

	public void OnDestinationReached()
	{
		//print("Reached destination!");
		foreach (Action method in methodsToCallWhenReachDestination)
		{
			method.Invoke();
		}

		methodsToCallWhenReachDestination = new List<Action>();
		isMoving = false;
	}
	
	// GETTER AND SETTER
	
	public void SetPickUpControllers(List<PickUpController> pickUpControllers)
	{
		this.pickUps = pickUpControllers;
	}
	
	public void SetPickUpController(PickUpController pickUp)
	{
		if (pickUps == null)
		{
			pickUps = new List<PickUpController>();
		}

		if (!pickUps.Contains(pickUp))
		{
			pickUps.Add(pickUp);
		}
	}
	
	public void SetHeldOrb(GameObject orb)
	{
		currentHeldOrb = orb;
	}

	public void DropCurrentOrb()
	{
		if (currentHeldOrb != null)
		{
			PickUpController pickUpController = currentHeldOrb.GetComponent<PickUpController>();
			if (pickUpController != null)
			{
				pickUpController.DropObject();
				currentHeldOrb = null;
				HandleDropOrb();
			}
		}
	}
	
	public Animator GetAnimator()
	{
		return stAnimator;
	}
	
	public void TriggerPickUpAnimation()
	{
		stAnimator.SetTrigger(pickedUpOrb);
	}

	public void StopWalking()
	{
		stAnimator.SetBool(isWalking, false);
	}
	
	public void HandlePickUpOrbStart()
	{
		Debug.Log("HandlePickUpOrbStart() called");
		stAnimator.SetTrigger(pickedUpOrb);
		StartCoroutine(SetHasPickedUpOrbAfterAnimation());
	}



	
	private void SetAnimatorIsMoving()
	{
		//stAnimator.SetBool(isWalking, navAgent.velocity.magnitude > 0.0001f);
		isMoving = navAgent.velocity.sqrMagnitude > 0.001f && navAgent.remainingDistance > navAgent.stoppingDistance;
		stAnimator.SetBool(isWalking, isMoving);
	}
	
	/*
	private void UpdateMovementState()
	{
		//Debug.Log($"Movement State Changed: IsMoving = {isMoving}");
		
		// Check if the character is moving
		bool currentlyMoving = navAgent.velocity.sqrMagnitude > 0.01f && navAgent.remainingDistance > navAgent.stoppingDistance;

		// Update the Animator only if the movement state changes
		if (isMoving != currentlyMoving)
		{
			isMoving = currentlyMoving;
			stAnimator.SetBool(isWalking, isMoving);
		}
	}
	*/
	
	private void UpdateMovementState()
	{
		bool currentlyMoving = navAgent.velocity.sqrMagnitude > 0.01f && navAgent.remainingDistance > navAgent.stoppingDistance;

		if (isMoving != currentlyMoving)
		{
			isMoving = currentlyMoving;
        
			// Check if holding an orb to determine animation
			if (currentHeldOrb != null)
			{
				stAnimator.SetBool(isWalking, isMoving);
				if (isMoving)
				{
					stAnimator.Play("WalkAndHold");
				}
				else
				{
					stAnimator.Play("IdleAndHold");
				}
			}
			else
			{
				stAnimator.SetBool(isWalking, isMoving);
			}
		}
	}
	
	private IEnumerator SetHasPickedUpOrbAfterAnimation()
    	{
		    Debug.Log("Waiting for pickup animation..."); 
		    //yield return new WaitForSeconds(2.0f); // Adjust this time based on your animation length
		    // wait for anim to finish instead of doing waitforSeconds 
		    
		    // Get the AnimatorStateInfo for the current layer (usually 0 for the default layer)
		    AnimatorStateInfo stateInfo = stAnimator.GetCurrentAnimatorStateInfo(0);
		    if (stateInfo.IsName(pickedUpOrb))
		    {
			    Debug.Log("This is actually getting the animation");
		    }
			// Wait until the animation state has finished playing
		    while (stAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f && stateInfo.IsName("pickUp0002"))
		    {
			    yield return null; // Wait for the next frame
		    }
		    
		    Debug.Log("Animation finished, setting hasPickedUpOrb = true"); 
		    stAnimator.SetBool("HasPickedUpOrb", true);
    	}
	
	private void HandleDropOrb()
	{
		stAnimator.SetTrigger(droppedOrb);
		stAnimator.SetBool("HasPickedUpOrb", false);
	}
	
	

}
