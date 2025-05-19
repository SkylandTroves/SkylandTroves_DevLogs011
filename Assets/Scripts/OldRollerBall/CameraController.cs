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
        offset = transform.position - Player.transform.position;
    }

    void LateUpdate()
    {
        if (isShaking)
        {
            if (followDuringShake)
            {
                transform.position = Player.transform.position + offset + currentShakeOffset;
            }
        }
        else
        {
            transform.position = Player.transform.position + offset;
        }
    }

    public void SetShaking(bool shaking)
    {
        isShaking = shaking;
        
        if (!shaking)
        {
            currentShakeOffset = Vector3.zero;
        }
    }
    
    public void ShakeCamera(float intensity, float duration)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        
        originalPosition = transform.position;
        
        shakeCoroutine = StartCoroutine(ShakeRoutine(intensity, duration));
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        SetShaking(true);
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float remainingTime = duration - elapsed;
            float shakeFactor = intensity * (remainingTime / duration);
            
            currentShakeOffset = new Vector3(
                Random.Range(-1f, 1f) * shakeFactor,
                Random.Range(-1f, 1f) * shakeFactor,
                Random.Range(-1f, 1f) * shakeFactor * 0.5f 
            );
            
            if (!followDuringShake)
            {
                transform.position = originalPosition + currentShakeOffset;
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!followDuringShake)
        {
            transform.position = originalPosition;
        }
        
        SetShaking(false);
        shakeCoroutine = null;
    }
}