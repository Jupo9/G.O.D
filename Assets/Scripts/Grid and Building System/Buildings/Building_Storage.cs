using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Storage : MonoBehaviour
{
    public GameObject fireRessource;
    public GameObject lightRessource;

    public float fireCounter = 0f;
    public float lightCounter = 0f;

    public bool calculate = false;

    private bool fireOn  = false;
    private bool lightOn = false;

    private const string BuildingStorageKey = "Build_storage";

    private void Start()
    {
        fireRessource.SetActive(false);
        lightRessource.SetActive(false);
    }

    private void Update()
    {
        if (fireCounter > 0 && !fireOn)
        {
            fireRessource.SetActive(true);
            fireOn = true;
        }
        if (lightCounter > 0 && !lightOn) 
        {
            lightRessource.SetActive(true);
            lightOn = true;
        }
        if (fireCounter <= 0)
        {
            fireRessource.SetActive(false);
            fireOn = false;
        }
        if (lightCounter <= 0)
        {
            lightRessource.SetActive(false);
            lightOn = false;
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

        if (!worldStates.HasState(BuildingStorageKey))
        {
            Debug.LogError($"WorldStates does not contain the key '{BuildingStorageKey}'. Make sure it is initialized.");
            return;
        }

        worldStates.ModifyState(BuildingStorageKey, 1);
        Debug.Log($"Building added. Current count: {worldStates.GetStates()[BuildingStorageKey]}");
    }

    public void RemoveBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(BuildingStorageKey))
        {
            int currentCount = worldStates.GetStates()["Build_storage"];
            worldStates.ModifyState(BuildingStorageKey, -1);
            Debug.Log($"Building removed. Remaining: {currentCount - 1}");
        }
        else
        {
            Debug.LogWarning("Cannot remove 'Build_storage'. State does not exist.");
        }
    }

    public void IncreaseFireCounter()
    {
        fireCounter += 1;
    }

    public void DecreaseFireCounter() 
    {
        fireCounter -= 1;
    }

    public void IncreaseLightCounter()
    {
        lightCounter += 1;
    }

    public void DecreaseLightCounter()
    {
        lightCounter -= 1;
    }

}
