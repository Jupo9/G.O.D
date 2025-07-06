using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GA_MoveAround : Actions
{
    [SerializeField] private float moveRadius = 25f;
    [SerializeField] private float minMovementTime = 5f;
    [SerializeField] private float maxMovementTime = 15f;

    [SerializeField] private LayerMask avoidZonesLayer;
    [SerializeField] private float avoidRadius = 5f;
    [SerializeField] private int maxTries = 10;


    private bool movingAround = false;
    private GameObject roamTarget;

    public override bool PrePerform()
    {
        movingAround = true;

        if (roamTarget == null)
        {
            roamTarget = new GameObject("RoamTarget");
        }

        target = roamTarget;

        StartCoroutine(RandomMove());

        return true;
    }

    public override bool PostPerform()
    {
        movingAround = false;

        if (agent != null)
        {
            agent.ResetPath();
        }

        return true;
    }

    private IEnumerator RandomMove()
    {
        while (movingAround)
        {
            SetNewRandomDestination();

            float waitTime = Random.Range(minMovementTime, maxMovementTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SetNewRandomDestination()
    {
        for (int i = 0; i < maxTries; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
            randomDirection += transform.position;
            randomDirection.y = transform.position.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, moveRadius, NavMesh.AllAreas))
            {
                Collider[] colliders = Physics.OverlapSphere(hit.position, avoidRadius, avoidZonesLayer);
                if (colliders.Length == 0)
                {
                    if (agent == null)
                    {
                        Debug.LogWarning("NavMeshAgent is missing!");
                        return;
                    }

                    roamTarget.transform.position = hit.position;
                    agent.isStopped = false;
                    agent.SetDestination(hit.position);

                    Debug.DrawLine(transform.position, hit.position, Color.green, 5f);
                    return;
                }
            }
        }

        Debug.LogWarning("no target found, outside of avoid zones!");
    }
}
