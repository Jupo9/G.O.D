using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Building_Trap : MonoBehaviour
{
    [Header("NavMeshUpdate")]
    public GameObject updateParts;
    public NavMeshSurface navMeshManager;

    public bool isAvailable = true;


    [Header("Active GameObjects")]
    [SerializeField] private List<GameObject> activatableObjects;

    [Header("Rotating Objects")]
    [SerializeField] private Transform[] rotatingObjects;
    [SerializeField] private float rotationSpeed = 90f;

    private Coroutine rotationCoroutine;

    private void Start()
    {
        NavMeshSync();
        SetActiveObjects(false);
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

    // ------------- Building Events -------------

    public void BuildingStainEvents(bool isActive)
    {
        SetActiveObjects(isActive);

        if (isActive)
        {
            if (rotationCoroutine == null)
            {
                rotationCoroutine = StartCoroutine(RotateObjects());
            }
        }
        else
        {
            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
                rotationCoroutine = null;
            }
        }
    }

    // ------------- Activate GameObjects -------------

    private void SetActiveObjects(bool state)
    {
        foreach (GameObject obj in activatableObjects)
        {
            if (obj != null)
            {
                obj.SetActive(state);
            }
        }
    }

    // ------------- Rotate Objects -------------

    private IEnumerator RotateObjects()
    {
        while (true)
        {
            foreach (Transform rot in rotatingObjects)
            {
                if (rot != null)
                {
                    rot.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
                }
            }

            yield return null;
        }
    }
}
