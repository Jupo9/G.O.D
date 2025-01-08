using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DA_ChillComplete : Actions
{
    private Building_IronMaiden buildingIronMaiden;
    [SerializeField] private Transform tempPosition;
    private NavMeshAgent newAgent;

    private enum ChillingState
    {
        None,
        Prepare,
        Chilling,
        Clean
    }

    private ChillingState currentState = ChillingState.None;

    private void Start()
    {
        newAgent = GetComponentInParent<NavMeshAgent>();
        if (newAgent == null)
        {
            Debug.LogError("NavMeshAgent not found on parent GameObject!");
        }

        if (targetTag == "WO_Iron")
        {
            GameObject ironParent = GameObject.FindWithTag("Iron");
            if (ironParent != null)
            {
                buildingIronMaiden = ironParent.GetComponentInChildren<Building_IronMaiden>();
            }
        }
    }

    public override bool PrePerform()
    {
        currentState = ChillingState.Prepare;
        StartCoroutine(HandleChillingAction());
        return true;
    }

    public override bool PostPerform()
    {
        currentState = ChillingState.None;
        return true;
    }

    private IEnumerator HandleChillingAction()
    {
        while (currentState != ChillingState.None)
        {
            switch (currentState)
            {
                case ChillingState.Prepare:
                    yield return PreparePhase();
                    currentState = ChillingState.Chilling; 
                    break;

                case ChillingState.Chilling:
                    yield return ChillingPhase();
                    currentState = ChillingState.Clean; 
                    break;

                case ChillingState.Clean:
                    yield return CleanPhase();
                    currentState = ChillingState.None; 
                    break;
            }
        }
    }

    private IEnumerator PreparePhase()
    {
        Debug.Log("Prepare Chilling phase started.");

        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);
        if (buildings.Length == 0)
        {
            Debug.LogWarning("No buildings found for the target tag.");
            yield break;
        }

        GameObject closestBuilding = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject build in buildings)
        {
            float distance = Vector3.Distance(transform.position, build.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBuilding = build;
            }
        }

        if (closestBuilding == null)
        {
            Debug.LogWarning("No suitable building found.");
            yield break;
        }

        target = closestBuilding;
        newAgent.SetDestination(target.transform.position);

        while (newAgent.pathPending || newAgent.remainingDistance > newAgent.stoppingDistance)
        {
            yield return null;
        }

        if (targetTag == "WO_Iron" && buildingIronMaiden != null)
        {
            buildingIronMaiden.isAvailable = false;
        }

        Debug.Log("Prepare phase completed.");
    }

    private IEnumerator ChillingPhase()
    {
        Debug.Log("Chilling phase started.");

        if (tempPosition == null)
        {
            Debug.LogError("TempPosition is not assigned in the Inspector!");
            yield break;
        }

        while (newAgent.pathPending || newAgent.remainingDistance > newAgent.stoppingDistance)
        {
            yield return null;
        }

        yield return new WaitForSeconds(5);

        if (targetTag == "WO_Iron" && buildingIronMaiden != null)
        {
            buildingIronMaiden.CloseDoubleDoors();
        }

        yield return new WaitForSeconds(5);

        Debug.Log("Chilling phase completed.");
    }

    private IEnumerator CleanPhase()
    {
        Debug.Log("Clean phase started.");

        yield return new WaitForSeconds(2);

        if (targetTag == "WO_Iron" && buildingIronMaiden != null)
        {
            buildingIronMaiden.OpenDoubleDoors();
            buildingIronMaiden.isAvailable = true;
        }

        Debug.Log("Clean phase completed.");
    }
}