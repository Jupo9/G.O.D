using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_PrayStatue : MonoBehaviour
{
    public bool isAvailable = true;

    [System.Serializable]
    public class Waypoint
    {
        public GameObject waypointObject;
        public bool open = true;
    }

    public Waypoint[] waypoints = new Waypoint[4];

    private const string BuildingPrayKey = "Build_pray";

    private void Update()
    {
        CheckAvailability();
    }

    private void OnDestroy()
    {
        RemoveBuilding();
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

    public void AddBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(BuildingPrayKey))
        {
            worldStates.SetState(BuildingPrayKey, 1);
            Debug.Log($"Building_PrayStatue added. Current count: {worldStates.GetStates()[BuildingPrayKey]}");
        }
        else
        {
            worldStates.ModifyState(BuildingPrayKey, +1);
            Debug.Log($"Building_PrayStatue added. Current count: {worldStates.GetStates()[BuildingPrayKey]}");
        }
    }

    public void RemoveBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(BuildingPrayKey))
        {
            int currentCount = worldStates.GetStates()["Build_pray"];
            if (currentCount > 0)
            {
                worldStates.ModifyState(BuildingPrayKey, -1);
            }
        }
    }
}
