using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothPickUp : PickUpController
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wrist"))
        {
            print("COMPARED WRIST ");
            PickUpController parentOrb = transform.parent.GetComponentInParent<PickUpController>();
            print(parentOrb.gameObject.name);
            if (parentOrb != null)
            {
                GetComponent<Collider>().isTrigger = false;
                parentOrb.putOrbInHands();
            }
        }
    }
}
