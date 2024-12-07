using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_PrepareAction : Actions
{
    private bool wantShower = false;

    private Angel angelScript;
    private Building_Shower buildingShower;


    private void Start()
    {
        angelScript = GetComponent<Angel>();

        if (angelScript == null)
        {
            Debug.LogWarning("Angel script not found on this GameObject.");
        }

        GameObject showerParent = GameObject.FindWithTag("Shower");

        if (showerParent != null)
        {
            buildingShower = showerParent.GetComponentInChildren<Building_Shower>();
        }

        if (buildingShower == null)
        {
            Debug.LogWarning("Building_Shower script not found on ShowerBuilding.");
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
        buildingShower.OpenDoorAnimation();
    }


    public override bool PrePerform()
    {
        wantShower = true;

        if (angelScript != null)
        {
            angelScript.available = false;
        }

        if (buildingShower != null && wantShower)
        {
            buildingShower.isAvailable = false;
        }

        return true;
    }

    public override bool PostPerform()
    {
        wantShower = false;
        return true;
    }
}
