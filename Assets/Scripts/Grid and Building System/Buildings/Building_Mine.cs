using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Mine : MonoBehaviour, IResourceManager
{
    /// <summary>
    /// THIS SCRIPT CONTAINS:
    /// Working Building Structure for Devil and Angels
    /// Amount of Resources with visual updates
    /// Interface to lock Resources when they are choosen to transport to another place
    /// ...
    /// </summary>

    [Header("Resources")]
    [SerializeField] private int maxAmount = 4;
    [SerializeField] private int currentAmount = 0;
    [SerializeField] private int fireAmount = 0;
    [SerializeField] private int lightAmount = 0;

    [Header("Conditions")]
    public bool isAvailable = true;

    private bool isBlocked = false;

    public bool IsBlocked
    {
        get => isBlocked;
        set
        {
            isBlocked = value;
            UpdateAvailability();
        }
    }

    [Header("Queue")]
    [SerializeField] private int lockedFire = 0;
    [SerializeField] private int lockedLight = 0;

    [Header("Store")]
    [SerializeField] private Building_Battery batteryVisual;
    [SerializeField] private Transform mineStorePoint;
    public Transform GetMineStorePoint() => mineStorePoint;

    [Header("CheckoutPoint")]
    [SerializeField] private Transform checkoutPoint;
    public Transform GetCheckoutPoint() => checkoutPoint;

    [SerializeField] private RegisterResources registerResources; 

    //WorldStates
    private const string fireResourceKey = "Res_fire";
    private const string lightResourceKey = "Res_light";

    private void Start()
    {
        SyncWithWorldState();
        BatteryVisual();

        ResourceCalculator.Instance?.RegisterResourceSource(new ResourceManager("Res_fire", ResourceType.Mine, this));
        ResourceCalculator.Instance?.RegisterResourceSource(new ResourceManager("Res_light", ResourceType.Mine, this));
    }

    private void Update()
    {
        CalculateAmounts();
        UpdateAvailability();
    }

    // ------------- Interface (IResourceManager) -------------

    //check if resource type is available
    public bool HasAvailableResource(string resourceType)
    {
        if (resourceType == "Light")
        {
            return lightAmount - lockedLight > 0;
        }
        else if (resourceType == "Fire")
        {
            return fireAmount - lockedFire > 0;
        }

        return false;
    }

    //lock choosen resource type
    public bool LockResource(string resourceType)
    {
        if (!HasAvailableResource(resourceType))
        {
            return false;
        }

        if (resourceType == "Light")
        {
            lockedLight++;
            return true;
        }

        if (resourceType == "Fire")
        {
            lockedFire++;
            return true;
        }

        return false;
    }

    //release lock for resource type
    public void ReleaseLock(string resourceType)
    {
        if (resourceType == "Light" && lockedLight > 0)
        {
            lockedLight--;
        }
        else if (resourceType == "Fire" && lockedFire > 0)
        {
            lockedFire--;
        }

    }

    // release lock for resource type and remove resource type
    public void ConsumeLockedRessource(string resourceType)
    {
        if (resourceType == "Light" && lightAmount > 0 && lockedLight > 0)
        {
            lockedLight--;
            DecreaseLightAmount();
            Debug.Log("picked up one light.");
        }

        if (resourceType == "Fire" && fireAmount > 0 && lockedFire > 0)
        {
            lockedFire--;
            DecreaseFireAmount();
            Debug.Log("picked up one Fire.");
        }
    }

    // ------------- Resource & Availability Managment -------------

    public bool IsAvailable => currentAmount < maxAmount && !isBlocked;

    private void UpdateAvailability()
    {
        isAvailable = IsAvailable;
    }

    public void SetBlocked(bool blocked)
    {
        IsBlocked = blocked;
    }

    private void CalculateAmounts()
    {
        currentAmount = fireAmount + lightAmount;
    }

    public void IncreaseFireAmount()
    {
        fireAmount += 1;
        RegisterFire();
        batteryVisual?.AddSlot("Fire");
    }

    public void DecreaseFireAmount()
    {
        fireAmount -= 1;
        UnregisterFire();
        batteryVisual?.RemoveSlot("Fire");
    }

    public void IncreaseLightAmount()
    {
        lightAmount += 1;
        RegisterLight();
        batteryVisual?.AddSlot("Light");
    }

    public void DecreaseLightAmount()
    {
        lightAmount -= 1;
        UnregisterLight();
        batteryVisual?.RemoveSlot("Light");
    }

    private void BatteryVisual()
    {
        /*if (batteryVisual == null)
        {
            Debug.LogError("Battery is Missing");
        }
        else
        {
            Debug.Log("Battery was found");
        }*/

        for (int i = 0; i < fireAmount; i++)
        {
            batteryVisual?.AddSlot("Fire");
        }

        for (int i = 0; i < lightAmount; i++)
        {
            batteryVisual?.AddSlot("Light");
        }
    }

    // ------------- Register World States -------------

    private void SyncWithWorldState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(fireResourceKey))
        {
            worldStates.SetState(fireResourceKey, 0);
        }

        if (!worldStates.HasState(lightResourceKey))
        {
            worldStates.SetState(lightResourceKey, 0);
        }

        worldStates.ModifyState(fireResourceKey, fireAmount);
        worldStates.ModifyState(lightResourceKey, lightAmount);
        ResourceWatcher.Notify();
    }

    private void RegisterFire()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(fireResourceKey))
        {
            worldStates.SetState(fireResourceKey, 0);
        }

        worldStates.ModifyState(fireResourceKey, +1);
        ResourceWatcher.Notify();
    }

    private void RegisterLight()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(lightResourceKey))
        {
            worldStates.SetState(lightResourceKey, 0);
        }

        worldStates.ModifyState(lightResourceKey, +1);
        ResourceWatcher.Notify();
    }

    private void UnregisterFire()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(fireResourceKey))
        {
            worldStates.ModifyState(fireResourceKey, -1);
        }
        ResourceWatcher.Notify();
    }

    private void UnregisterLight()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(lightResourceKey))
        {
            worldStates.ModifyState(lightResourceKey, -1);
        }
        ResourceWatcher.Notify();
    }
}
