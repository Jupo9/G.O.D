using System.Collections;
using UnityEngine;

public class DA_Stain : Actions
{
    [Header("Stain Settings")]
    [SerializeField] private float stainTime = 3f;
    [SerializeField] private float stainIncreasePerSecond = 10f;

    private Building_Trap stained;

    public override bool PrePerform()
    {
        Devil devilScript = agentScriptReference as Devil;

        if (devilScript != null && devilScript.stain >= 0.8f)
        {
            Debug.Log("stain is enough, jump to next action");
            FinishAction();
            return false;
        }

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

        if (stained != null)
        {
            stained.BuildingStainEvents(true);
        }

        Coroutine regenRoutine = StartCoroutine(IncreaseStainOverTime(stainTime));

        yield return new WaitForSeconds(stainTime);

        if (regenRoutine != null)
        {
            StopCoroutine(regenRoutine);
        }

        if (stained != null)
        {
            stained.BuildingStainEvents(false);
            stained.isAvailable = true;
        }

        FinishAction();
    }

    private IEnumerator IncreaseStainOverTime(float duration)
    {
        float timer = 0f;
        Devil devilScript = agentScriptReference as Devil;

        if (devilScript == null)
        {
            Debug.LogWarning("DA_Stain: agentScriptReference is not an Devil.");
            yield break;
        }

        while (timer < duration)
        {
            devilScript.stain += stainIncreasePerSecond * Time.deltaTime;
            devilScript.stain = Mathf.Clamp01(devilScript.stain);

            timer += Time.deltaTime;
            yield return null;
        }
    }

    public override bool PostPerform()
    {
        Debug.Log("Finished Action: " + actionName);
        return true;
    }
}
