using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Building_Storage : MonoBehaviour
{
    public GameObject fireRessource;
    public GameObject lightRessource;

    public float fireCounter = 0f;
    public float lightCounter = 0f;
    public float maxFireCounter = 10f;
    public float maxLightCounter = 10f;

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

    private void OnDestroy()
    {
        if (builded)
        {
            RemoveBuilding();
        }
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
