using System.Collections.Generic;
using UnityEngine;

public class TriggerBox : MonoBehaviour
{
    /// <summary>
    /// THIS SCRIPT CONTAINS:
    /// Simple Script for adding OnTriggereffects
    /// </summary>

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Devil"))
        {
            Debug.Log("Hello");
        }
    }
}

