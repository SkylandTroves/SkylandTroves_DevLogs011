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
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Object = System.Object;

public class PlayerController : MonoBehaviour
{
	public AnimationStates AnimationStates;
	[SerializeField] private GameObject clickParticle;
	[SerializeField] private Animator stAnimator;
	[SerializeField] private AudioClip OnClickSFX;
	[SerializeField] private float faceDelay = 0.5f;
	[SerializeField] private float rotationSpeed = 1000f;

	private List<PickUpController> pickUps;
	private NavMeshAgent navAgent;
	private Action onDestinationReached;
	private GameObject currentHeldOrb;
	
	private float wheelInteractionTimer = 0f;
	private float wheelInteractionDuration = 0.2f;
	private bool isFacingWheel = false;
	private bool hasStartedTurning = false;
	private bool isMoving;


	private List<Action> methodsToCallWhenReachDestination = new List<Action>();

	private PlayerState playerState;
	public PauseMenu pauseMenu;
	private Collider wristCollider;

	private Rigidbody rb;
	private void Awake()
	{
		navAgent = GetComponent<NavMeshAgent>();
	}

	void Start()
	{
		playerState = GetComponent<PlayerState>();
		navAgent = GetComponent<NavMeshAgent>();
		navAgent.autoTraverseOffMeshLink = false;
		rb = GetComponent<Rigidbody>();
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
		Vector3 endPos =
			new Vector3(linkData.endPos.x, agent.transform.position.y, linkData.endPos.z); // Keep consistent Y level
		
		float duration = Vector3.Distance(startPos, endPos) / agent.speed;
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;
			
			agent.transform.position = Vector3.Lerp(startPos, endPos, t);

			yield return null;
		}
		

		agent.transform.position = endPos;
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
				StartWalking(hit.point);
			}

			// Check if clickParticle is not null before instantiating
			if (clickParticle != null && !pauseMenu.GetIsPaused())
			{
				// instantiate instance of particle effect 
				Instantiate(clickParticle, hit.point, Quaternion.identity);
				SoundController.instance.PlaySFX(SoundController.instance.ClickSFX, transform, .25f);
			}
			else
			{
				Debug.LogWarning("Click particle is not assigned or game is paused!");
			}
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
			PickUpController orbController = currentHeldOrb.GetComponent<PickUpController>();
			if (currentHeldOrb != null)
			{
				orbController.DropObject();
				currentHeldOrb = null;
			}
		}
	}

	public void ToggleWristCollider()
	{
		wristCollider.enabled = !wristCollider.enabled;
	}

	private void SetAnimatorIsMoving()
	{
		//stAnimator.SetBool(isWalking, navAgent.velocity.magnitude > 0.0001f);
		isMoving = navAgent.velocity.sqrMagnitude > 0.001f && navAgent.remainingDistance > navAgent.stoppingDistance;
		//stAnimator.SetBool(isWalking, isMoving);
	}

	public void HandlePickUpOrbStart()
	{
		SoundController.instance.PlaySFX(SoundController.instance.PickUpBallSFX, transform, .5f);
	}

	public void HandlePickUpOrbEnd()
	{
		SoundController.instance.PlaySFX(SoundController.instance.DropBallSFX, transform, .5f);
	}

	private void UpdateMovementState()
	{
		// Check if the character is moving
		bool currentlyMoving = navAgent.velocity.sqrMagnitude > 0.01f &&
		                       navAgent.remainingDistance > navAgent.stoppingDistance;

		// Update the Animator only if the movement state changes

		if (currentlyMoving)
		{
			//If moving switch to walk 
			playerState.UpdateStateOnStartMoving();
		}
	}

	public void StopWalking()
	{
		playerState.UpdateStateOnStopMoving();
	}

	public void StartWalking(Vector3 destination)
	{
		isFacingWheel = false;
		navAgent.SetDestination(destination);
		
		playerState.UpdateStateOnStartMoving();
		onDestinationReached = OnDestinationReached;
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
				HandleTurnWheelState(currentState, currentInteractable); //kevin
				break;
			case PlayerStateType.NextToWheelWithOrb:
				HandleTurnWheelState(currentState, currentInteractable);
				break;
			default:
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
		}
	}

	private void HandleIdleWithOrbState()
	{
		if (Input.GetMouseButtonDown(1))
		{
			DropCurrentOrb();
			playerState.dropOrb();
		}
	}
    
	private void HandlePickupState(GameObject currentInteractable, bool isResetting = false)
	{
		if (currentHeldOrb == null)
		{
			PickUpController currentHeldOrbController = currentInteractable.GetComponent<PickUpController>();

			if (currentHeldOrbController != null)
			{
				bool canPickUp = false;

				if (isResetting)
				{
					// Skip obstruction check
					canPickUp = true;
					
				}
				else
				{
					// Check if there's anything between the player and orb
					Vector3 direction = currentInteractable.transform.position - transform.position;
					float distance = direction.magnitude;

					Ray ray = new Ray(transform.position, direction.normalized);
					if (Physics.Raycast(ray, out RaycastHit hit, distance))
					{
						if (hit.collider.gameObject == currentInteractable
						    || hit.collider.gameObject.CompareTag("podiumCharged")
						    || hit.collider.gameObject.CompareTag("podiumUncharged")
						    || hit.collider.gameObject.transform.parent.CompareTag("energyOrbCharged")
						    || hit.collider.gameObject.transform.parent.CompareTag("energyOrbUncharged"))
						{
							canPickUp = true;
						}
					}
				}

				if (canPickUp)
				{
					currentHeldOrbController.PickUpObject();
					SetHeldOrb(currentInteractable);
					AnimationStates.ChangeToPickUp(playerState.getIsOrbOnPodium());
				}
				playerState.UpdateStateOnStopMoving();
			}
		}

		playerState.setIsOrbOnPodium(false);

		if (Input.GetMouseButtonDown(1))
		{
			DropCurrentOrb();
			playerState.dropOrb();
		}
	}
	
	private void HandleNextToPodiumWithOrbState(GameObject currentInteractable)
	{
		if (Input.GetMouseButtonDown(1))
		{
			if (currentHeldOrb != null)
			{
				DropCurrentOrb();
				playerState.dropOrb();
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
			if (!hasStartedTurning)
			{
				hasStartedTurning = true;
				SoundController.instance.PlayWheelTurningSound(currentInteractable.transform);
			}
			
			wheelInteractionTimer = wheelInteractionDuration;
			currentWheel.HandleWheelScroll();
			AnimationStates.resumeAnimation();
		}
		else if (wheelInteractionTimer > 0f)
		{
			wheelInteractionTimer -= Time.deltaTime;
		}
		if (wheelInteractionTimer <= 0f && hasStartedTurning)
		{
			SoundController.instance.StopWheelTurningSound(currentInteractable.transform);
			hasStartedTurning = false;
			AnimationStates.pauseAnimation();
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
			navAgent.isStopped = true;
			navAgent.ResetPath();

			float moveSpeed = 3f;

			while (Vector3.Distance(transform.position, targetPosition) > 0.25f)
			{
				Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
				newPosition.y = transform.position.y;
				transform.position = newPosition;

				yield return null;
			}
		}
		
		yield return new WaitForSeconds(faceDelay);

		isFacingWheel = true;
	}
	

	private void OnTriggerExit(Collider other)
	{
		transform.parent = null;
		rb.constraints = RigidbodyConstraints.None;
	}

	public void HandleResetOrb(GameObject resetOrb)
	{
		HandlePickupState(resetOrb, true);
	}
}