using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_Transport : Actions
{
    private Building_Storage storage;
    private GOD god;

    private GameObject devilResource;

    private Devil devilScript;

    public override bool PrePerform()
    {
        devilScript = GetComponent<Devil>();

        if (devilScript == null)
        {
            Debug.LogWarning("Angel script not found on this GameObject.");
        }

        devilResource = devilScript.fireObject;

        if (devilResource == null)
        {
            Debug.LogWarning("No LIGHT resource found to transport.");
            return false;
        }

        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);
        if (buildings.Length == 0)
        {
            Debug.LogWarning("No building found with target Tag Store");
            return false;
        }

        GameObject closestBuilding = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject build in buildings)
        {
            if (targetTag == "Store")
            {
                Building_Storage storageScript = build.GetComponentInParent<Building_Storage>();
                float distance = Vector3.Distance(this.transform.position, build.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBuilding = build;
                }
            }

            if (targetTag == "GOD_WO")
            {
                GOD GODScript = build.GetComponentInParent<GOD>();
                float distance = Vector3.Distance(this.transform.position, build.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBuilding = build;
                }
            }
        }

        if (closestBuilding == null)
        {
            Debug.LogWarning("No valid building found for transport.");
            return false;
        }


        target = closestBuilding;
        agent.SetDestination(target.transform.position);

        if (targetTag == "Store")
        {
            storage = closestBuilding.GetComponentInParent<Building_Storage>();
            if (storage == null)
            {
                Debug.LogWarning("Building_Storage script missing on the closest building.");
                return false;
            }
        }

        if (targetTag == "GOD_WO")
        {
            god = closestBuilding.GetComponentInParent<GOD>();
            if (god == null)
            {
                Debug.LogWarning("GOD script missing on the closest building.");
                return false;
            }
        }

        return true;
    }

    public override bool PostPerform()
    {
        if (devilResource != null)
        {
            devilResource.SetActive(false);
        }

        if (targetTag == "Store")
        {
            if (storage != null)
            {
                storage.IncreaseFireCounter();
            }
        }

        if (targetTag == "GOD_WO")
        {
            god.IncreaseFireRessource();
        }


        return true;
    }

}

