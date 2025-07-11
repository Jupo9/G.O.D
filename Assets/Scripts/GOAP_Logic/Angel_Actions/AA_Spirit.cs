using System.Collections;
using UnityEngine;

public class AA_Spirit : Actions
{
    [Header("Spirit Settings")]
    [SerializeField] private float spiritTime = 3f;
    [SerializeField] private float spiritIncreasePerSecond = 10f;

    private Building_LightCharge lightCharge;

    public override bool PrePerform()
    {
        Angel angelScript = agentScriptReference as Angel;

        if (angelScript != null && angelScript.spirit >= 0.8f)
        {
            Debug.Log("spirit is enough, jump to next action");
            FinishAction();
            return false;
        }

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

        if (angelScript != null)
        {
            angelScript.isAvailable = false; 
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

        if (lightCharge != null)
        {
            lightCharge.BuildingSpiritEvents(true);
        }

        Coroutine regenRoutine = StartCoroutine(IncreaseSpiritOverTime(spiritTime));

        yield return new WaitForSeconds(spiritTime);

        if (regenRoutine != null)
        {
            StopCoroutine(regenRoutine);
        }

        if (lightCharge != null)
        {
            lightCharge.BuildingSpiritEvents(false);
            lightCharge.isAvailable = true;
        }

        FinishAction();
    }

    private IEnumerator IncreaseSpiritOverTime(float duration)
    {
        float timer = 0f;
        Angel angelScript = agentScriptReference as Angel;

        if (angelScript == null)
        {
            Debug.LogWarning("AA_Spirit: agentScriptReference is not an Angel.");
            yield break;
        }

        while (timer < duration)
        {
            angelScript.spirit += spiritIncreasePerSecond * Time.deltaTime;
            angelScript.spirit = Mathf.Clamp01(angelScript.spirit);

            timer += Time.deltaTime;
            yield return null;
        }
    }

    public override bool PostPerform()
    {
        Angel angelScript = agentScriptReference as Angel;

        if (angelScript != null)
        {
            angelScript.isAvailable = true; 
        }

        Debug.Log("Finished Action: " + actionName);
        return true;
    }
}
