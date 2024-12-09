using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_CleanAction : Actions
{
    private Building_IronMaiden buildingIronMaiden;

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

    public override bool PrePerform()
    {
        agent.isStopped = true;
        StartCoroutine(WaitBeforeAction());
        return false;
    }

    public override bool PostPerform()
    {
        return true;
    }

    private IEnumerator WaitBeforeAction()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);
        if (buildings.Length == 0)
        {
            yield return false;
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
           yield return false;
        }

        target = closestBuilding;
        agent.SetDestination(target.transform.position);

        buildingIronMaiden = closestBuilding.GetComponentInParent<Building_IronMaiden>();
        if (buildingIronMaiden == null)
        {
            Debug.LogWarning("Building_IronMaiden script not found on the closest building.");
            yield return false;
        }

        buildingIronMaiden.OpenDoubleDoors();

        yield return new WaitForSeconds(4);

        agent.isStopped = false;

        yield return new WaitForSeconds(2);

        buildingIronMaiden.isAvailable = true;
    }
}
