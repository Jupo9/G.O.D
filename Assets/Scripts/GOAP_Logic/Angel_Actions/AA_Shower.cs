using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_Shower : Actions
{
    private Building_Shower buildingShower;

    private Angel angel;

    public bool done = false;

    private void Start()
    {
        angel = GetComponent<Angel>();

        GameObject showerParent = GameObject.FindWithTag("Shower");

        if (showerParent != null)
        {
            buildingShower = showerParent.GetComponentInChildren<Building_Shower>();
        }

        if (buildingShower == null)
        {
            Debug.LogWarning("Building_Shower script not found on ShowerBuilding.");
        }
    }

    public override bool PrePerform()
    {
        Dictionary<string, int> relevantState = GetRelevantAngelState();

        if (relevantState.ContainsKey("cleanShower"))
        {
            int evilValue = relevantState["cleanShower"];

            if (evilValue <= 1)
            {
                Debug.Log("Key 'cleanShower' has value 1. Action will be skipped.");
                done = true;
                ApplyAngelEffects();
                return false;
            }
        }
        else
        {
            Debug.Log("PrePerform Check: Key 'cleanShower' does not exist.");
            return false;
        }

        Invoke("CloseDoor", 2f);

        angel.isPurity = true;

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

        buildingShower = closestBuilding.GetComponentInParent<Building_Shower>();
        if (buildingShower == null)
        {
            Debug.LogWarning("Building_Shower script not found on the closest building.");
            return false;
        }

        buildingShower.isAvailable = false;

        return true;
    }

    private void CloseDoor()
    {
        buildingShower.CloseDoorAnimation();
        buildingShower.StartSteam();
    }

    public override bool PostPerform()
    {
        buildingShower.StopSteam();
        ApplyAngelEffects();
        angel.isPurity = false;
        done = true;
        return true;
    }

}
