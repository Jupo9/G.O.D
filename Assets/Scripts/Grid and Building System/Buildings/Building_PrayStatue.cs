using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_PrayStatue : MonoBehaviour
{
    public bool isAvailable = false;

    [System.Serializable]
    public class Waypoint
    {
        public GameObject waypointObject;
        public bool open = true;
    }

    public Waypoint[] waypoints = new Waypoint[4];

    [Tooltip("This bool needs to be true if the buidling exist in the scene before the game starts!")]
    public bool startBuilding = false;

    private Angel assignedAngel;

    private Dictionary<Renderer, Material[]> originalMaterials = new();
    public bool isPreview = false;
    private bool builded = false;

    public int requiredLight = 0;

    private const string BuildingPrayKey = "Build_pray";

    private void Awake()
    {
        CacheOriginalMaterials();
    }

    private void Update()
    {
        if (isPreview)
        {
            isPreview = false;
            builded = true;
            AddBuilding();
            RestoreOriginalMaterials();

            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i].open = true;
            }
        }

        CheckAvailability();
    }

    private void OnEnable()
    {
        if (!isAvailable)
        {
            Angel[] allAngels = FindObjectsByType<Angel>(FindObjectsSortMode.None);
            foreach (Angel angel in allAngels)
            {
                if (angel.choosenOne)
                {
                    assignedAngel = angel;
                    break;
                }
            }

            if (assignedAngel != null)
            {
                assignedAngel.choosenOne = false;
                assignedAngel.buildingAction = true;
                assignedAngel.isBuilding = true;
                Debug.Log($"Building assigned to {assignedAngel.name}.");
            }
            else
            {
                Debug.LogWarning("No Angel selected to assign the building task.");
            }
        }

        if (startBuilding)
        {
            isPreview = true;
        }
    }

    private void OnDestroy()
    {
        if (builded)
        {
            RemoveBuilding();
        }
    }
    /// <summary>
    /// CheckAvailability is for controlling pray positions because this building is used to give 4 Angels a place to pray
    /// if there is no place left then the Action should be scripted
    /// </summary>
    public void CheckAvailability()
    {
        foreach (Waypoint wp in waypoints)
        {
            if (wp.open)
            {
                isAvailable = true;
                return;
            }
        }
        isAvailable = false;
    }

    public GameObject GetFreeWaypoint()
    {
        foreach (Waypoint wp in waypoints)
        {
            if (wp.open)
            {
                wp.open = false;
                return wp.waypointObject;
            }
        }
        return null;
    }

    public void SetWaypointState(int index, bool state)
    {
        if (index >= 0 && index < waypoints.Length)
        {
            waypoints[index].open = state;
            CheckAvailability();
        }
    }

    /// <summary>
    /// BuildingCosts is a method in many buildings. it works like that
    /// It looks for enough ressource in the scene. first for all lights in Storage, if there isn't enough then also in 
    /// the light building. The idea is that it first removed from storage and then from the working place because for the efficie 
    /// the angels and devil look first on working buildings, for any ressources to give the GOD tributes. So that
    /// the working area is faster empty and can be faster reused. if both building and cost would be focus the same building first, then
    /// the storage would be kinda useless in the beginnig and also there could be faster narrow passes as nothing is ever stored. 
    /// The final idear is that a building that cost to much is automatic grey in the UI and can not be build
    /// </summary>
    private void BuildingCosts()
    {
        int remainingLightToSubtract = requiredLight;

        GameObject[] storageBuildings = GameObject.FindGameObjectsWithTag("Storage");
        foreach (GameObject storageBuilding in storageBuildings)
        {
            Building_Storage buildingStorage = storageBuilding.GetComponentInChildren<Building_Storage>();
            if (buildingStorage != null && remainingLightToSubtract > 0)
            {
                int availableLight = buildingStorage.lightCounter;

                if (availableLight > 0)
                {
                    int lightToSubtract = Mathf.Min(availableLight, remainingLightToSubtract);
                    for (int i = 0; i < lightToSubtract; i++)
                    {
                        buildingStorage.DecreaseLightCounter();
                    }

                    remainingLightToSubtract -= lightToSubtract;
                }
            }
        }

        if (remainingLightToSubtract > 0)
        {
            GameObject[] lightBuildings = GameObject.FindGameObjectsWithTag("Angel_WorkBuilding");
            foreach (GameObject lightBuilding in lightBuildings)
            {
                Building_Light buildingLight = lightBuilding.GetComponentInChildren<Building_Light>();
                if (buildingLight != null && remainingLightToSubtract > 0)
                {
                    int availableLight = buildingLight.lightAmount;

                    if (availableLight > 0)
                    {
                        int lightToSubtract = Mathf.Min(availableLight, remainingLightToSubtract);
                        for (int i = 0; i < lightToSubtract; i++)
                        {
                            buildingLight.DecreaseLightAmount();
                        }

                        remainingLightToSubtract -= lightToSubtract;
                    }
                }
            }
        }

        if (remainingLightToSubtract > 0)
        {
            Debug.LogWarning("Not enough ressources!");
        }
        else
        {
            Debug.Log("Building Complete!");
        }
    }

    public void ChangePreviewState()
    {
        Debug.Log("Methode X in Script1 wurde aufgerufen.");
        isPreview = true;
    }

    private void CacheOriginalMaterials()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (!originalMaterials.ContainsKey(renderer))
            {
                originalMaterials[renderer] = renderer.materials;
            }
        }
    }

    private void RestoreOriginalMaterials()
    {
        foreach (var entry in originalMaterials)
        {
            Renderer renderer = entry.Key;
            Material[] materials = entry.Value;

            if (renderer != null && materials != null && materials.Length == renderer.materials.Length)
            {
                renderer.materials = materials;
            }
            else
            {
                Debug.LogWarning($"Mismatch in material count or missing renderer on {renderer.gameObject.name}");
            }
        }
    }
    /// <summary>
    /// World States so every NPC can see if there are enough or any buildings
    /// </summary>
    public void AddBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(BuildingPrayKey))
        {
            worldStates.SetState(BuildingPrayKey, 1);
            Debug.Log($"Building_PrayStatue added. Current count: {worldStates.GetStates()[BuildingPrayKey]}");
        }
        else
        {
            worldStates.ModifyState(BuildingPrayKey, +1);
            Debug.Log($"Building_PrayStatue added. Current count: {worldStates.GetStates()[BuildingPrayKey]}");
        }
    }

    public void RemoveBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(BuildingPrayKey))
        {
            int currentCount = worldStates.GetStates()["Build_pray"];
            if (currentCount > 0)
            {
                worldStates.ModifyState(BuildingPrayKey, -1);
            }
        }
    }
}
