using UnityEngine;

public class DA_TakeFire : Actions
{
    private Building_Fire fireBuilding;
    private Building_Storage storage;

    private GameObject fireResource;

    private Devil devilScript;

    public override bool PrePerform()
    {
        devilScript = GetComponent<Devil>();

        if (devilScript == null)
        {
            Debug.LogWarning("Angel script not found on this GameObject.");
        }

        fireResource = devilScript.fireObject;

        if (fireResource == null)
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
            if (targetTag == "FireStore")
            {
                Building_Fire fireScript = build.GetComponentInParent<Building_Fire>();

                if (fireScript != null && fireScript.fireAmount > 0 && !fireScript.calculate)
                {
                    float distance = Vector3.Distance(this.transform.position, build.transform.position);

                    fireScript.calculate = true;

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

                if (storageScript != null && storageScript.fireCounter > 0 && !storageScript.calculate)
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

        if (targetTag == "FireStore")
        {
            fireBuilding = closestBuilding.GetComponentInParent<Building_Fire>();
            if (fireBuilding == null)
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
        fireResource.SetActive(true);


        if (targetTag == "FireStore")
        {
            if (fireBuilding != null)
            {
                fireBuilding.DecreaseFireAmount();
                fireBuilding.calculate = false;
            }
        }

        if (targetTag == "Store")
        {
            if (storage != null)
            {
                storage.DecreaseFireCounter();
                storage.calculate = false;
            }
        }

        return true;
    }

}
