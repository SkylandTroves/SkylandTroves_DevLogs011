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
using Object = System.Object;

public class PlayerController : MonoBehaviour
{

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

	private List<Action> methodsToCallWhenReachDestination = new List<Action>();
	private Wheel currentWheel;

	private PlayerState playerState;
	private void Awake()
	{
		navAgent = GetComponent<NavMeshAgent>();
	}

	void Start()
	{
		playerState = GetComponent<PlayerState>();
		navAgent = GetComponent<NavMeshAgent>();
		navAgent.autoTraverseOffMeshLink = false;

	}

	void Update()
	{
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			GetPlayerInput();
			CheckArrivedAtDestination();
			UpdateMovementState();

			if (navAgent.isOnOffMeshLink)
			{
				playerState.UpdateStateOnStartMoving();
				//stAnimator.SetBool(isWalking, true);
				StartCoroutine(SmoothTraverse(navAgent));
			}
		}
		else
		{
			/*playerState.UpdateStateOnStopMoving();*/
			StopWalking();
		}
	}

	
	IEnumerator SmoothTraverse(NavMeshAgent agent)
	{
		if (!agent.isOnOffMeshLink) yield break;

		OffMeshLinkData linkData = agent.currentOffMeshLinkData;
		Vector3 startPos = agent.transform.position;
		Vector3 endPos =
			new Vector3(linkData.endPos.x, agent.transform.position.y, linkData.endPos.z); // Keep consistent Y level

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
				
				//playerState.UpdateStateOnStopMoving();
				StopWalking();
			}
			/*else
			{
				playerState.UpdateStateOnStartMoving();
			}*/
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
				StartWalking(hit.point);
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

	//TODO: when we stop moving, call PLayerState.UpdateStateOnStopMoving() to set our new state
	public void OnDestinationReached()
	{
		//print("Reached destination!");
		foreach (Action method in methodsToCallWhenReachDestination)
		{
			method.Invoke();
		}

		methodsToCallWhenReachDestination = new List<Action>();
		StopWalking();
		
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

	//if have orb, change playerstate walking with orb or idling
	public bool IsCarryingOrb()
	{
		return (currentHeldOrb != null);
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
			}
		}
	}

	private void SetAnimatorIsMoving()
	{
		//stAnimator.SetBool(isWalking, navAgent.velocity.magnitude > 0.0001f);
		isMoving = navAgent.velocity.sqrMagnitude > 0.001f && navAgent.remainingDistance > navAgent.stoppingDistance;
		stAnimator.SetBool(isWalking, isMoving);
	}

	private void HandlePickUpOrbStart()
	{
		stAnimator.SetTrigger(pickedUpOrb);
	}

	private void HandlePickUpOrbEnd()
	{
		stAnimator.SetTrigger(droppedOrb);
	}

	private void UpdateMovementState()
	{
		//Debug.Log($"Movement State Changed: IsMoving = {isMoving}");

		// Check if the character is moving
		bool currentlyMoving = navAgent.velocity.sqrMagnitude > 0.01f &&
		                       navAgent.remainingDistance > navAgent.stoppingDistance;

		// Update the Animator only if the movement state changes

		if (currentlyMoving)
		{
			//If moving switch to walk 
			playerState.UpdateStateOnStartMoving();
		}
		/*else
		{
			//If not moving switch to idle
			playerState.UpdateStateOnStopMoving();
		}*/
	}

	public void StopWalking()
	{
		stAnimator.SetBool(isWalking, false);
		playerState.UpdateStateOnStopMoving();
		//isMoving = false;
	}

	public void StartWalking(Vector3 destination)
	{
		stAnimator.SetBool(isWalking, true);
		navAgent.SetDestination(destination);
		onDestinationReached = OnDestinationReached;
		//isMoving = true;
		playerState.UpdateStateOnStartMoving();
	}
	
	public void HandleCurrentState(PlayerStateType currentState, GameObject currentInteractable)
	{
		switch (currentState)
		{
			case PlayerStateType.IdleWithoutOrb:
				HandleIdleState();
				break;

			case PlayerStateType.WalkWithoutOrb:
				HandleWalkState();
				break;

			case PlayerStateType.WalkWithOrb:
				HandleWalkWithOrbState();
				break;

			case PlayerStateType.IdleWithOrb:
				HandleIdleWithOrbState();
				break;

			case PlayerStateType.NextToOrb:
				HandlePickupState();
				break;

			case PlayerStateType.NextToPodiumWithOrb:
				HandleNextToPodiumWithOrbState();
				break;
			case PlayerStateType.NextToWheel:
				HandleTurnWheelState(currentInteractable);
				break;
			default:
				Debug.Log("need handle");
				break;
		}
	}

	private void HandleIdleState()
	{
		StopWalking();
	}

	private void HandleWalkState()
	{
		//StartWalking();
	}

	private void HandleWalkWithOrbState()
	{
		
	}

	private void HandleIdleWithOrbState()
	{
		
	}

	private void HandlePickupState()
	{
		
	}

	private void HandleNextToPodiumWithOrbState()
	{
		DropCurrentOrb();
	}

	private void HandleTurnWheelState(GameObject currentInteractable)
	{
		currentWheel = currentInteractable.GetComponent<Wheel>();
		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			currentWheel.HandleWheelScroll();
		}
	}
}
