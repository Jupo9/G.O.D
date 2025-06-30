using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_Summon : Actions
{
    [Header("Summon Settings")]
    [SerializeField] private float summonTime = 3f;

    private Building_Pentagram summon;

    public override bool PrePerform()
    {
        target = GetBuildingTarget();

        if (target == null)
        {
            Debug.LogWarning("No valid Pentagram found.");
            return false;
        }

        summon = target.GetComponent<Building_Pentagram>();

        if (summon != null)
        {
            summon.isAvailable = false;
        }

        agent.SetDestination(target.transform.position);
        StartCoroutine(SummonRoutine());

        return true;
    }

    private GameObject GetBuildingTarget()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);

        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var building in buildings)
        {
            var ps = building.GetComponentInChildren<Building_Pentagram>();
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

    private IEnumerator SummonRoutine()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        agent.isStopped = true;

        yield return new WaitForSeconds(summonTime);

        agent.isStopped = false;

        if (summon != null)
        {
            summon.isAvailable = true;
        }

        running = false;
    }

    public override bool PostPerform()
    {
        Debug.Log("Finished Action: " + actionName);
        return true;
    }
}
