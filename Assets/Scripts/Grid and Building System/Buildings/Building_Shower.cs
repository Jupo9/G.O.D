using System.Collections.Generic;
using UnityEngine;

public class Building_Shower : MonoBehaviour
{
    /// <summary>
    /// The Shower building for Angel
    /// has an cost int as buidling price (requiredLight)
    /// waypoints on buildings fixed an issues, for a long time the agent didn't change the target.
    /// For this i needed a solution and thats why the buildings get always a outside and inside object as a waypoint
    /// </summary>

    public bool isAvailable = false;

    public new ParticleSystem particleSystem;

    public Animator openDoor;

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

    private const string BuildingShowerKey = "Build_shower";

    private void Awake()
    {
        CacheOriginalMaterials();
    }

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
    /// <summary>
    /// BuildingCosts is a method in many buildings. it works like that
    /// It looks for enough ressource in the scene. first for all lights in Storage, if there isn't enough then also in 
    /// the light building. The idea is that it first removed from storage and then from the working place because for the efficie 
    /// the angels and devil look first on working buildings if there are ressourcem when they need to give the GOD tribute. So that
    /// the wotking area is faster empty and can be faster reused. if both building and cost would be focus the same building first, then
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
    /// World States so every NPC can see if there are enough buildings
    /// </summary>
    public void AddBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(BuildingShowerKey))
        {
            worldStates.SetState(BuildingShowerKey, 1);
            Debug.Log($"Building_Shower added. Current count: {worldStates.GetStates()[BuildingShowerKey]}");
        }
        else
        {
            worldStates.ModifyState(BuildingShowerKey, +1);
            Debug.Log($"Building_Shower added. Current count: {worldStates.GetStates()[BuildingShowerKey]}");
        }
    }

    public void RemoveBuilding()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(BuildingShowerKey))
        {
            int currentCount = worldStates.GetStates()["Build_shower"];
            if (currentCount > 0)
            {
                worldStates.ModifyState(BuildingShowerKey, -1);
            }
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
