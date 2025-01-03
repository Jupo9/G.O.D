using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_CleanAction : Actions
{
    private Building_IronMaiden buildingIronMaiden;

    private Building_Fire buildingFire;

    public bool done = false;

    private void Start()
    {
        if (targetTag == "WO_Iron")
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

        if (targetTag == "WO_Fire")
        {
            GameObject fireParent = GameObject.FindWithTag("FIRE");

            if (fireParent != null)
            {
                buildingFire = fireParent.GetComponentInChildren<Building_Fire>();
            }
            if (buildingFire == null)
            {
                Debug.LogWarning("Building_Fire script not found on FireBuilding.");
            }
        }

    }

    public override bool PrePerform()
    {
        agent.isStopped = true;

        if (targetTag == "WO_Iron")
        {
            StartCoroutine(WaitBeforeActionLight());
        }

        if (targetTag == "WO_Fire")
        {
            StartCoroutine(WaitBeforeActionFire());
        }

        return false;
    }

    public override bool PostPerform()
    {
        if (targetTag == "WO_Iron")
        {
            done = true;
        }
        return true;
    }

    private IEnumerator WaitBeforeActionLight()
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

    private IEnumerator WaitBeforeActionFire()
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

        buildingFire = closestBuilding.GetComponentInParent<Building_Fire>();
        if (buildingFire == null)
        {
            Debug.LogWarning("Building_Fire script not found on the closest building.");
            yield return false;
        }

        buildingFire.IncreaseFireAmount();

        yield return new WaitForSeconds(4);

        agent.isStopped = false;

        yield return new WaitForSeconds(2);

        buildingFire.isAvailable = true;
        buildingFire.devilInside = false;
    }
}
