using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_ShowerComplete : Actions
{
    private Angel angel;

    private Building_Shower targetBuilding;

    public void Start()
    {
        angel = GetComponent<Angel>();
    }

    public override bool PrePerform()
    {
        Dictionary<string, int> relevantState = GetRelevantAngelState();

        if (relevantState.ContainsKey("shower"))
        {
            int showerValue = relevantState["shower"];

            if (showerValue >= 1)
            {
                Debug.Log("Key 'shower' has value " + showerValue + " Action will be skipped.");
                ApplyAngelEffects();
                return false;
            }
        }
        else
        {
            Debug.Log("PrePerform Check in Shower: Key 'shower' does not exist.");
        }

        GameObject[] showerBuildings = GameObject.FindGameObjectsWithTag("WO_Shower");

        if (showerBuildings.Length == 0)
        {
            Debug.LogWarning("No available Building_Shower found.");
            return false;
        }

        GameObject closestBuilding = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject building in showerBuildings)
        {
            Building_Shower showerScript = building.GetComponentInParent<Building_Shower>();

            if (showerScript != null && showerScript.isAvailable)
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBuilding = building;
                    targetBuilding = showerScript;
                    targetBuilding.isAvailable = false;
                }
            }
        }


        if (closestBuilding == null)
        {
            Debug.Log("No Building_Shower is available.");
            return false;
        }

        target = closestBuilding;
        targetBuilding.isAvailable = false;
        targetTag = "WO_Shower";

        agent.SetDestination(target.transform.position);

        StartCoroutine("ShowerRoutine");

        return true;
    }

    public override bool PostPerform()
    {
        ApplyAngelEffects();
        return true;
    }

    private IEnumerator ShowerRoutine()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        agent.isStopped = true;

        if (targetBuilding != null)
        {
            targetBuilding.OpenDoorAnimation();
        }

        yield return new WaitForSeconds(5f);
        agent.isStopped = false;

        targetTag = "WI_Shower";
        GameObject newTarget = FindClosestAvailableBuildingInside();

        if (newTarget == null)
        {
            Debug.LogWarning("No target found with tag WI_Shower.");
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
            targetBuilding.CloseDoorAnimation();
        }

        if (targetBuilding != null)
        {
            targetBuilding.StartSteam();
        }

        //angel.isPurity = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(10f);

        if (targetBuilding != null)
        {
            targetBuilding.OpenDoorAnimation();
        }

        if (targetBuilding != null)
        {
            targetBuilding.StopSteam();
        }

        yield return new WaitForSeconds(5f);

        //angel.isPurity = false;
        agent.isStopped = false;

        targetTag = "WO_Shower";
        GameObject finalTarget = FindClosestAvailableBuilding();

        if (finalTarget == null)
        {
            Debug.LogWarning("No target found with tag WO_Shower.");
            yield break;
        }

        if (targetBuilding != null)
        {
            targetBuilding.CloseDoorAnimation();
        }

        target = finalTarget;
        agent.SetDestination(target.transform.position);

        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        targetBuilding.isAvailable = true;
    }

    private GameObject FindClosestAvailableBuilding()
    {
        GameObject[] showerBuildings = GameObject.FindGameObjectsWithTag("WO_Shower");

        GameObject closestBuilding = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject building in showerBuildings)
        {
            Building_Shower showerScript = building.GetComponentInParent<Building_Shower>();

            if (showerScript != null)
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBuilding = building;
                    targetBuilding = showerScript;
                    targetBuilding.isAvailable = false;
                }
            }
        }

        return closestBuilding;
    }

    private GameObject FindClosestAvailableBuildingInside()
    {
        GameObject[] showerBuildings = GameObject.FindGameObjectsWithTag("WI_Shower");

        GameObject closestBuilding = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject building in showerBuildings)
        {
            Building_Shower showerScript = building.GetComponentInParent<Building_Shower>();

            if (showerScript != null)
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBuilding = building;
                    targetBuilding = showerScript;
                    targetBuilding.isAvailable = false;
                }
            }
        }

        return closestBuilding;
    }
}
