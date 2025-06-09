using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Building_LightCharge : MonoBehaviour
{
    [Header("NavMeshUpdate")]
    public GameObject updateParts;
    public NavMeshSurface navMeshManager;

    private void Start()
    {
        navMeshManager.BuildNavMesh();
    }
}
