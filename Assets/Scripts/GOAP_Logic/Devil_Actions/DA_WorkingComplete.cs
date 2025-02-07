using System.Collections;
using UnityEngine;

public class DA_WorkingComplete : Actions
{
    private Building_Fire targetBuilding;

    private bool breaktest = false;

    public override bool PrePerform()
    {
        breaktest = false;

        GameObject[] fireBuildings = GameObject.FindGameObjectsWithTag("WO_Fire");

        if (fireBuildings.Length == 0)
        {
            Debug.LogWarning("No available Building_Fire found.");
            return false;
        }

        GameObject closestBuilding = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject building in fireBuildings)
        {
            Building_Fire fireScript = building.GetComponentInParent<Building_Fire>();

            if (fireScript != null && fireScript.isAvailable)
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBuilding = building;
                    targetBuilding = fireScript;
                }
            }
        }

        if (closestBuilding == null)
        {
            Debug.Log("No Building_Fire is available.");
            return false;
        }

        target = closestBuilding;
        targetBuilding.isAvailable = false;
        targetTag = "WO_Fire";

        agent.SetDestination(target.transform.position);

        StartCoroutine("WorkingRoutine");

        duration = 35f;

        return true;
    }

    public override bool PostPerform()
    {
        if (!breaktest)
        {
            //Debug.Log("WaitForEnding");
            return false;
        }

        //Debug.Log("finishWork");
        return true;
    }

    private IEnumerator WorkingRoutine()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.3f)
        {
            yield return null;
        }

        agent.isStopped = true;
        yield return new WaitForSeconds(5f);
        agent.isStopped = false;

        targetTag = "WI_Fire";
        GameObject newTarget = GameObject.FindGameObjectWithTag("WI_Fire");

        if (newTarget == null)
        {
            Debug.LogWarning("No target found with tag WI_Fire.");
            yield break;
        }

        target = newTarget;
        agent.SetDestination(target.transform.position);

        while (Vector3.Distance(transform.position, target.transform.position) > 1.3f)
        {
            yield return null;
        }

        agent.isStopped = true;

        yield return new WaitForSeconds(10f);

        agent.isStopped = false;

        targetTag = "FireStore";
        GameObject finalTarget = GameObject.FindGameObjectWithTag("FireStore");

        if (finalTarget == null)
        {
            Debug.LogWarning("No target found with tag FireStore.");
            yield break;
        }

        target = finalTarget;
        agent.SetDestination(target.transform.position);

        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        if (targetBuilding != null)
        {
            targetBuilding.IncreaseFireAmount();
        }

        targetBuilding.isAvailable = true;
        breaktest = true;
    }
}
