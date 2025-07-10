using Unity.AI.Navigation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Pentagram : MonoBehaviour
{
    [Header("NavMeshUpdate")]
    public GameObject updateParts;
    public NavMeshSurface navMeshManager;

    public bool isAvailable = true;

    [System.Serializable]
    public class Waypoint
    {
        public GameObject waypointObject;
        public bool open = true;
    }

    public Waypoint[] waypoints = new Waypoint[4];


    [Header("Rotating Objects")]
    [SerializeField] private Transform[] rotatingObjects;
    [SerializeField] private float rotationSpeed = 90f;

    private Coroutine rotationCoroutine;

    private void Start()
    {
        NavMeshSync();

        if (rotationCoroutine == null)
        {
            rotationCoroutine = StartCoroutine(RotateObjects());
        }
    }

    // ------------- Control Waypoints Availability  -------------

    private void Update()
    {
        CheckAvailability();
    }

    public void CheckAvailability()
    {
        foreach (Waypoint wp in waypoints)
        {
            if (wp.open)
            {
                isAvailable = true;
                return;
            }
        }
        isAvailable = false;
    }

    public GameObject GetFreeWaypoint()
    {
        foreach (Waypoint wp in waypoints)
        {
            if (wp.open)
            {
                wp.open = false;
                return wp.waypointObject;
            }
        }
        return null;
    }

    public void SetWaypointState(int index, bool state)
    {
        if (index >= 0 && index < waypoints.Length)
        {
            waypoints[index].open = state;
            CheckAvailability();
        }
    }

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
