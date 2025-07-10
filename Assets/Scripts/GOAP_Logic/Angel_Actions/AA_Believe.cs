using System.Collections;
using UnityEngine;

public class AA_Believe : Actions
{
    [Header("Belief Settings")]
    [SerializeField] private float believeTime = 3f;
    [SerializeField] private float believeIncreasePerSecond = 10f;

    private Building_PrayStatue prayStatue;
    private GameObject currentWaypoint;
    private int waypointIndex = -1;

    public override bool PrePerform()
    {
        Angel angelScript = agentScriptReference as Angel;

        if (angelScript != null && angelScript.believe >= 0.8f)
        {
            Debug.Log("believe is enough, jump to next action");
            FinishAction();
            return false; 
        }

        prayStatue = FindClosestValidStatue();

        if (prayStatue == null)
        {
            Debug.LogWarning("No valid pray statue found.");
            return false;
        }

        currentWaypoint = GetAndReserveFreeWaypoint(prayStatue, out waypointIndex);

        target = currentWaypoint;

        agent.SetDestination(currentWaypoint.transform.position);
        StartCoroutine(BelieveRoutine());

        return true;
    }

    private Building_PrayStatue FindClosestValidStatue()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);

        Building_PrayStatue closestStatue = null;
        float minDist = float.MaxValue;

        foreach (var building in buildings)
        {
            var statue = building.GetComponentInChildren<Building_PrayStatue>();
            if (statue != null && statue.isAvailable)
            {
                float dist = Vector3.Distance(transform.position, building.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestStatue = statue;
                }
            }
        }

        return closestStatue;
    }

    private GameObject GetAndReserveFreeWaypoint(Building_PrayStatue statue, out int index)
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

    private IEnumerator BelieveRoutine()
    {
        while (Vector3.Distance(transform.position, currentWaypoint.transform.position) > 1.1f)
        {
            yield return null;
        }

        Coroutine regenRoutine = StartCoroutine(IncreaseBelieveOverTime(believeTime));

        yield return new WaitForSeconds(believeTime);

        if (regenRoutine != null)
        {
            StopCoroutine(regenRoutine);
        }

        if (prayStatue != null && waypointIndex != -1)
        {
            prayStatue.SetWaypointState(waypointIndex, true);
        }

        FinishAction();
    }

    private IEnumerator IncreaseBelieveOverTime(float duration)
    {
        float timer = 0f;
        Angel angelScript = agentScriptReference as Angel;

        if (angelScript == null)
        {
            Debug.LogWarning("AA_Believe: agentScriptReference is not an Angel.");
            yield break;
        }

        while (timer < duration)
        {
            angelScript.believe += believeIncreasePerSecond * Time.deltaTime;
            angelScript.believe = Mathf.Clamp01(angelScript.believe);

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
