using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class Building_Fire : MonoBehaviour
{
    [Header("Conditions")]
    public bool isAvailable = true;
    public bool devilInside = false;
    public bool calculate = false;

    [Header("Fire Inputs")]
    public float maxAmount = 4f;
    public float fireAmount = 0f;

    public GameObject fireResource;

    [Header("Waypoints")]
    public GameObject waypointOutside;
    public GameObject waypointInside;


    private const string BuildingFireKey = "Build_fire";

    private void Update()
    {
        if (fireAmount == maxAmount)
        {
            isAvailable = false;
        }

        if (!devilInside)
        {
            if (fireAmount < maxAmount)
            {
                isAvailable = true;
            }
        }

        if (fireAmount > 0)
        {
            fireResource.SetActive(true);
        }

        if (fireAmount == 0)
        {
            fireResource.SetActive(false);
        }

    }

    private void OnEnable()
    {
        AddBuilding();
    }

    private void OnDestroy()
    {
        RemoveBuilding();
    }

    public void AddBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(BuildingFireKey))
        {
            Debug.LogError($"WorldStates does not contain the key '{BuildingFireKey}'. Make sure it is initialized.");
            return;
        }

        worldStates.ModifyState(BuildingFireKey, 1);
        Debug.Log($"Building added. Current count: {worldStates.GetStates()[BuildingFireKey]}");
    }

    public void RemoveBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(BuildingFireKey))
        {
            int currentCount = worldStates.GetStates()["Build_fire"];
            worldStates.ModifyState(BuildingFireKey, -1);
            Debug.Log($"Building removed. Remaining: {currentCount - 1}");
        }
        else
        {
            Debug.LogWarning("Cannot remove 'Build_fire'. State does not exist.");
        }
    }

    public void IncreaseFireAmount()
    {
        fireAmount += 1f;
    }

    public void DecreaseFireAmount()
    {
        fireAmount -= 1f;
    }

    public void TestStandby()
    {
        if (isAvailable)
        {
            isAvailable = false;
        }
        else
        {
            isAvailable = true;
        }
    }
}
