/*
Full Name: Aliya Rafei
Student ID:  2391746
rafei@chapman.edu
GAME 340 - 01
Assignment:  Final Project Submission
*/
using UnityEngine;
using System.Collections;

// NOTE: class is from ROLLER BALL PROJECT FROM PRATE - Aliya did not make changes 
public class CameraController : MonoBehaviour 
{
	public GameObject Player;
	private Vector3 offset; // offset between player and cam 
	
	void Start ()
	{
		// Create an offset by subtracting the Camera's position from the player's position
		offset = transform.position - Player.transform.position;
	}

	// late update is after the standard 'Update()' loop runs, and just before each frame is rendered..
	void LateUpdate ()
	{
		// Set the position of the Camera (the game object this script is attached to)
		// to the player's position, plus the offset amount
		transform.position = Player.transform.position + offset;
	}
	
	
}

