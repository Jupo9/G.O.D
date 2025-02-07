using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public Vector3 pointA = new Vector3(0, 0, 0);  
    public Vector3 pointB = new Vector3(5, 0, 0);  
    public float speed = 2f;

    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = pointB; 
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = (targetPosition == pointA) ? pointB : pointA;
        }
    }
}
