using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class Building_Fire : MonoBehaviour
{
    [Header("Conditions")]
    public bool fireIsOpen = true;
    public bool devilInside = false;
    public bool calculate = false;
    public bool empty = false;
    public bool addOne = false;
    public bool minusOne = false;

    private bool fullBuilding = false;

    [Header("Fire Inputs")]
    public float maxAmount = 4f;
    public float fireAmount = 0f;

    public GameObject fireResource;

    [Header("Waypoints")]
    public GameObject waypointOutside;
    public GameObject waypointInside;


    private const string BuildingFireKey = "Build_fire";
    private const string FireRessource = "Res_fire";

    private void Update()
    {
        if (addOne)
        {
            addOne = false;
            IncreaseFireAmount();
        }

        if (minusOne)
        {
            minusOne = false;
            DecreaseFireAmount();
        }

        if (fireAmount == maxAmount && !fullBuilding)
        {
            fireIsOpen = false;
            fullBuilding = true;
            RemoveBuilding();
            NoKeyFixer();
        }

        if (fireAmount == maxAmount - 1 && fullBuilding)
        {
            fullBuilding = false;
            AddBuilding();
        }

        if (!devilInside)
        {
            if (fireAmount < maxAmount)
            {
                fireIsOpen = true;
            }
        }

        if (fireAmount > 0 && empty)
        {
            fireResource.SetActive(true);
            empty = false;
        }

        if (fireAmount == 0 && !empty)
        {
            fireResource.SetActive(false);
            empty = true;
        }

        if (fireAmount < 0)
        {
            fireAmount = 0;
            NoKeyFixer();
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
            worldStates.SetState(BuildingFireKey, 1);
            Debug.Log($"Building_Fire added. Current count: {worldStates.GetStates()[BuildingFireKey]}");
        }
        else
        {
            worldStates.ModifyState(BuildingFireKey, +1);
            Debug.Log($"Building_Fire added. Current count: {worldStates.GetStates()[BuildingFireKey]}");
        }
    }

    public void RemoveBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(BuildingFireKey))
        {
            int currentCount = worldStates.GetStates()["Build_fire"];
            if (currentCount > 0)
            {
                worldStates.ModifyState(BuildingFireKey, -1);
            }

        }
    }

    public void AddFire()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(FireRessource))
        {
            worldStates.SetState(FireRessource, 1);
            Debug.Log($"Fireadded. Current count: {worldStates.GetStates()[FireRessource]}");
        }
        else
        {
            worldStates.ModifyState(FireRessource, +1);
            Debug.Log($"Fire added. Current count: {worldStates.GetStates()[FireRessource]}");
        }
    }

    public void RemoveFire() 
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(FireRessource))
        {
            int currentCount = worldStates.GetStates()["Res_fire"];
            if (currentCount > 0)
            {
                worldStates.ModifyState(FireRessource, -1);
                Debug.Log($"Fire added. Current count: {worldStates.GetStates()[FireRessource]}");
            }
        }
    }

    public void IncreaseFireAmount()
    {
        fireAmount += 1f;
        AddFire();
    }

    public void DecreaseFireAmount()
    {
        fireAmount -= 1f;
        RemoveFire();
    }

    private void NoKeyFixer()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(FireRessource))
        {
            worldStates.SetState(FireRessource, 0);
            Debug.Log($"Ressource removed. Remaining: {worldStates.GetStates()[FireRessource]}");
        }

        if (!worldStates.HasState(BuildingFireKey))
        {
            worldStates.SetState(BuildingFireKey, 0);
        }
    }
}
