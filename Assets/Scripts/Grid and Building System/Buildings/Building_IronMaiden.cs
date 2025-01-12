using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_IronMaiden : MonoBehaviour
{
    public bool isAvailable = true;

    public Animator doubleDoors;

    public GameObject waypointOutside;
    public GameObject waypointInside;

    private const string BuildingIronKey = "Build_iron";

    private void Start()
    {
        if (doubleDoors == null)
        {
            Transform childTransfrom = transform.Find("Doors");

            if (childTransfrom != null)
            {
                doubleDoors = childTransfrom.GetComponent<Animator>();
            }
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

        if (!worldStates.HasState(BuildingIronKey))
        {
            Debug.LogError($"WorldStates does not contain the key '{BuildingIronKey}'. Make sure it is initialized.");
            return;
        }

        worldStates.ModifyState(BuildingIronKey, 1);
        Debug.Log($"Building added. Current count: {worldStates.GetStates()[BuildingIronKey]}");
    }

    public void RemoveBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(BuildingIronKey))
        {
            int currentCount = worldStates.GetStates()["Build_iron"];
            worldStates.ModifyState(BuildingIronKey, -1);
            Debug.Log($"Building removed. Remaining: {currentCount - 1}");
        }
        else
        {
            Debug.LogWarning("Cannot remove 'Build_iron'. State does not exist.");
        }
    }

    public void OpenDoubleDoors()
    {
        doubleDoors.Play("DoubleDoorsBackwards");
    }

    public void CloseDoubleDoors()
    {
        doubleDoors.Play("DoubleDoors");
    }
}
