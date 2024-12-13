using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_Transport : Actions
{
    private Building_Storage storage;

    public override bool PrePerform()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);
        if (buildings.Length == 0)
        {
            Debug.LogWarning("No building found with target Tag Store");
            return false;
        }

        GameObject closestBuilding = null;
        float closestDistance = Mathf.Infinity;

        foreach(GameObject build in buildings)
        {
            Building_Storage storageScript = build.GetComponentInParent<Building_Storage>();
            float distance = Vector3.Distance(this.transform.position, build.transform.position);
            
            if (distance < closestDistance) 
            {
                closestDistance = distance;
                closestBuilding = build;
            }

        }

        if (closestBuilding == null)
        {
            Debug.LogWarning("No valid building found for transport.");
            return false;
        }


        target = closestBuilding;
        agent.SetDestination(target.transform.position);

        storage = closestBuilding.GetComponentInParent<Building_Storage>();
        if (storage == null) 
        {
            Debug.LogWarning("Building_Storage script missing on the closest building.");
            return false;
        }


        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }

}


