using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DA_MoveAround : Actions
{
    private NavMeshAgent agents;

    [SerializeField] public float moveRadius = 10f;
    [SerializeField] public float minMovementTime = 1f;
    [SerializeField] public float maxMovementTime = 10f;

    private float timeToChangeDirection;
    private bool moveAround = false;

    void Update()
    {
        if (moveAround)
        {
            timeToChangeDirection -= Time.deltaTime;


            if (timeToChangeDirection <= 0 || agent.remainingDistance <= agent.stoppingDistance)
            {
                SetNewRandomDestination();

                timeToChangeDirection = Random.Range(minMovementTime, maxMovementTime);
            }
        }
    }

    void SetNewRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
        randomDirection += transform.position;


        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, moveRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    public override bool PrePerform()
    {
        agent = GetComponent<NavMeshAgent>();

        SetNewRandomDestination();

        timeToChangeDirection = Random.Range(minMovementTime, maxMovementTime);
        moveAround = true;
        return true;
    }

    public override bool PostPerform()
    {
        moveAround = false;
        return true;
    }
}
