using System.Collections;
using UnityEngine;

public class AA_Purity : Actions
{
    [Header("Purity Settings")]
    [SerializeField] private float purityTime = 3f;

    private Building_Waterfall waterFall;

    public override bool PrePerform()
    {
        target = GetBuildingTarget();

        if (target == null)
        {
            Debug.LogWarning("No valid Waterfall found.");
            return false;
        }

        waterFall = target.GetComponent<Building_Waterfall>();

        if (waterFall != null)
        {
            waterFall.isAvailable = false;
        }

        agent.SetDestination(target.transform.position);
        StartCoroutine(PurityRoutine());

        return true;
    }

    private GameObject GetBuildingTarget()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);

        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var building in buildings)
        {
            var ps = building.GetComponentInChildren<Building_Waterfall>();
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

    private IEnumerator PurityRoutine()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        agent.isStopped = true;

        yield return new WaitForSeconds(purityTime);

        agent.isStopped = false;

        if (waterFall != null)
        {
            waterFall.isAvailable = true;
        }

        FinishAction();
    }

    public override bool PostPerform()
    {
        Debug.Log("Finished Action: " + actionName);
        return true;
    }
}
