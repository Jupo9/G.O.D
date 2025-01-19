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

    private Dictionary<Renderer, Material[]> originalMaterials = new();
    public bool isPreview = false;
    private bool builded = false;

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
