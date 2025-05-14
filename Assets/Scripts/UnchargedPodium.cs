using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnchargedPodium : PodiumController
{
    [SerializeField] private GameObject chargedPodiumPrefab; 
    [SerializeField] private bool loadNextLevelOnCharged = false; 
    [SerializeField] private float nextLevelDelay = 2.0f; 

    private SceneController sceneController;

    private void Start()
    {
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
            if (loadNextLevelOnCharged && sceneController != null)
            {
                Debug.Log("GO TO LEVEL 3");
                sceneController.StartCoroutine(DelayedLoadNextLevel());
            }

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
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Charged podium prefab is not assigned!");
        }
    }


    private IEnumerator DelayedLoadNextLevel()
    {
        yield return new WaitForSeconds(nextLevelDelay);
        
        int currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        
        Debug.Log("ACTUALLY LOADING NEXT LEVEL NOW");
        
        sceneController.GoToNextLevel(currentLevel, false); 
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