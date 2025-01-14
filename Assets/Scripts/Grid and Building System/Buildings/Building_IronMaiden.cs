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
            worldStates.SetState(BuildingIronKey, 1);
            Debug.Log($"Building_IronMaiden added. Current count: {worldStates.GetStates()[BuildingIronKey]}");
        }
        else
        {
            worldStates.ModifyState(BuildingIronKey, 1);
            Debug.Log($"Building_IronMaiden added. Current count: {worldStates.GetStates()[BuildingIronKey]}");
        }
    }

    public void RemoveBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(BuildingIronKey))
        {
            int currentCount = worldStates.GetStates()["Build_iron"];
            if (currentCount > 0)
            {
                worldStates.ModifyState(BuildingIronKey, -1);
                Debug.Log($"Building removed. Remaining: {worldStates.GetStates()[BuildingIronKey]}");
            }
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
