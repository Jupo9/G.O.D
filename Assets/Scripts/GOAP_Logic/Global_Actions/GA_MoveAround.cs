using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GA_MoveAround : Actions
{
    [SerializeField] private float moveRadius = 25f;
    [SerializeField] private float minMovementTime = 5f;
    [SerializeField] private float maxMovementTime = 15f;

    private bool movingAround = false;

    public override bool PrePerform()
    {
        movingAround = true;
        StartCoroutine(RandomMove());

        return true;
    }

    public override bool PostPerform()
    {
        movingAround = false;
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
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, moveRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            Debug.Log($"Neues Ziel: {hit.position}");
        }
        else
        {
            Debug.LogWarning("Keine gültige NavMesh-Position gefunden!");
        }
    }
}
