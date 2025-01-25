using System.Collections.Generic;
using UnityEngine;

public class Building_Light : MonoBehaviour
{
    //the buildings that stores or produce ressources have a lot of Coniditions, if statements and they kinda overwhelming. 
    //they need a enum, for the if's as soon as the actions work probably
    //many of the thinks from the shower building is repeating here
    [Header("Conditions")]
    public bool isAvailable = false;
    public bool angelInside = false;
    public bool calculate = false;
    public bool empty = false;
    public bool addOne = false;
    public bool minusOne = false;

    public bool fullBuilding = false;

    [Header("Light Inputs")]
    public int maxAmount = 4;
    public int lightAmount = 0;

    public GameObject lightResource;

    [Header("Waypoints")]
    public GameObject waypointOutside;
    public GameObject waypointInside;

    // startBuilding is for Building that exist in the scene when the game starts, so basically start buildings, set bool = true only for them!
    [Tooltip("This bool needs to be true if the buidling exist in the scene before the game starts!")]
    public bool startBuilding = false;

    private Angel assignedAngel;

    private Dictionary<Renderer, Material[]> originalMaterials = new();
    public bool isPreview = false;
    private bool builded = false;

    public int requiredLight = 0;

    private const string BuildingLightKey = "Build_light";
    private const string LightRessource = "Res_light";

    private void Awake()
    {
        CacheOriginalMaterials();
    }

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
            isAvailable = false;
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
                isAvailable = true;
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
        lightAmount += 1;
        AddLight();

    }

    public void DecreaseLightAmount()
    {
        lightAmount -= 1;
        RemoveLight();
    }

    //this fixed an error mesage that cames when a state falls to 0, the error message self was not a problem
    //but because of the World States it cames to strange interaction where two npc didn't find any building or go together in one
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
