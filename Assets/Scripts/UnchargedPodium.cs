using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UnchargedPodium : PodiumController
{
    [SerializeField] private GameObject chargedPodiumPrefab; // Reference to the charged orb prefab
    public override void OnCollisionEnter(Collision other)
    {
        print("*** OVERRIDE OnCollisionEnter : UNCHARGED PODIUM SNAPPING ORB");
        bool isCharged = other.gameObject.CompareTag("energyOrbCharged");
        if (other.gameObject == Orb && isCharged)
        {
            //TODO: podiumcontroller.cs oncollisionenter not being triggered which means snapOrb isnt being called
            print(" *** OVERRIDE OnCollisionEnter: SWITCH TO CHARGED PODIUM");
            SwitchToChargedPodium();
        }
        else if (other.gameObject == Orb &&!isCharged)
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
            // Instantiate the charged podium at the current position and rotation of the uncharged podium
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
            //destroy the uncharged podium (or deactivate it)
            Destroy(gameObject); //ameObject.SetActive(false) //to disable instead
        }
        else
        {
            Debug.LogError("Charged podium prefab is not assigned!");
        }
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