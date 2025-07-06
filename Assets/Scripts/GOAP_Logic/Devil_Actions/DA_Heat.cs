using System.Collections;
using UnityEngine;

public class DA_Heat : Actions
{
    [Header("Heat Settings")]
    [SerializeField] private float heatTime = 3f;

    private Building_FireCharge heat;

    public override bool PrePerform()
    {

        target = GetBuildingTarget();

        if (target == null)
        {
            Debug.LogWarning("No valid fire Charge found.");
            return false;
        }

        heat = target.GetComponent<Building_FireCharge>();

        if (heat != null)
        {
            heat.isAvailable = false;
        }

        agent.SetDestination(target.transform.position);
        StartCoroutine(HeatRoutine());

        return true;
    }

    private GameObject GetBuildingTarget()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);

        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var building in buildings)
        {
            var ps = building.GetComponentInChildren<Building_FireCharge>();
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

    private IEnumerator HeatRoutine()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        agent.isStopped = true;

        yield return new WaitForSeconds(heatTime);

        agent.isStopped = false;

        if (heat != null)
        {
            heat.isAvailable = true;
        }

        FinishAction();
    }

    public override bool PostPerform()
    {
        Debug.Log("Finishedd Action: " + actionName);

        return true;
    }
}
