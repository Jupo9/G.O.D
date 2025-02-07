using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 10f;
    [SerializeField] private Vector3 axis = Vector3.up;

    private void Update()
    {
        if (target != null)
        {
            transform.RotateAround(target.position, axis, speed * Time.deltaTime);
        }
    }

}
