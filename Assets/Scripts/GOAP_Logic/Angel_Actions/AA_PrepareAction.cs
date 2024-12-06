using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_PrepareAction : Actions
{
    public Animator openDoor;

    private bool wantShower = false;

    private Angel angelScript;


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

        angelScript = GetComponent<Angel>();

        if (angelScript == null)
        {
            Debug.LogWarning("Angel script not found on this GameObject.");
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

        if (angelScript != null)
        {
            angelScript.available = false;
        }

        return true;
    }

    public override bool PostPerform()
    {
        wantShower = false;
        return true;
    }
}
