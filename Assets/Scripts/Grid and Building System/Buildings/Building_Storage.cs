using System.Collections.Generic;
using UnityEngine;

public class Building_Storage : MonoBehaviour
{
    public GameObject fireRessource;
    public GameObject lightRessource;

    public int fireCounter = 0;
    public int lightCounter = 0;
    public int maxFireCounter = 10;
    public int maxLightCounter = 10;

    public bool isAvailable = false;
    public bool calculate = false;
    public bool oneFire = false;
    public bool oneLight = false;
    public bool removeFire = false;
    public bool removeLight = false;
    public bool emptyFire  = false;
    public bool emptyLight = false;

    private bool fullFire = false;
    private bool fullLight = false;
    private bool storageIsFull = false;

    private Dictionary<Renderer, Material[]> originalMaterials = new();
    public bool isPreview = false;
    private bool builded = false;

    // startBuilding is for Building that exist in the scene when the game starts, so basically start buildings, set bool = true only for them!
    [Tooltip("This bool needs to be true if the buidling exist in the scene before the game starts!")]
    public bool startBuilding = false;

    private Angel assignedAngel;

    public int requiredLight = 0;

    private const string BuildingStorageKey = "Build_storage";

    private const string LightRessourceStorage = "Res_light";
    private const string FireRessourceStorage = "Res_fire";
    private void Awake()
    {
        CacheOriginalMaterials();
    }

    private void Start()
    {
        fireRessource.SetActive(false);
        lightRessource.SetActive(false);
    }

    private void Update()
    {
        if (oneFire)
        {
            oneFire = false;
            IncreaseFireCounter();
        }

        if (oneLight) 
        {
            oneLight = false;
            IncreaseLightCounter();
        }

        if (removeFire)
        {
            removeFire = false;
            DecreaseFireCounter();
        }

        if (removeLight)
        {
            removeLight = false;
            DecreaseLightCounter();
        }


        if (fireCounter == maxFireCounter && lightCounter == maxLightCounter && !storageIsFull)
        {
            storageIsFull = true;
            RemoveBuilding();
            NoKeyFixer();
        }

        if (lightCounter == maxLightCounter - 1 && fullLight)
        {
            fullLight = false;
            if (storageIsFull)
            {
                storageIsFull = false;
                AddBuilding();
            }
        }

        if (fireCounter == maxFireCounter - 1 && fullFire)
        {
            fullFire = false;
            if (storageIsFull) 
            {
                storageIsFull = false;
                AddBuilding();
            }

        }

        if (lightCounter == maxLightCounter && !fullLight)
        {
            fullLight = true;
        }

        if (fireCounter == maxFireCounter && !fullFire)
        {
            fullFire = true;
        }

        if (fireCounter > 0 && emptyFire)
        {
            fireRessource.SetActive(true);
            emptyFire = false;
        }
        if (lightCounter > 0 && emptyLight) 
        {
            lightRessource.SetActive(true);
            emptyLight = false;
        }
        if (fireCounter == 0 && !emptyFire)
        {
            fireRessource.SetActive(false);
            emptyFire = true;
        }
        if (lightCounter == 0 && !emptyLight)
        {
            lightRessource.SetActive(false);
            emptyLight = true;
        }

        if (lightCounter < 0)
        {
            lightCounter = 0;
            NoKeyFixer();
        }

        if (fireCounter < 0)
        {
            fireCounter = 0;
            NoKeyFixer();
        }

        if (isPreview)
        {
            isPreview = false;
            builded = true;
            AddBuilding();
            RestoreOriginalMaterials();
        }
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

    public void AddBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(BuildingStorageKey))
        {
            worldStates.SetState(BuildingStorageKey, 1);
            Debug.Log($"Building_Storage added. Current count: {worldStates.GetStates()[BuildingStorageKey]}");
        }
        else
        {
            worldStates.ModifyState(BuildingStorageKey, +1);
            Debug.Log($"Building_Storage added. Current count: {worldStates.GetStates()[BuildingStorageKey]}");
        }
    }

    public void RemoveBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(BuildingStorageKey))
        {
            int currentCount = worldStates.GetStates()["Build_storage"];
            if (currentCount > 0) 
            {
                worldStates.ModifyState(BuildingStorageKey, -1);
            }
        }
    }

    public void AddLight()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(LightRessourceStorage))
        {
            worldStates.SetState(LightRessourceStorage, 1);
            Debug.Log($"Light added. Current count: {worldStates.GetStates()[LightRessourceStorage]}");
        }
        else
        {
            worldStates.ModifyState(LightRessourceStorage, +1);
            Debug.Log($"Light added. Current count: {worldStates.GetStates()[LightRessourceStorage]}");
        }
    }

    public void RemoveLight()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(LightRessourceStorage))
        {
            int currentCount = worldStates.GetStates()["Res_light"];
            if (currentCount > 0)
            {
                worldStates.ModifyState(LightRessourceStorage, -1);
                Debug.Log($"Light added. Current count: {worldStates.GetStates()[LightRessourceStorage]}");
            }
        }
    }

    public void AddFire()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(FireRessourceStorage))
        {
            worldStates.SetState(FireRessourceStorage, 1);
            Debug.Log($"Fireadded. Current count: {worldStates.GetStates()[FireRessourceStorage]}");
        }
        else
        {
            worldStates.ModifyState(FireRessourceStorage, +1);
            Debug.Log($"Fire added. Current count: {worldStates.GetStates()[FireRessourceStorage]}");
        }
    }

    public void RemoveFire()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(FireRessourceStorage))
        {
            int currentCount = worldStates.GetStates()["Res_fire"];
            if (currentCount > 0)
            {
                worldStates.ModifyState(FireRessourceStorage, -1);
                Debug.Log($"Fire added. Current count: {worldStates.GetStates()[FireRessourceStorage]}");
            }
        }
    }

    public void IncreaseFireCounter()
    {
        fireCounter += 1;
        AddFire();
    }

    public void DecreaseFireCounter() 
    {
        fireCounter -= 1;
        RemoveFire();
    }

    public void IncreaseLightCounter()
    {
        lightCounter += 1;
        AddLight();
    }

    public void DecreaseLightCounter()
    {
        lightCounter -= 1;
        RemoveLight();
    }

    private void NoKeyFixer()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(LightRessourceStorage))
        {
            worldStates.SetState(LightRessourceStorage, 0);
            Debug.Log($"Ressource removed. Remaining: {worldStates.GetStates()[LightRessourceStorage]}");
        }
        if (!worldStates.HasState(FireRessourceStorage))
        {
            worldStates.SetState(FireRessourceStorage, 0);
            Debug.Log($"Building_Light added. Current count: {worldStates.GetStates()[FireRessourceStorage]}");
        }
        if (!worldStates.HasState(BuildingStorageKey))
        {
            worldStates.SetState(BuildingStorageKey, 0);
            Debug.Log($"Building_Light added. Current count: {worldStates.GetStates()[BuildingStorageKey]}");
        }

    }
}
