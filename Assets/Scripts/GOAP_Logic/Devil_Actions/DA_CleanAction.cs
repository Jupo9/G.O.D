using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_CleanAction : Actions
{
    private Building_IronMaiden buildingIronMaiden;

    private Building_Fire buildingFire;

    public bool doneChill = false;
    public bool doneWork = false;
    public bool foundBuilding = false;

    private void Start()
    {
        if (targetTag == "WO_Iron")
        {
            GameObject ironParent = GameObject.FindWithTag("Iron");

            if (ironParent != null)
            {
                buildingIronMaiden = ironParent.GetComponentInChildren<Building_IronMaiden>();
            }

            /*if (buildingIronMaiden == null)
            {
                Debug.LogWarning("Building_IronMaiden script not found on IronMaidenBuilding.");
            }*/
        }

        if (targetTag == "WO_Fire")
        {
            GameObject fireParent = GameObject.FindWithTag("FIRE");

            if (fireParent != null)
            {
                buildingFire = fireParent.GetComponentInChildren<Building_Fire>();
            }
            /*if (buildingFire == null)
            {
                Debug.LogWarning("Building_Fire script not found on FireBuilding.");
            }*/
        }

    }

    public override bool PrePerform()
    {
        foundBuilding = false;

        if (targetTag == "WO_Iron")
        {
            Dictionary<string, int> relevantState = GetRelevantDevilState();

            if (relevantState.ContainsKey("cleanChill"))
            {
                int evilValue = relevantState["cleanChill"];

                if (evilValue <= 1)
                {
                    Debug.Log("Key 'cleanChill' has value 1. Action will be skipped.");
                    doneChill = true;
                    ApplyDevilEffects();
                    return false;
                }
            }
            else
            {
                Debug.Log("PrePerform Check in Bully: Key 'cleanChill' does not exist.");
                return false;
            }
        }
        foundBuilding = true;
        agent.isStopped = true;

        if (targetTag == "WO_Iron")
        {
            StartCoroutine(WaitBeforeActionIron());
            return true;
        }

        if (targetTag == "WO_Fire")
        {
            StartCoroutine(WaitBeforeActionFire());
            return true;
        }

        else
        {
            return false;
        }

    }

    public override bool PostPerform()
    {
        if (targetTag == "WO_Iron")
        {
            doneChill = true;
        }

        if (targetTag == "WO_Fire")
        {
            doneWork = true;
        }

        return true;
    }

    private IEnumerator WaitBeforeActionIron()
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
        buildingIronMaiden.AddBuilding();
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

        buildingFire.fireIsOpen = true;
        buildingFire.devilInside = false;
    }
}
