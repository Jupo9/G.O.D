using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Light : MonoBehaviour
{
    [Header("Conditions")]
    public bool isAvailable = true;
    public bool angelInside = false;
    public bool calculate = false;

    [Header("Light Inputs")]
    public float maxAmount = 4f;
    public float lightAmount = 0f;

    public GameObject lightResource;

    [Header("Waypoints")]
    public GameObject waypointOutside;
    public GameObject waypointInside;

    private const string BuildingFireKey = "Build_light";

    private void Update()
    {
        if (lightAmount == maxAmount)
        {
            isAvailable = false;
        }

        if (!angelInside)
        {
            if (lightAmount < maxAmount)
            {
                isAvailable = true;
            }
        }

        if(lightAmount > 0)
        {
            lightResource.SetActive(true);
        }

        if(lightAmount == 0)
        {
            lightResource.SetActive(false);
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
            int currentCount = worldStates.GetStates()["Build_light"];
            worldStates.ModifyState(BuildingFireKey, -1);
            Debug.Log($"Building removed. Remaining: {currentCount - 1}");
        }
        else
        {
            Debug.LogWarning("Cannot remove 'Build_light'. State does not exist.");
        }
    }

    public void IncreaseLightAmount()
    {
        lightAmount += 1f;
    }

    public void DecreaseLightAmount()
    {
        lightAmount -= 1f;
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
