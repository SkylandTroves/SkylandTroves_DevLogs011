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
using UnityEngine.UIElements;
// NOTE: some code from https://www.youtube.com/watch?v=qLckTZ2E1JA 

public class ConstMovingPlatform : MonoBehaviour
{
    public Transform positionA;
    public Transform positionB;
    [SerializeField] private float moveSpeed; 

    private float dockDuration = 2f;
    private List<NavMeshAgent> agentsOnPlatform = new List<NavMeshAgent>();
    private List<Vector3> positions = new List<Vector3>();
    //private Vector3[] Positions;

    private void Start()
    {
        InitializePositions();
        StartCoroutine(MovePlatform());
    }
    
    private void InitializePositions()
    {
        if (positionA != null) positions.Add(positionA.position);
        if (positionB != null) positions.Add(positionB.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            agentsOnPlatform.Add(agent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            agentsOnPlatform.Remove(agent);
        }
    }

    private IEnumerator MovePlatform()
    {
        // Initialize the platform's position to the first point
        transform.position = positions[0];
        int positionIndex = 0;
        int lastPositionIndex;
        WaitForSeconds wait = new WaitForSeconds(dockDuration);

        while (true)
        {
            lastPositionIndex = positionIndex;
            positionIndex = (positionIndex + 1) % positions.Count; // Cycle through positions

            Vector3 platformMoveDirection = (positions[positionIndex] - positions[lastPositionIndex]).normalized;
            float distance = Vector3.Distance(positions[lastPositionIndex], positions[positionIndex]);
            float distanceTraveled = 0;

            // Move towards the next position
            while (distanceTraveled < distance)
            {
                float step = moveSpeed * Time.deltaTime;
                transform.position += platformMoveDirection * step;
                distanceTraveled += step;

                // Move agents with the platform
                foreach (var agent in agentsOnPlatform)
                {
                    agent.Warp(agent.transform.position + platformMoveDirection * step);
                }

                yield return null;
            }

            // Snap to the target position
            transform.position = positions[positionIndex];

            // Wait at the dock
            yield return wait;
        }
    }

    
}
