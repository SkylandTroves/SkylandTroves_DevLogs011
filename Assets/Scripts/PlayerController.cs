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
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Object = System.Object;

public class PlayerController : MonoBehaviour
{
	public AnimationStates AnimationStates;
	//[SerializeField] private PickUpController pickUp;
	[SerializeField] private GameObject clickParticle;
	[SerializeField] private Animator stAnimator;
	[SerializeField] private AudioClip OnClickSFX;
	[SerializeField] private float faceDelay = 0.5f;
	[SerializeField] private float rotationSpeed = 1000f;

	private List<PickUpController> pickUps;
	private NavMeshAgent navAgent;
	private Action onDestinationReached;
	private GameObject currentHeldOrb;
	//private PickUpController currentHeldOrbController;
	private bool isMoving = false;
	
	private float wheelInteractionTimer = 0f;
	private float wheelInteractionDuration = 0.2f;
	private bool isFacingWheel = false;

	private bool hasStartedTurning = false;

	private const string isWalking = "IsWalking";
	private const string pickedUpOrb = "PickedUpOrb";
	private const string droppedOrb = "DroppedOrb";

	private List<Action> methodsToCallWhenReachDestination = new List<Action>();
	//private Wheel currentWheel;

	private PlayerState playerState;
	public PauseMenu pauseMenu;
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
				print("*** AGENT ON OFF MESH LINK ***");
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
		print("*** ENDPOS: " + endPos);
		
		float duration = Vector3.Distance(startPos, endPos) / agent.speed;
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;
			
			agent.transform.position = Vector3.Lerp(startPos, endPos, t);

