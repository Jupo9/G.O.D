using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GA_PrepareAction : Actions
{
    public Animator openDoor;

    private bool wantShower = false;


    private void Start()
    {
        if (openDoor == null)
        {
            Transform childTransform = transform.Find("ShowerDoor"); 

            if (childTransform != null)
            {
                openDoor = childTransform.GetComponent<Animator>();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shower") && wantShower)
        {
            Invoke("OpenShowerDoor", 2f);
        }

    }

    private void OpenShowerDoor()
    {
        openDoor.Play("Shower Door Prototyp");
    }

    public override bool PrePerform()
    {
        wantShower = true;
        return true;
    }

    public override bool PostPerform()
    {
        wantShower = false;
        return true;
    }
}
