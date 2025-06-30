using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_Spirit : Actions
{
    [Header("Spirit Settings")]
   // [SerializeField] private float spiritTime = 3f;

    private Building_LightCharge lightCharge;

    public override bool PrePerform()
    {
        target = GetBuildingTarget();

        if (target == null)
        {
            Debug.LogWarning("No valid light Charge found.");
            return false;
        }

        lightCharge = target.GetComponent<Building_LightCharge>();

        if (lightCharge != null)
        {
            lightCharge.isAvailable = false;
        }

        agent.SetDestination(target.transform.position);
        StartCoroutine(SpiritRoutine());

        return true;
    }

    private GameObject GetBuildingTarget()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);

        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var building in buildings)
        {
            var ps = building.GetComponentInChildren<Building_LightCharge>();
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

    private IEnumerator SpiritRoutine()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        agent.isStopped = true;

        yield return new WaitForSeconds(duration);

        agent.isStopped = false;

        if (lightCharge != null)
        {
            lightCharge.isAvailable = true;
        }

        running = false;
    }

    public override bool PostPerform()
    {
        Debug.Log("Finished Action: " + actionName);
        return true;
    }
}
