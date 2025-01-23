using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_CleanAction : Actions
{
    private Angel angelScript;
    private Building_Shower buildingShower;
    private Building_Light buildingLight;

    public bool foundBuilding = false;
    public bool doneShower = false;
    public bool doneWork = false;

    private void Start()
    {
        if (targetTag == "WO_Light")
        {

        }

        angelScript = GetComponent<Angel>();

        if (angelScript == null)
        {
            Debug.LogWarning("Angel script not found on this GameObject.");
        }

        if (targetTag == "WO_Shower")
        {
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

        if (targetTag == "WO_Light")
        {
            GameObject lightParent = GameObject.FindWithTag("LIGHT");

            if (lightParent != null)
            {
                buildingLight = lightParent.GetComponentInParent<Building_Light>();
            }

            if (buildingLight == null)
            {
                Debug.LogWarning("Building_Light script not found on LightBuilding.");
            }
        }
    }

    public override bool PrePerform()
    {
        if (targetTag == "WO_Light")
        {
            Dictionary<string, int> relevantState = GetRelevantAngelState();

            if (relevantState.ContainsKey("cleanShower"))
            {
                int evilValue = relevantState["cleanShower"];

                if (evilValue <= 1)
                {
                    Debug.Log("Key 'cleanShower' has value 1. Action will be skipped.");
                    doneShower = true;
                    ApplyAngelEffects();
                    return false;
                }
            }
            else
            {
                Debug.Log("PrePerform Check: Key 'cleanShower' does not exist.");
                return false;
            }
        }

        agent.isStopped = true;

        if (targetTag == "WO_Shower")
        {
            StartCoroutine(WaitBeforeActionShower());
        }

        if (targetTag == "WO_Light")
        {
            StartCoroutine(WaitBeforeActionLight());
        }

        return false;
    }

    public override bool PostPerform()
    {
        if (angelScript != null)
        {
            angelScript.available = true;
            angelScript.isStunned = false;
        }

        if (targetTag == "WO_Shower")
        {
            doneShower = true;
            ApplyAngelEffects();
        }

        return true;
    }

    private IEnumerator WaitBeforeActionShower() 
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

        buildingShower = closestBuilding.GetComponentInParent<Building_Shower>();
        if (buildingShower == null)
        {
            Debug.LogWarning("Building_Shower script not found on the closest building.");
            yield return false;
        }

        buildingShower.OpenDoorAnimation();

        yield return new WaitForSeconds(4);

        agent.isStopped = false;
        yield return new WaitForSeconds(2);

        buildingShower.CloseDoorAnimation();
        buildingShower.isAvailable = true;

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

        buildingLight = closestBuilding.GetComponentInParent<Building_Light>();
        if (buildingLight == null)
        {
            Debug.LogWarning("Building_Light script not found on the closest building.");
            yield return false;
        }

        buildingLight.IncreaseLightAmount();

        yield return new WaitForSeconds(4);

        agent.isStopped = false;

        yield return new WaitForSeconds(2);

        buildingLight.angelInside = false;
    }
}
