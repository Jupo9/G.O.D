using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_ChillComplete : Actions
{
    private Devil devil;

    private Building_IronMaiden targetBuilding;

    public void Start()
    {
        devil = GetComponent<Devil>();
    }

    public override bool PrePerform()
    {
        Dictionary<string, int> relevantState = GetRelevantDevilState();

        if (relevantState.ContainsKey("chilling"))
        {
            int chillValue = relevantState["chilling"];

            Debug.Log(chillValue);

            if (chillValue >= 1)
            {
                Debug.Log("Key 'chilling' has value " + chillValue + " Action will be skipped.");
                ApplyDevilEffects();
                return false;
            }
        }
        else
        {
            Debug.Log("PrePerform Check in Chilling: Key 'chilling' does not exist.");
        }

    GameObject[] ironBuildings = GameObject.FindGameObjectsWithTag("WO_Iron");

        if (ironBuildings.Length == 0)
        {
            Debug.LogWarning("No available Building_IronMaiden found.");
            return false;
        }

        GameObject closestBuilding = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject building in ironBuildings)
        {
            Building_IronMaiden ironScript = building.GetComponentInParent<Building_IronMaiden>();

            if (ironScript != null && ironScript.isAvailable)
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBuilding = building;
                    targetBuilding = ironScript;
                }
            }
        }

        if (closestBuilding == null)
        {
            Debug.Log("No Building_IronMaiden is available.");
            return false;
        }

        target = closestBuilding;
        targetBuilding.isAvailable = false;
        targetTag = "WO_Iron";

        agent.SetDestination(target.transform.position);

        StartCoroutine("ChillingRoutine");

        return true;
    }

    public override bool PostPerform()
    {
        ApplyDevilEffects();
        return true;
    }

    private IEnumerator ChillingRoutine()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        agent.isStopped = true;
        yield return new WaitForSeconds(5f);
        agent.isStopped = false;

        targetTag = "WI_Iron";
        GameObject newTarget = FindClosestAvailableBuildingInside();

        if (newTarget == null)
        {
            Debug.LogWarning("No target found with tag WI_Iron.");
            yield break;
        }

        target = newTarget;
        agent.SetDestination(target.transform.position);

        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        if (targetBuilding != null)
        {
            targetBuilding.CloseDoubleDoors();
        }

        //devil.isChilled = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(10f);

        if (targetBuilding != null)
        {
            targetBuilding.OpenDoubleDoors();
        }

        yield return new WaitForSeconds(5f);

        //devil.isChilled = false;
        agent.isStopped = false;

        targetTag = "WO_Iron";
        GameObject finalTarget = FindClosestAvailableBuilding();

        if (finalTarget == null)
        {
            Debug.LogWarning("No target found with tag WO_Iron.");
            yield break;
        }

        target = finalTarget;
        agent.SetDestination(target.transform.position);

        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        //devil.isChilled = false;

        targetBuilding.isAvailable = true;
    }

    private GameObject FindClosestAvailableBuilding()
    {
        GameObject[] ironBuildings = GameObject.FindGameObjectsWithTag("WO_Iron");

        GameObject closestBuilding = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject building in ironBuildings)
        {
            Building_IronMaiden ironScript = building.GetComponentInParent<Building_IronMaiden>();

            if (ironScript != null)
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBuilding = building;
                    targetBuilding = ironScript;
                    targetBuilding.isAvailable = false;
                }
            }
        }

        return closestBuilding;
    }

    private GameObject FindClosestAvailableBuildingInside()
    {
        GameObject[] ironBuildings = GameObject.FindGameObjectsWithTag("WI_Iron");

        GameObject closestBuilding = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject building in ironBuildings)
        {
            Building_IronMaiden ironScript = building.GetComponentInParent<Building_IronMaiden>();

            if (ironScript != null)
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBuilding = building;
                    targetBuilding = ironScript;
                    targetBuilding.isAvailable = false;
                }
            }
        }

        return closestBuilding;
    }
}
