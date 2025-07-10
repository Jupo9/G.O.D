using System.Collections;
using UnityEngine;

public class DA_Summon : Actions
{
    [Header("Summon Settings")]
    [SerializeField] private float summonTime = 3f;
    [SerializeField] private float summonIncreasePerSecond = 10f;

    private Building_Pentagram pentagram;
    private GameObject currentWaypoint;
    private int waypointIndex = -1;

    public override bool PrePerform()
    {
        Devil devilScript = agentScriptReference as Devil;

        if (devilScript != null && devilScript.summon >= 0.8f)
        {
            Debug.Log("summon is enough, jump to next action");
            FinishAction();
            return false;
        }

        pentagram = FindClosestValidPentagram();

        if (pentagram == null)
        {
            Debug.LogWarning("No valid pentagram found.");
            return false;
        }

        currentWaypoint = GetAndReserveFreeWaypoint(pentagram, out waypointIndex);

        target = currentWaypoint;

        agent.SetDestination(currentWaypoint.transform.position);
        StartCoroutine(SummonRoutine());

        return true;
    }

    private Building_Pentagram FindClosestValidPentagram()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);

        Building_Pentagram closestStatue = null;
        float minDist = float.MaxValue;

        foreach (var building in buildings)
        {
            var sign = building.GetComponentInChildren<Building_Pentagram>();
            if (sign != null && sign.isAvailable)
            {
                float dist = Vector3.Distance(transform.position, building.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestStatue = sign;
                }
            }
        }

        return closestStatue;
    }

    private GameObject GetAndReserveFreeWaypoint(Building_Pentagram statue, out int index)
    {
        for (int i = 0; i < statue.waypoints.Length; i++)
        {
            if (statue.waypoints[i].open && statue.waypoints[i].waypointObject != null)
            {
                statue.waypoints[i].open = false;
                statue.CheckAvailability();
                index = i;
                return statue.waypoints[i].waypointObject;
            }
        }

        index = -1;
        return null;
    }

    private IEnumerator SummonRoutine()
    {
        while (Vector3.Distance(transform.position, currentWaypoint.transform.position) > 1.1f)
        {
            yield return null;
        }

        Coroutine regenRoutine = StartCoroutine(IncreaseSummonOverTime(summonTime));

        yield return new WaitForSeconds(summonTime);

        if (regenRoutine != null)
        {
            StopCoroutine(regenRoutine);
        }

        if (pentagram != null && waypointIndex != -1)
        {
            pentagram.SetWaypointState(waypointIndex, true);
        }

        FinishAction();
    }

    private IEnumerator IncreaseSummonOverTime(float duration)
    {
        float timer = 0f;
        Devil devilScript = agentScriptReference as Devil;

        if (devilScript == null)
        {
            Debug.LogWarning("DA_Summon: agentScriptReference is not an Devil.");
            yield break;
        }

        while (timer < duration)
        {
            devilScript.summon += summonIncreasePerSecond * Time.deltaTime;
            devilScript.summon = Mathf.Clamp01(devilScript.summon);

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
