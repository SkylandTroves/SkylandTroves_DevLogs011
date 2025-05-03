using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnchargedPodium : PodiumController
{
    [SerializeField] private GameObject chargedPodiumPrefab; // Reference to the charged orb prefab
    [SerializeField] private bool loadNextLevelOnCharged = false; // Inspector option to load next level
    [SerializeField] private float nextLevelDelay = 2.0f; // Delay before loading next level

    // Reference to the SceneController
    private SceneController sceneController;

    private void Start()
    {
        // Find the SceneController in the scene
        sceneController = FindObjectOfType<SceneController>();
        if (sceneController == null && loadNextLevelOnCharged)
        {
            Debug.LogWarning("SceneController not found. Next level loading won't work.");
        }
    }

    public override void OnCollisionEnter(Collision other)
    {
        print("*** OVERRIDE OnCollisionEnter : UNCHARGED PODIUM SNAPPING ORB");
        bool isCharged = other.gameObject.CompareTag("energyOrbCharged");
        
        if (other.gameObject == Orb && isCharged)
        {
            print(" *** OVERRIDE OnCollisionEnter: SWITCH TO CHARGED PODIUM");
            SwitchToChargedPodium();
        }
        else if (other.gameObject == Orb && !isCharged)
        {
            print(" *** OVERRIDE OnCollisionEnter: put uncharged on podium ");
            PutUnChargedOnPodium();
        }
    }

    public override void SnapOrb()
    {
        base.SnapOrb();
    }

    private void SwitchToChargedPodium()
    {
        if (chargedPodiumPrefab != null)
        {
            // Check if we should load the next level
            if (loadNextLevelOnCharged && sceneController != null)
            {
                Debug.Log("GO TO LEVEL 3");
                // Instead of starting a coroutine that will be destroyed, directly handle level loading
                // First start a coroutine on the SceneController which won't be destroyed
                sceneController.StartCoroutine(DelayedLoadNextLevel());
            }

            // Continue with the normal podium switch process
            GameObject newPodium = Instantiate(chargedPodiumPrefab, transform.position, transform.rotation);
            PodiumController podiumController = newPodium.GetComponent<PodiumController>();
            podiumController.SetOrb(GetOrb());
            podiumController.SetMovingPlatforms(GetMovingPlatformsList());
            PodiumController thisController = GetComponent<PodiumController>();
            CopyShakeParameters(thisController, podiumController);
            podiumController.transform.SetParent(transform.parent);

            GetOrb().GetComponent<Rigidbody>().useGravity = false;
            GetOrb().GetComponentInChildren<Collider>().enabled = false;
            GetOrb().transform.SetParent(podiumController.GetOrbPositionOnPodium());
            GetOrb().transform.localPosition = Vector3.zero;

            Debug.Log("HERE");
            // Destroy the uncharged podium
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Charged podium prefab is not assigned!");
        }
    }

    // In UnchargedPodium.cs, modify the DelayedLoadNextLevel() method:

    private IEnumerator DelayedLoadNextLevel()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(nextLevelDelay);
        
        // Get the current level index
        int currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        
        Debug.Log("ACTUALLY LOADING NEXT LEVEL NOW");
        
        // Load the next level - passing false to indicate we're loading from a podium activation
        // Add a new parameter to this method call
        sceneController.GoToNextLevel(currentLevel, false);  // <-- Add 'false' parameter
    }

    private void CopyShakeParameters(PodiumController source, PodiumController target)
    {
        target.SetShakeIntensity(source.GetShakeIntensity());
        target.SetShakeDuration(source.GetShakeDuration());
        target.SetActivationDelay(source.GetActivationDelay());
    }

    private void PutUnChargedOnPodium()
    {
        base.SnapOrb();
    }
}