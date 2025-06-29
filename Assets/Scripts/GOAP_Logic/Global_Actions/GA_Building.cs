using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GA_Building : Actions
{
    private Construction constructionTarget;

    [SerializeField] private float buildTime = 2.5f;

    public override bool PrePerform()
    {
        constructionTarget = BuildingTaskManager.Instance.DequeueNextTask(GetComponent<Agents>().unitType);

        if (constructionTarget == null)
        {
            Debug.LogWarning("No Building found in Queue.");
            return false;
        }

        target = constructionTarget.gameObject;

        StartCoroutine(BuildRoutine());

        return true;
    }

    private IEnumerator BuildRoutine()
    {

        while (Vector3.Distance(transform.position, constructionTarget.transform.position) > 1.2f)
        {
            yield return null;
        }

        agent.isStopped = true;

        yield return new WaitForSeconds(buildTime);

        FinalizeConstruction();
    }

    private void FinalizeConstruction()
    {
        if (constructionTarget == null)
        {
            return;
        }

        Instantiate(constructionTarget.previewData.buildingPrefab,
                    constructionTarget.transform.position,
                    constructionTarget.transform.rotation);

        Destroy(constructionTarget.gameObject);
        Debug.Log("Building was succesful placed");

        agent.isStopped = false;
    }

    public void SetConstructionTarget(Construction target)
    {
        constructionTarget = target;
    }

    public override bool PostPerform()
    {
        Agents agentScript = GetComponent<Agents>();

        if (agentScript.unitType == Agents.UnitType.Devil)
        {
            Debug.Log("[GA_Building] Devil finished building and will die.");
            agent.GetComponent<Agents>()?.Die();
        }

        return true;
    }
}
