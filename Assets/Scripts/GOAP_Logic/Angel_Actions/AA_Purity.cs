using System.Collections;
using UnityEngine;

public class AA_Purity : Actions
{
    [Header("Purity Settings")]
    [SerializeField] private float purityTime = 3f;
    [SerializeField] private float purityIncreasePerSecond = 10f;

    private Building_Waterfall waterFall;

    public override bool PrePerform()
    {
        Angel angelScript = agentScriptReference as Angel;

        if (angelScript != null && angelScript.purity >= 0.8f)
        {
            Debug.Log("purity is enough, jump to next action");
            FinishAction();
            return false;
        }

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

        Coroutine regenRoutine = StartCoroutine(IncreasePurityOverTime(purityTime));

        yield return new WaitForSeconds(purityTime);

        if (regenRoutine != null)
        {
            StopCoroutine(regenRoutine);
        }

        if (waterFall != null)
        {
            waterFall.isAvailable = true;
        }

        FinishAction();
    }

    private IEnumerator IncreasePurityOverTime(float duration)
    {
        float timer = 0f;
        Angel angelScript = agentScriptReference as Angel;

        if (angelScript == null)
        {
            Debug.LogWarning("AA_Purity: agentScriptReference is not an Angel.");
            yield break;
        }

        while (timer < duration)
        {
            angelScript.purity += purityIncreasePerSecond * Time.deltaTime;
            angelScript.purity = Mathf.Clamp01(angelScript.purity);

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
