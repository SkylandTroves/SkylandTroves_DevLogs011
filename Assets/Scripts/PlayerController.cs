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

	private List<Action> methodsToCallWhenReachDestination = new List<Action>();

	private void Awake()
	{
		navAgent = GetComponent<NavMeshAgent>();
	}

	void Start()
	{
		navAgent = GetComponent<NavMeshAgent>();
	}

	void Update()
	{
		GetPlayerInput();
		CheckArrivedAtDestination();
		
		//SetAnimatorIsMoving();
		UpdateMovementState();
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
		layerMask = ~layerMask; // invert the mask to exclude teh player 

		Vector3 rayOrigin = Camera.main.transform.position;
		Vector3 rayDirection = Camera.main.transform.forward;
		float rayLength = 100f;
		Color rayColor = Color.red;
		
		Debug.DrawRay(rayOrigin, rayDirection * rayLength, rayColor);
		
		if (Input.GetMouseButtonDown(0)) // left click - - - - - - - - - - - -
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
			{
				navAgent.SetDestination(hit.point);
				onDestinationReached = OnDestinationReached;
				isMoving = true;
			}
			
			// instantiate instance of particle effect 
			Instantiate(clickParticle,hit.point ,Quaternion.identity);
		}

		if (Input.GetMouseButtonDown(1)) // right click - - - - - - - - - - - -
		{
			DropCurrentOrb();
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			QuitGame();
		}
		
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
		bool currentlyMoving = navAgent.velocity.sqrMagnitude > 0.01f && navAgent.remainingDistance > navAgent.stoppingDistance;

		// Update the Animator only if the movement state changes
		if (isMoving != currentlyMoving)
		{
			isMoving = currentlyMoving;
			stAnimator.SetBool(isWalking, isMoving);
		}
	}


}