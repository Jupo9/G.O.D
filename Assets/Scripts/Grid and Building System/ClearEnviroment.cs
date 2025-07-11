using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearEnviroment : MonoBehaviour
{
    [SerializeField] private float clearRadius = 5f;

    private void Start()
    {
        ClearEnvironmentObjects();
    }

    private void ClearEnvironmentObjects()
    {
        GameObject[] envObjects = GameObject.FindGameObjectsWithTag("Enviroment");

        foreach (GameObject obj in envObjects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance <= clearRadius)
            {
                Destroy(obj);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, clearRadius);
    }
}
