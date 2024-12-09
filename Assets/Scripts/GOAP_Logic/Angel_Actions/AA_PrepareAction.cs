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

        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);
        if (buildings.Length == 0)
        {
            return false;
        }

        GameObject closestBuilding = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject build in buildings)
        {
            Building_Shower buildingShowerScript = build.GetComponentInParent<Building_Shower>();
            if (buildingShowerScript != null && buildingShowerScript.isAvailable)
            {
                float distance = Vector3.Distance(this.transform.position, build.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBuilding = build;
                }
            }

        }

        if (closestBuilding == null)
        {
            return false;
        }

        target = closestBuilding;
        agent.SetDestination(target.transform.position);


        buildingShower = closestBuilding.GetComponentInParent<Building_Shower>();
        if (buildingShower == null)
        {
            buildingShower.isAvailable = false;
            Debug.LogWarning("Building_Shower script not found on the closest building.");
            return false;
        }

        return true;
    }

    public override bool PostPerform()
    {
        wantShower = false;
        angelScript.isStunned = true;
        return true;
    }
}