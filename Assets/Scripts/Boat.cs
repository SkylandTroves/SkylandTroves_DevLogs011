/*
Full Name: Aliya Rafei
Student ID:  2391746
rafei@chapman.edu
GAME 340 - 01
Assignment:  Final Project Submission
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boat : MonoBehaviour
{
    public GameObject Player;
    public Game Game;
    [SerializeField] private bool isLevelSwitchCatalyst;

    private bool hasCollider;
    private List<NavMeshAgent> agentsOnPlatform = new List<NavMeshAgent>();

    private void Start()
    {
        if (GetComponent<Collider>() != null)
        {
            hasCollider = true;
        }
        else
        {
            hasCollider = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasCollider)
        {
            AddNavMeshAgentToList(other);
            MoveBoat(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (hasCollider)
        {
            RemoveNavMeshAgentFromList(other);
        }
    }

    private void AddNavMeshAgentToList(Collider other)
    {
        if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            agentsOnPlatform.Add(agent);
        }
    }

    private void RemoveNavMeshAgentFromList(Collider other)
    {
        if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            agentsOnPlatform.Remove(agent);
        }
    }

    // This method is called from MovingPlatform.cs when the platform starts moving ONLY if its a boat
    public void OnMovingPlatformStarted()
    {
        RemoveNavMeshAgentAndParentPlayer();
        if (isLevelSwitchCatalyst)
        {
            GoToNextLevel();
        }
    }

    private void RemoveNavMeshAgentAndParentPlayer()
    {
        if (Player != null)
        {
            NavMeshAgent playerAgent = Player.GetComponent<NavMeshAgent>();
            if (playerAgent != null)
            {
                Destroy(playerAgent);  // Remove the NavMeshAgent component
                Debug.Log("player no longer nav mesh agent ");
            }
            Player.transform.SetParent(transform);
            
        }
    }

    private void MoveBoat(Collider other)
    {
        if (other.gameObject == Player)
        {
            Debug.Log("Player is on the boat");

            if (GetComponent<MovingPlatform>() is MovingPlatform movingPlatform)
            {
                Player.transform.SetParent(movingPlatform.transform);
                KeepPlayerFromMoving();
                movingPlatform.StartMoving();

                if (isLevelSwitchCatalyst)
                {
                    GoToNextLevel();
                }
            }
            else
            {
                Debug.LogError("MovingPlatform is not found on the boat object.");
            }
        }
    }

    private void KeepPlayerFromMoving()
    {
        Vector3 platformMoveDirection = (GetComponent<MovingPlatform>().GetPositionA() - GetComponent<MovingPlatform>().GetPositionB()).normalized;
        NavMeshAgent playerAgent = Player.GetComponent<NavMeshAgent>();
        float moveSpeed = playerAgent.speed;

        foreach (var agent in agentsOnPlatform)
        {
            agent.Warp(agent.transform.position + platformMoveDirection * moveSpeed);
        }
    }

    private IEnumerator WaitBeforeNextLevel()
    {
        yield return new WaitForSeconds(1);
        Game.LoadNextLevel();
    }

    private void GoToNextLevel()
    {
        StartCoroutine(WaitBeforeNextLevel());
    }
}
