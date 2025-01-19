using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_IronMaiden : MonoBehaviour
{
    public bool isAvailable = true;

    public Animator doubleDoors;

    public GameObject waypointOutside;
    public GameObject waypointInside;

    private Dictionary<Renderer, Material[]> originalMaterials = new();
    public bool isPreview = false;
    private bool builded = false;

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
