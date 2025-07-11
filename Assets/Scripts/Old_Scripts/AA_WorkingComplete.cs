using System.Collections;
using UnityEngine;

public class AA_WorkingComplete : Actions
{
    private Building_Light targetBuilding;

    private bool breaktest = false;

    public override bool PrePerform()
    {
        breaktest = false;

        GameObject[] lightBuildings = GameObject.FindGameObjectsWithTag("WO_Light");

        if (lightBuildings.Length == 0)
        {
            Debug.LogWarning("No available Building_Fire found.");
            return false;
        }

        GameObject closestBuilding = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject building in lightBuildings)
        {
            Building_Light lightScript = building.GetComponentInParent<Building_Light>();

            if (lightScript != null && lightScript.isAvailable)
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBuilding = building;
                    targetBuilding = lightScript;
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
        targetTag = "WO_Light";

        agent.SetDestination(target.transform.position);

        StartCoroutine("WorkingRoutine");

        return true;
    }

    public override bool PostPerform()
    {
        if (!breaktest)
        {
            Debug.Log("testbreak");
            return false;
        }

        Debug.Log("workisdone");
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

        targetTag = "WI_Light";
        GameObject newTarget = GameObject.FindGameObjectWithTag("WI_Light");

        if (newTarget == null)
        {
            Debug.LogWarning("No target found with tag WI_Light.");
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

        targetTag = "LightStore";

        GameObject finalTarget = GameObject.FindGameObjectWithTag("LightStore");

        if (finalTarget == null)
        {
            Debug.LogWarning("No target found with tag LightStore.");
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
            targetBuilding.IncreaseLightAmount();
        }

        targetBuilding.isAvailable = true;
        breaktest = true;
    }
}
