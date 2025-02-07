using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_Working : Actions
{
    private Building_Fire buildingFire;

    private void Start()
    {
        GameObject fireParent = GameObject.FindWithTag("Devil_WorkBuilding");

        if (fireParent != null)
        {
            buildingFire = fireParent.GetComponentInParent<Building_Fire>();
        }

        /*if (buildingFire == null)
        {
            Debug.LogWarning("Building_Fire script not found on FireBuilding.");
        }*/
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
            /// IDK ob das hier noch probleme später macht, mal schauen ^^
            Debug.LogWarning("Building_Fire script not found on the closest building.");
            return false;
        }

        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }
}
