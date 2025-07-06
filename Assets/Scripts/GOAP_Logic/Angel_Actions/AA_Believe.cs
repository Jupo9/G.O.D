using System.Collections;
using UnityEngine;

public class AA_Believe : Actions
{
    [Header("Belief Settings")]
    [SerializeField] private float believeTime = 3f;

    private Building_PrayStatue prayStatue;

    public override bool PrePerform()
    {
        target = GetBuildingTarget();

        if (target == null)
        {
            Debug.LogWarning("No valid pray statue found.");
            return false;
        }

        prayStatue = target.GetComponent<Building_PrayStatue>();

        if (prayStatue != null)
        {
            prayStatue.isAvailable = false;
        }

        agent.SetDestination(target.transform.position);
        StartCoroutine(BelieveRoutine());

        return true;
    }

    private GameObject GetBuildingTarget()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);

        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var building in buildings)
        {
            var ps = building.GetComponentInChildren<Building_PrayStatue>();
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

    private IEnumerator BelieveRoutine()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        agent.isStopped = true;

        yield return new WaitForSeconds(believeTime);

        agent.isStopped = false;

        if (prayStatue != null)
        {
            prayStatue.isAvailable = true;
        }

        FinishAction();
    }

    public override bool PostPerform()
    {
        Debug.Log("Finished Action: " + actionName);
        return true;
    }
}
