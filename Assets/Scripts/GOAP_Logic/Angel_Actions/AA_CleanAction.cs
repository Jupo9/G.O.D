using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_CleanAction : Actions
{
    private Angel angelScript;
    private Building_Shower buildingShower;

    private void Start()
    {
        angelScript = GetComponent<Angel>();

        if (angelScript == null)
        {
            Debug.LogWarning("Angel script not found on this GameObject.");
        }

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
        agent.isStopped = true;
        StartCoroutine(WaitBeforeAction());
        return false;
    }

    public override bool PostPerform()
    {
        if (angelScript != null)
        {
            angelScript.available = true;
            angelScript.isStunned = false;
        }

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
}
