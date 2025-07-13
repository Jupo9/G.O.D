using System.Collections;
using UnityEngine;

public class DA_Heat : Actions
{
    [Header("Heat Settings")]
    [SerializeField] private float heatTime = 3f;
    [SerializeField] private float heatIncreasePerSecond = 10f;

    private Building_FireCharge bbq;

    public override bool PrePerform()
    {
        Devil devilScript = agentScriptReference as Devil;

        if (devilScript != null && devilScript.heat >= 0.8f)
        {
            //Debug.Log("heat is enough, jump to next action");
            FinishAction();
            return false;
        }

        target = GetBuildingTarget();

        if (target == null)
        {
            //Debug.LogWarning("No valid fire Charge found.");
            return false;
        }

        bbq = target.GetComponent<Building_FireCharge>();

        if (bbq != null)
        {
            bbq.isAvailable = false;
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

        if (bbq != null)
        {
            bbq.BuildingHeatEvents(true);
        }

        Coroutine regenRoutine = StartCoroutine(IncreaseHeatOverTime(heatTime));

        yield return new WaitForSeconds(heatTime);

        if (regenRoutine != null)
        {
            StopCoroutine(regenRoutine);
        }

        if (bbq != null)
        {
            bbq.BuildingHeatEvents(false);
            bbq.isAvailable = true;
        }

        FinishAction();
    }

    private IEnumerator IncreaseHeatOverTime(float duration)
    {
        float timer = 0f;
        Devil devilScript = agentScriptReference as Devil;

        if (devilScript == null)
        {
            //Debug.LogWarning("DA_Heat: agentScriptReference is not an Devil.");
            yield break;
        }

        while (timer < duration)
        {
            devilScript.heat += heatIncreasePerSecond * Time.deltaTime;
            devilScript.heat = Mathf.Clamp01(devilScript.heat);

            timer += Time.deltaTime;
            yield return null;
        }
    }

    public override bool PostPerform()
    {
        //Debug.Log("Finishedd Action: " + actionName);

        return true;
    }
}
