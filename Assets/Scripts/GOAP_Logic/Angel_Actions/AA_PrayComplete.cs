using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AA_PrayComplete : Actions
{
    private Angel angel;

    private Building_PrayStatue targetBuilding;

    public void Start()
    {
        angel = GetComponent<Angel>();
    }

    public override bool PrePerform()
    {
        Dictionary<string, int> relevantState = GetRelevantAngelState();

        if (relevantState.ContainsKey("pray"))
        {
            int prayValue = relevantState["pray"];

            if (prayValue >= 1)
            {
                Debug.Log("Key 'pray' has value " + prayValue + " Action will be skipped.");
                ApplyAngelEffects();
                return false;
            }
        }
        else
        {
            Debug.Log("PrePerform Check in Pray: Key 'pray' does not exist.");
        }

        GameObject[] prayBuildings = GameObject.FindGameObjectsWithTag("WO_Pray");
        if (prayBuildings.Length == 0)
        {
            Debug.LogWarning("No available Building_PrayStatue found.");
            return false;
        }

        GameObject chosenBuilding = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject building in prayBuildings)
        {
            Building_PrayStatue buildingScript = building.GetComponentInParent<Building_PrayStatue>();

            if (buildingScript != null && buildingScript.isAvailable)
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    chosenBuilding = building;
                    targetBuilding = buildingScript;
                }
            }
        }
        if (chosenBuilding == null)
        {
            Debug.Log("No available Building_PrayStatue is available.");
            return false;
        }

        GameObject freeWaypoint = targetBuilding.GetFreeWaypoint();
        if (freeWaypoint == null)
        {
            Debug.Log("No free waypoint available in the Building_PrayStatue.");
            return false;
        }

        target = freeWaypoint;
        targetTag = "WO_Pray"; 
        agent.SetDestination(target.transform.position);

        StartCoroutine("PrayRoutine");

        duration = 35f;
        return true;
    }

    public override bool PostPerform()
    {
        ApplyAngelEffects();
        return true;
    }

    private IEnumerator PrayRoutine()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        angel.isBelieve = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(15f);

        angel.isBelieve = false;
        agent.isStopped = false;
    }
}

