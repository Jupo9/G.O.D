using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_Working : Actions
{
    private Building_Fire buildingFire;

    private void Start()
    {
        GameObject lightParent = GameObject.FindWithTag("FIRE");

        if (lightParent != null)
        {
            buildingFire = lightParent.GetComponentInParent<Building_Fire>();
        }

        if (buildingFire == null)
        {
            Debug.LogWarning("Building_Fire script not found on FireBuilding.");
        }
    }

    public override bool PrePerform()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);
        if (buildings.Length == 0)
        {
            return false;
        }

        GameObject closestBuilding = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject build in buildings)
        {
            Building_Fire buildingLightScript = build.GetComponentInParent<Building_Fire>();
            float distance = Vector3.Distance(this.transform.position, build.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBuilding = build;
            }

        }

        if (closestBuilding == null)
        {
            return false;
        }

        target = closestBuilding;
        agent.SetDestination(target.transform.position);

        buildingFire = closestBuilding.GetComponentInParent<Building_Fire>();
        if (buildingFire == null)
        {
            Debug.LogWarning("Building_Light script not found on the closest building.");
            return false;
        }

        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }
}
