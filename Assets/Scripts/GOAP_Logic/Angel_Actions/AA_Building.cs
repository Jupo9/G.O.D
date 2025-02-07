using UnityEngine;

public class AA_Building : Actions
{
    private ChangeBuildState changeBuildState;

    public override bool PrePerform()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);
        if (buildings.Length == 0)
        {
            return false;
        }

        GameObject closestBuilding = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject build in buildings)
        {
            ChangeBuildState changeBuildStateScript = build.GetComponentInChildren<ChangeBuildState>();
            if (changeBuildStateScript != null && !changeBuildStateScript.buildingIsComplete)
            {
                float distance = Vector3.Distance(this.transform.position, build.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBuilding = build;
                }
            }
            else
            {
                Debug.Log("No building found");
            }
        }

        if (closestBuilding == null)
        {
            return false;
        }

        target = closestBuilding;
        agent.SetDestination(target.transform.position);

        duration = 30;

        changeBuildState = closestBuilding.GetComponentInChildren<ChangeBuildState>();
        if (changeBuildState == null)
        {
            Debug.LogWarning("changeBuildState script not found on the closest building.");
            return false;
        }

        changeBuildState.buildingIsComplete = true;

        Debug.Log($"Building '{closestBuilding.name}' aktivate");


        return true;
    }

    public override bool PostPerform()
    {
        changeBuildState.changeBuildingState = true;
        Debug.Log("changeBuildingState set to true.");
        Debug.Log("Finished Building");
        return true;
    }
}
