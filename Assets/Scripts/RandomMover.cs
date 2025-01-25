using UnityEngine;
using UnityEngine.AI;

public class RandomMover : MonoBehaviour 
{
    /// <summary>
    /// Test Script for moving on Nav mesh, early Script for testing how many NPC can exist without performance issues
    /// </summary>
    private NavMeshAgent agent;

    [SerializeField] public float moveRadius = 10f;
    [SerializeField] public float minMovementTime = 1f;
    [SerializeField] public float maxMovementTime = 10f;

    private float timeToChangeDirection;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        SetNewRandomDestination();

        timeToChangeDirection = Random.Range(minMovementTime, maxMovementTime);
    }

    void Update()
    {
        timeToChangeDirection -= Time.deltaTime;


        if (timeToChangeDirection <= 0 || agent.remainingDistance <= agent.stoppingDistance)
        {
            SetNewRandomDestination();

            timeToChangeDirection = Random.Range(minMovementTime, maxMovementTime);
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
}

