using System.Collections.Generic;
using UnityEngine;

public class Building_Fire : MonoBehaviour
{
    //near the same as light building
    [Header("Conditions")]
    public bool isAvailable = false;
    public bool devilInside = false;
    public bool calculate = false;
    public bool empty = false;
    public bool addOne = false;
    public bool minusOne = false;
    // startBuilding is for Building that exist in the scene when the game starts, so basically start buildings, set bool = true only for them!
    [Tooltip("This bool needs to be true if the buidling exist in the scene before the game starts!")]
    public bool startBuilding = false;

    public bool fullBuilding = false;

    [Header("Fire Inputs")]
    public int maxAmount = 4;
    public int fireAmount = 0;

    public GameObject fireResource;

    [Header("Waypoints")]
    public GameObject waypointOutside;
    public GameObject waypointInside;

    private Dictionary<Renderer, Material[]> originalMaterials = new();
    public bool isPreview = false;
    private bool builded = false;

    private Devil assignedDevil;

    public int requiredFire = 0;

    private const string BuildingFireKey = "Build_fire";
    private const string FireRessource = "Res_fire";

    private void Awake()
    {
        CacheOriginalMaterials();
    }

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
            isAvailable = false;
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
                isAvailable = true;
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

        if (isPreview)
        {
            isPreview = false;
            isAvailable = true;
            builded = true;
            AddBuilding();
            RestoreOriginalMaterials();
        }

    }

    private void OnEnable()
    {
        if (!isAvailable)
        {
            Devil[] allDevils = FindObjectsByType<Devil>(FindObjectsSortMode.None);
            foreach (Devil devil in allDevils)
            {
                if (devil.choosenOne)
                {
                    assignedDevil = devil;
                    break;
                }
            }

            if (assignedDevil != null)
            {
                assignedDevil.choosenOne = false;
                assignedDevil.buildingAction = true;
                assignedDevil.isBuilding = true;
                Debug.Log($"Building assigned to {assignedDevil.name}.");
            }
            else
            {
                Debug.LogWarning("No Devil selected to assign the building task.");
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

    private void BuildingCosts()
    {
        int remainingFireToSubtract = requiredFire;


        GameObject[] storageBuildings = GameObject.FindGameObjectsWithTag("Storage");
        foreach (GameObject storageBuilding in storageBuildings)
        {
            Building_Storage buildingStorage = storageBuilding.GetComponentInChildren<Building_Storage>();
            if (buildingStorage != null && remainingFireToSubtract > 0)
            {
                int availableFire = buildingStorage.fireCounter;

                if (availableFire > 0)
                {
                    int fireToSubtract = Mathf.Min(availableFire, remainingFireToSubtract);
                    for (int i = 0; i < fireToSubtract; i++)
                    {
                        buildingStorage.DecreaseFireCounter();
                    }

                    remainingFireToSubtract -= fireToSubtract;
                }
            }
        }

        if (remainingFireToSubtract > 0)
        {
            GameObject[] fireBuildings = GameObject.FindGameObjectsWithTag("Devil_WorkBuilding");
            foreach (GameObject fireBuilding in fireBuildings)
            {
                Building_Fire buildingFire = fireBuilding.GetComponentInChildren<Building_Fire>();
                if (buildingFire != null && remainingFireToSubtract > 0)
                {
                    int availableFire = buildingFire.fireAmount;

                    if (availableFire > 0)
                    {
                        int fireToSubtract = Mathf.Min(availableFire, remainingFireToSubtract);
                        for (int i = 0; i < fireToSubtract; i++)
                        {
                            buildingFire.DecreaseFireAmount();
                        }

                        remainingFireToSubtract -= fireToSubtract;
                    }
                }
            }
        }

        if (remainingFireToSubtract > 0)
        {
            Debug.LogWarning("Not enough ressources!");
        }
        else
        {
            Debug.Log("Building Complete!");
        }
    }

    /// <summary>
    /// Change State when building was build, as long as "isPreview = false" the building can't be used by Devils
    /// </summary>
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
        fireAmount += 1;
        AddFire();
    }

    public void DecreaseFireAmount()
    {
        fireAmount -= 1;
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

    public void DeleteRessource()
    {
        DecreaseFireAmount();
    }
}
