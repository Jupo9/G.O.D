using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_TakeLight : Actions
{
    private Building_Light lightBuilding;
    private Building_Storage storage;

    private GameObject lightResource;

    private Angel angelScript;

    public override bool PrePerform()
    {
        angelScript = GetComponent<Angel>();

        if (angelScript == null)
        {
            Debug.LogWarning("Angel script not found on this GameObject.");
        }

        lightResource = angelScript.lightResource;

        if (lightResource == null)
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
            if (targetTag == "LightStore")
            {
                Building_Light lightScript = build.GetComponentInParent<Building_Light>();

                if (lightScript != null && lightScript.lightAmount > 0 && !lightScript.calculate)
                {
                    float distance = Vector3.Distance(this.transform.position, build.transform.position);

                    lightScript.calculate = true;

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestBuilding = build;
                    }
                }
            }

            if (targetTag == "Store")
            {
                Building_Storage storageScript = build.GetComponentInParent<Building_Storage>();

                if (storageScript != null && storageScript.lightCounter > 0 && !storageScript.calculate)
                {
                    float distance = Vector3.Distance(this.transform.position, build.transform.position);

                    storageScript.calculate = true;

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestBuilding = build;
                    }
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

        if (targetTag == "LightStore")
        {
            lightBuilding = closestBuilding.GetComponentInParent<Building_Light>();
            if (lightBuilding == null)
            {
                Debug.LogWarning("Building_Storage script missing on the closest building.");
                return false;
            }
        }

        if (targetTag == "Store")
        {
            storage = closestBuilding.GetComponentInParent<Building_Storage>();
            if (storage == null)
            {
                Debug.LogWarning("Building_Storage script missing on the closest building.");
                return false;
            }
        }

        return true;
    }

    public override bool PostPerform()
    {
        lightResource.SetActive(true);


        if (targetTag == "LightStore")
        {
            if (lightBuilding != null)
            {
                lightBuilding.DecreaseLightAmount();
                lightBuilding.calculate = false;
            }
        }

        if (targetTag == "Store")
        {
            if (storage != null)
            {
                storage.DecreaseLightCounter();
                storage.calculate = false;
            }
        }

        return true;
    }

}
