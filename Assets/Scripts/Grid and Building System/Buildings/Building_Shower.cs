using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Shower : MonoBehaviour
{
    public bool isAvailable = true;

    public new ParticleSystem particleSystem;

    public Animator openDoor;

    public GameObject waypointOutside;
    public GameObject waypointInside;

    private const string BuildingShowerKey = "Build_shower";

    private void Start()
    {
        if (openDoor == null)
        {
            Transform childTransform = transform.Find("ShowerDoor");

            if (childTransform != null)
            {
                openDoor = childTransform.GetComponent<Animator>();
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

        if (!worldStates.HasState(BuildingShowerKey))
        {
            Debug.LogError($"WorldStates does not contain the key '{BuildingShowerKey}'. Make sure it is initialized.");
            return;
        }

        worldStates.ModifyState(BuildingShowerKey, 1);
        Debug.Log($"Building added. Current count: {worldStates.GetStates()[BuildingShowerKey]}");
    }

    public void RemoveBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(BuildingShowerKey))
        {
            int currentCount = worldStates.GetStates()["Build_shower"];
            worldStates.ModifyState(BuildingShowerKey, -1);
            Debug.Log($"Building removed. Remaining: {currentCount - 1}");
        }
        else
        {
            Debug.LogWarning("Cannot remove 'Build_shower'. State does not exist.");
        }
    }

    public void StartSteam()
    {
        particleSystem.Play();
    }

    public void StopSteam()
    {
        particleSystem.Stop();
    }

    public void OpenDoorAnimation()
    {
        openDoor.Play("Shower Door Prototyp");
    }

    public void CloseDoorAnimation()
    {
        openDoor.Play("Shower Door Close Prototyp");
    }
}