			yield return null;
		}

		

		agent.transform.position = endPos; // Ensure final position is accurate
		if (agent.isOnNavMesh)
		{
			agent.CompleteOffMeshLink();
		}
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
				//StopWalking();
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
			if (clickParticle != null && !pauseMenu.GetIsPaused())
			{
				// instantiate instance of particle effect 
				Instantiate(clickParticle, hit.point, Quaternion.identity);
				SoundController.instance.PlaySFX(OnClickSFX, transform, 1f);
			}
			else
			{
				Debug.LogWarning("Click particle is not assigned or game is paused!");
			}
		}

		/*if (Input.GetMouseButtonDown(1)) // RIGHT CLICK  - - - - - - - - - - - -
		{
			DropCurrentOrb();
		}*/

		/*if (Input.GetKeyDown(KeyCode.Escape))
		{
			QuitGame();
		}*/

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
		print(" *** SetHeldOrb: setting currentHeldOrb");
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
			print("*** DropCurrentOrb: dropping currentHeldOrb");
			//Debugger.UpdateMessage("dropcurrentorb: dropping");
			PickUpController orbController = currentHeldOrb.GetComponent<PickUpController>();
			if (currentHeldOrb != null)
			{
				orbController.DropObject();
				currentHeldOrb = null;
			}
		}
	}

	private void SetAnimatorIsMoving()
	{
		//stAnimator.SetBool(isWalking, navAgent.velocity.magnitude > 0.0001f);
		isMoving = navAgent.velocity.sqrMagnitude > 0.001f && navAgent.remainingDistance > navAgent.stoppingDistance;
		//stAnimator.SetBool(isWalking, isMoving);
	}

	public void HandlePickUpOrbStart()
	{
		SoundController.instance.PlaySFX(SoundController.instance.PickUpBallSFX, transform, 1f);
		//stAnimator.SetTrigger(pickedUpOrb);
	}

	public void HandlePickUpOrbEnd()
	{
		SoundController.instance.PlaySFX(SoundController.instance.DropBallSFX, transform, 1f);
		//stAnimator.SetTrigger(droppedOrb);
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
		//stAnimator.SetBool(isWalking, false);
		playerState.UpdateStateOnStopMoving();
		//isMoving = false;
	}

	public void StartWalking(Vector3 destination)
	{
		isFacingWheel = false;
		//stAnimator.SetBool(isWalking, true);
		navAgent.SetDestination(destination);
		
		//isMoving = true;
		playerState.UpdateStateOnStartMoving();
		onDestinationReached = OnDestinationReached;
		//Debugger.UpdateMessage("Started Walking");
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
				HandlePickupState(currentInteractable);
				break;

			case PlayerStateType.NextToPodiumWithOrb:
				HandleNextToPodiumWithOrbState(currentInteractable);
				break;
			case PlayerStateType.NextToWheelWithoutOrb:
				HandleTurnWheelState(currentState, currentInteractable);
				break;
			case PlayerStateType.NextToWheelWithOrb:
				HandleTurnWheelState(currentState, currentInteractable);
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
		if (Input.GetMouseButtonDown(1))
		{
			DropCurrentOrb();
			playerState.dropOrb();
			//playerState.UpdateStateOnStopMoving();
			//AnimationStates.ChangeToDrop();
			//Debugger.UpdateMessage("Dropped orb walking");
		}
	}

	private void HandleIdleWithOrbState()
	{
		if (Input.GetMouseButtonDown(1))
		{
			print("*** HandleIdleWithOrbState: right click drop orb ");
			DropCurrentOrb();
			playerState.dropOrb();
			//playerState.UpdateStateOnStopMoving();
			//Debugger.UpdateMessage("Dropped orb idling");
		}
	}

	private void HandlePickupState(GameObject currentInteractable)
	{
		if (currentHeldOrb == null)
		{
			print("*** HandlePickupState: currentHeldOrb is null");
			PickUpController currentHeldOrbController = currentInteractable.GetComponent<PickUpController>();
			if (currentHeldOrbController != null)
			{
				AnimationStates.ChangeToPickUp(playerState.getIsOrbOnPodium());
				print("*** HandlePickupState: currentOrb is not null");
				//StartCoroutine(WaitFor20Seconds(currentHeldOrbController));
				currentHeldOrbController.PickUpObject();
				
				SetHeldOrb(currentInteractable);
				
				
				
				//Debugger.UpdateMessage("Picked up orb");
			}
			playerState.UpdateStateOnStopMoving();
		}
		playerState.setIsOrbOnPodium(false);
	}
	
	IEnumerator WaitFor20Seconds(PickUpController currentHeldOrbController)
	{
		Debug.Log("Start waiting...");
		yield return new WaitForSeconds(.75f); // Wait for 0.20 seconds
		currentHeldOrbController.PickUpObject();
		Debug.Log("Finished waiting!");
	}
	
	//TODO: FIX SNAPPING TO PODIUM
	//TODO: make transition between walking with orb and idling without?
	private void HandleNextToPodiumWithOrbState(GameObject currentInteractable)
	{
		if (Input.GetMouseButtonDown(1))
		{
			print(" *** HandleNextToPodiumWithOrbState: mouse down, dropping orb");
			//PickUpController currentOrb = currentInteractable.GetComponent<PickUpController>();
			if (currentHeldOrb != null)
			{
				DropCurrentOrb();
			}
		}
	}
	
	private void HandleTurnWheelState(PlayerStateType currentState, GameObject currentInteractable)
	{
		float scrollInput = Input.GetAxis("Mouse ScrollWheel");
		Wheel currentWheel = currentInteractable.GetComponent<Wheel>();
		if (!isFacingWheel)
		{
			
			StartCoroutine(FaceThenInteract(currentWheel));
		}
		if (scrollInput != 0f)
		{
			hasStartedTurning = true;
			wheelInteractionTimer = wheelInteractionDuration;

			//Wheel currentWheelScroll = currentInteractable.GetComponent<Wheel>();
			currentWheel.HandleWheelScroll();
			AnimationStates.resumeAnimation();
		}
		else if (wheelInteractionTimer > 0f)
		{
			wheelInteractionTimer -= Time.deltaTime;
			if(hasStartedTurning)
			{
				AnimationStates.pauseAnimation();
			}
		}

		if (wheelInteractionTimer > 0f)
		{
			AnimationStates.resumeAnimation();
		}
		else
		{
			hasStartedTurning = false;
			if(hasStartedTurning)
			{
				AnimationStates.pauseAnimation();
			}
		}
		if (hasStartedTurning)
		{
			AnimationStates.UpdateState(currentState);
		}
	}

	private IEnumerator FaceThenInteract(Wheel wheel)
	{
		Vector3 wheelPos = wheel.transform.position;
		Vector3 toWheel = (wheelPos - transform.position);
		toWheel.y = 0f;

		Quaternion targetRot = Quaternion.LookRotation(toWheel.normalized);

		// Have player face wheel
		while (Quaternion.Angle(transform.rotation, targetRot) > 0.5f)
		{
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
			yield return null;
		}
		transform.rotation = targetRot;

		// Put player just outside the NavMeshObstacle
		NavMeshObstacle obstacle = wheel.GetComponent<NavMeshObstacle>();
		float bufferDistance = 1f; // small offset to avoid touching

		float obstacleRadius = 0.5f; // Default fallback radius

		Collider obstacleCollider = wheel.GetComponent<Collider>();
		if (obstacleCollider != null)
		{
			obstacleRadius = obstacleCollider.bounds.extents.magnitude;
		}

		Vector3 offsetDirection = -toWheel.normalized;
		Vector3 targetPosition = wheelPos + offsetDirection * (obstacleRadius + bufferDistance);

		targetPosition.y = transform.position.y;

		float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

		if (distanceToTarget > 0.25f && !isFacingWheel)
		{
			navAgent.isStopped = false;
			navAgent.SetDestination(targetPosition);

			while (Vector3.Distance(transform.position, targetPosition) > 0.25f)
			{
				yield return null;
			}
		}
		
		yield return new WaitForSeconds(faceDelay);

		isFacingWheel = true;
	}
}