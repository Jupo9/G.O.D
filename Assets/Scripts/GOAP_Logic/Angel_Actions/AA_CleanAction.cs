using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AA_CleanAction : Actions
{
    private Angel angelScript;
    private Building_Shower buildingShower;
    private Building_Light buildingLight;

    public bool done = false;

    private void Start()
    {
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
            Worlds.Instance.GetWorld().SetState("cleanShower", 1);
            Debug.Log("cleanShower wurde zu WorldStates hinzugefügt.");
            done = true;
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
