using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_Chilling : Actions
{
    private Building_IronMaiden buildingIronMaiden;

    public bool done = false;

    private void Start()
    {
        GameObject ironParent = GameObject.FindWithTag("Iron");

        if (ironParent != null)
        {
            buildingIronMaiden = ironParent.GetComponentInChildren<Building_IronMaiden>();
        }

        if (buildingIronMaiden == null)
        {
            Debug.LogWarning("Building_IronMaiden script not found on IronMaidenBuilding.");
        }
    }

    private void CloseDoors()
    {
        buildingIronMaiden.CloseDoubleDoors();
    }

    public override bool PrePerform()
    {
        Invoke("CloseDoors", 2f);

        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);
        if (buildings.Length == 0)
        {
            return false;
        }

        GameObject closestBuilding = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject build in buildings)
        {
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

        buildingIronMaiden = closestBuilding.GetComponentInParent<Building_IronMaiden>();
        if (buildingIronMaiden == null)
        {
            Debug.LogWarning("Building_IronMaiden script not found on the closest building.");
            return false;
        }

        return true;
    }

    public override bool PostPerform()
    {
        done = true;
        return true;
    }

}
