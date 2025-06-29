using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Building_Altar : MonoBehaviour, IResourceManager
{
    /// <summary>
    /// THIS SCRIPT CONTAINS:
    /// Storage System for a specific amount of Resources
    /// Visual Updates for stored Resources
    /// Interface to lock Resources when they are choosen to transport to another place
    /// ...
    /// </summary>

    [Header("Resource")]
    public int fireAmount = 0;
    public int lightAmount = 0;
    public int maxResourceStacksFire = 8;
    public int maxResourceStacksLight = 8;

    [Header("Queue")]
    [SerializeField] private int lockedFire = 0;
    [SerializeField] private int lockedLight = 0;

    [Header("Store Points")]
    [SerializeField] private Transform altarStorePoint;
    public Transform GetCheckoutPoint() => altarStorePoint;

    [Header("Battery Visuals")]
    [SerializeField] private List<Building_Battery> fireBatteries;
    [SerializeField] private List<Building_Battery> lightBatteries;

    [Header("NavMeshUpdate")]
    public GameObject updateParts;
    public NavMeshSurface navMeshManager;

    //WorldStates
    private const string fireResourceKey = "Res_fire";
    private const string lightResourceKey = "Res_light";

    [SerializeField] private RegisterResources registerResources;

    private void Start()
    {
        for (int i = 0; i < fireAmount; i++) AddVisualSlot("Fire");
        for (int i = 0; i < lightAmount; i++) AddVisualSlot("Light");

        SyncWithWorldState();

        navMeshManager.BuildNavMesh();

        ResourceCalculator.Instance?.RegisterResourceSource(new ResourceManager("Res_fire", ResourceType.Mine, this));
        ResourceCalculator.Instance?.RegisterResourceSource(new ResourceManager("Res_light", ResourceType.Mine, this));
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

    // ------------- Resource Managment -------------

    public void IncreaseFireAmount()
    {
        fireAmount += 1;
        RegisterFire();
        AddVisualSlot("Fire");
    }

    public void DecreaseFireAmount()
    {
        fireAmount -= 1;
        UnregisterFire();
        RemoveVisualSlot("Fire");
    }

    public void IncreaseLightAmount()
    {
        lightAmount += 1;
        RegisterLight();
        AddVisualSlot("Light");
    }

    public void DecreaseLightAmount()
    {
        lightAmount -= 1;
        UnregisterLight();
        RemoveVisualSlot("Fire");
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

    // ------------- Visual Handling -------------

    private void AddVisualSlot(string type)
    {
        List<Building_Battery> batteries = type == "Fire" ? fireBatteries : lightBatteries;

        foreach (var battery in batteries)
        {
            if (battery.GetCurrentSlotCount(type) < battery.GetMaxSlotCount(type))
            {
                battery.AddSlot(type);
                return;
            }
        }

        Debug.LogWarning($"No available battery slot for {type}");
    }

    private void RemoveVisualSlot(string type)
    {
        List<Building_Battery> batteries = type == "Fire" ? fireBatteries : lightBatteries;

        for (int i = batteries.Count - 1; i >= 0; i--)
        {
            if (batteries[i].GetCurrentSlotCount(type) > 0)
            {
                batteries[i].RemoveSlot(type);
                return;
            }
        }

        Debug.LogWarning($"No battery has a {type} slot to remove.");
    }
}

