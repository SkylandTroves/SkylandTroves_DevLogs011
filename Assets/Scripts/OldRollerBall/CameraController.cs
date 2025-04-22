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
    
    [Header("Camera Shake Settings")]
    [Tooltip("When checked, camera will continue following player during shake")]
    [SerializeField] private bool followDuringShake = false;
    
    private Vector3 offset; // offset between player and cam 
    private bool isShaking = false;
    private Vector3 originalPosition;
    private Vector3 currentShakeOffset = Vector3.zero;
    private Coroutine shakeCoroutine;
    
    void Start()
    {
        // Create an offset by subtracting the Camera's position from the player's position
        offset = transform.position - Player.transform.position;
    }

    // late update is after the standard 'Update()' loop runs, and just before each frame is rendered
    void LateUpdate()
    {
        if (isShaking)
        {
            if (followDuringShake)
            {
                // Update position to follow player and apply the current shake offset
                transform.position = Player.transform.position + offset + currentShakeOffset;
            }
            // When not following during shake, position is controlled entirely by the shake coroutine
        }
        else
        {
            // Normal following when not shaking
            transform.position = Player.transform.position + offset;
        }
    }

    public void SetShaking(bool shaking)
    {
        isShaking = shaking;
        
        // Reset shake offset when we stop shaking
        if (!shaking)
        {
            currentShakeOffset = Vector3.zero;
        }
    }
    
    public void ShakeCamera(float intensity, float duration)
    {
        // Stop any existing shake
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        
        // Store original position before shake starts
        originalPosition = transform.position;
        
        // Start new shake
        shakeCoroutine = StartCoroutine(ShakeRoutine(intensity, duration));
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        SetShaking(true);
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Calculate shake factor based on remaining time (fade out)
            float remainingTime = duration - elapsed;
            float shakeFactor = intensity * (remainingTime / duration);
            
            // Calculate new random shake offset
            currentShakeOffset = new Vector3(
                Random.Range(-1f, 1f) * shakeFactor,
                Random.Range(-1f, 1f) * shakeFactor,
                Random.Range(-1f, 1f) * shakeFactor * 0.5f  // Less movement on Z axis
            );
            
            if (!followDuringShake)
            {
                // When not following, directly set position using original position and shake offset
                transform.position = originalPosition + currentShakeOffset;
            }
            // When following, LateUpdate will handle applying the shake offset on top of player following
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset when done shaking
        if (!followDuringShake)
        {
            // Return to original position if not following
            transform.position = originalPosition;
        }
        
        SetShaking(false);
        shakeCoroutine = null;
    }
}