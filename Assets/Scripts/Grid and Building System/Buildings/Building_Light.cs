using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Light : MonoBehaviour
{
    [Header("Conditions")]
    public bool lightIsOpen = true;
    public bool angelInside = false;
    public bool calculate = false;
    public bool empty = false;
    public bool addOne = false;
    public bool minusOne = false;

    private bool fullBuilding = false;

    [Header("Light Inputs")]
    public float maxAmount = 4f;
    public float lightAmount = 0f;

    public GameObject lightResource;

    [Header("Waypoints")]
    public GameObject waypointOutside;
    public GameObject waypointInside;

    private const string BuildingLightKey = "Build_light";
    private const string LightRessource = "Res_light";

    private void Update()
    {
        if(addOne)
        {
            addOne = false;
            IncreaseLightAmount();
        }

        if(minusOne)
        {
            minusOne = false;
            DecreaseLightAmount();
        }


        if (lightAmount == maxAmount && !fullBuilding)
        {
            lightIsOpen = false;
            fullBuilding = true;
            RemoveBuilding();
            NoKeyFixer();
        }

        if (lightAmount == maxAmount -1 && fullBuilding)
        {
            fullBuilding = false;
            AddBuilding();
        }

        if (!angelInside)
        {
            if (lightAmount < maxAmount)
            {
                lightIsOpen = true;
            }
        }

        if(lightAmount > 0 && empty)
        {
            lightResource.SetActive(true);
            empty = false;
          
        }

        if(lightAmount == 0 && !empty)
        {
            lightResource.SetActive(false);
            empty = true;
        }

        if (lightAmount < 0)
        {
            lightAmount = 0;
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

        if (!worldStates.HasState(BuildingLightKey))
        {
            worldStates.SetState(BuildingLightKey, 1);
            Debug.Log($"Building_Light added. Current count: {worldStates.GetStates()[BuildingLightKey]}");
        }
        else
        {
            worldStates.ModifyState(BuildingLightKey, +1);
            Debug.Log($"Building_Light added. Current count: {worldStates.GetStates()[BuildingLightKey]}");
        }
    }

    public void RemoveBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(BuildingLightKey))
        {
            int currentCount = worldStates.GetStates()["Build_light"];
            if (currentCount > 0) 
            {
                worldStates.ModifyState(BuildingLightKey, -1);
                Debug.Log($"Building removed. Remaining: {worldStates.GetStates()[BuildingLightKey]}");
            }
        }
    }

    public void AddLight()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(LightRessource))
        {
            worldStates.SetState(LightRessource, 1);
            Debug.Log($"Light added. Current count: {worldStates.GetStates()[LightRessource]}");
        }
        else
        {
            worldStates.ModifyState(LightRessource, +1);
            Debug.Log($"Light added. Current count: {worldStates.GetStates()[LightRessource]}");
        }
    }

    public void RemoveLight() 
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(LightRessource))
        {
            int currentCount = worldStates.GetStates()["Res_light"];
            if (currentCount > 0) 
            {
                worldStates.ModifyState(LightRessource, -1);
                Debug.Log($"Light added. Current count: {worldStates.GetStates()[LightRessource]}");
            }
        }
    }

    public void IncreaseLightAmount()
    {
        lightAmount += 1f;
        AddLight();

    }

    public void DecreaseLightAmount()
    {
        lightAmount -= 1f;
        RemoveLight();
    }

    private void NoKeyFixer()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(LightRessource))
        {
            worldStates.SetState(LightRessource, 0);
            Debug.Log($"Ressource removed. Remaining: {worldStates.GetStates()[LightRessource]}");
        }
        if (!worldStates.HasState(BuildingLightKey))
        {
            worldStates.SetState(BuildingLightKey, 0);
            Debug.Log($"Building_Light added. Current count: {worldStates.GetStates()[BuildingLightKey]}");
        }

    }
}
