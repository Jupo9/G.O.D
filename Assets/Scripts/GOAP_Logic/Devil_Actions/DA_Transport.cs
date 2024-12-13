using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_Transport : Actions
{
    private Building_Light buildingLight;
    private Building_Storage storage;

    private bool wantTransport = false;

    private void Start()
    {
        GameObject lightParent = GameObject.FindWithTag("LIGHT");

        if (lightParent != null)
        {
            buildingLight = lightParent.GetComponentInChildren<Building_Light>();
        }

        if (buildingLight == null)
        {
            Debug.LogWarning("Building_Light script not found on LightBuilding.");
        }

        storage = GetComponentInChildren<Building_Storage>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Store") && wantTransport)
        {
            Debug.Log("hitStore");
            storage.DecreaseFireCounter();
        }
    }

    public override bool PrePerform()
    {
        wantTransport = true;

        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);
        if (buildings.Length == 0)
        {
            return false;
        }

        GameObject closestBuilding = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject build in buildings)
        {
            Building_Light buildingLightScript = build.GetComponentInParent<Building_Light>();
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

        buildingLight = closestBuilding.GetComponentInParent<Building_Light>();
        if (buildingLight == null)
        {
            Debug.LogWarning("Building_Light script not found on the closest building.");
            return false;
        }

        return true;
    }

    public override bool PostPerform()
    {
        wantTransport = false;
        return true;
    }
}
