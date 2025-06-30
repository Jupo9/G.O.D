using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Building_LightCharge : MonoBehaviour
{
    [Header("NavMeshUpdate")]
    public GameObject updateParts;
    public NavMeshSurface navMeshManager;

    public bool isAvailable = true;

    private void Start()
    {
        NavMeshSync();
    }

    // ------------- NavMesh Update -------------

    private void NavMeshSync()
    {
        if (navMeshManager == null)
        {
            navMeshManager = FindAnyObjectByType<NavMeshSurface>();
        }

        if (navMeshManager != null)
        {
            navMeshManager.BuildNavMesh();
        }
        else
        {
            Debug.LogWarning("No NavMeshSurface found in the scene.");
        }
    }
}
