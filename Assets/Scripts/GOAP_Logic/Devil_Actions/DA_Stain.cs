using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_Stain : Actions
{
    [Header("Stain Settings")]
    //[SerializeField] private float stainTime = 3f;

    private Building_Trap stained;

    public override bool PrePerform()
    {
        target = GetBuildingTarget();

        if (target == null)
        {
            Debug.LogWarning("No valid Trap found.");
            return false;
        }

        stained = target.GetComponent<Building_Trap>();

        if (stained != null)
        {
            stained.isAvailable = false;
        }

        agent.SetDestination(target.transform.position);
        StartCoroutine(StainRoutine());

        return true;
    }

    private GameObject GetBuildingTarget()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);

        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var building in buildings)
        {
            var ps = building.GetComponentInChildren<Building_Trap>();
            if (ps != null && ps.isAvailable)
            {
                float dist = Vector3.Distance(transform.position, building.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = building;
                }
            }
        }

        return closest;
    }

    private IEnumerator StainRoutine()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        agent.isStopped = true;


        yield return new WaitForSeconds(duration);

        agent.isStopped = false;

        if (stained != null)
        {
            stained.isAvailable = true;
        }

        running = false;
    }

    public override bool PostPerform()
    {
        Debug.Log("Finished Action: " + actionName);
        return true;
    }
}
