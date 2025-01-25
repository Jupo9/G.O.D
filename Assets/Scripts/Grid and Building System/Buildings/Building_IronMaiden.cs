using System.Collections.Generic;
using UnityEngine;

public class Building_IronMaiden : MonoBehaviour
{
    //near the same as Shower but the animation here don't work that good like in shower. they look a bit buggy
    public bool test = false;

    public bool isAvailable = false;

    public Animator doubleDoors;

    public GameObject waypointOutside;
    public GameObject waypointInside;
    public int requiredFire = 0;

    // startBuilding is for Building that exist in the scene when the game starts, so basically start buildings, set bool = true only for them!
    [Tooltip("This bool needs to be true if the buidling exist in the scene before the game starts!")]
    public bool startBuilding = false; 

    private Dictionary<Renderer, Material[]> originalMaterials = new();
    public bool isPreview = false;
    private bool builded = false;

    private Devil assignedDevil;

    private const string BuildingIronKey = "Build_iron";

    private void Awake()
    {
        CacheOriginalMaterials();
    }

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

    private void Update()
    {
        if (isPreview)
        {
            isPreview = false;
            builded = true;
            isAvailable = true;
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


    public void RemoveBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(BuildingIronKey))
        {
            int currentCount = worldStates.GetStates()["Build_iron"];
            if (currentCount > 0)
            {
                worldStates.ModifyState(BuildingIronKey, -1);
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
