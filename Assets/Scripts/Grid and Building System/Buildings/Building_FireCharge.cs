using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Building_FireCharge : MonoBehaviour
{
    [Header("NavMeshUpdate")]
    public GameObject updateParts;
    public NavMeshSurface navMeshManager;

    public bool isAvailable = true;

    [Header("Heat Particle Effect")]
    [SerializeField] private ParticleSystem heatParticles;

    [Header("Renderer & Material Slots")]
    [Tooltip("Renderer for Materials from building parts")]
    [SerializeField] private Renderer[] targetRenderers;

    [Tooltip("Which Material-Slots (in Renderer) should change")]
    [SerializeField] private int[] targetMaterialSlots;

    private List<Material> heatMaterials = new();

    [SerializeField] private Color activeColor = new Color(1f, 0.3f, 0f);
    [SerializeField] private Color inactiveColor = Color.black;

    private void Start()
    {
        NavMeshSync();
        SetupMaterialInstances();
        ResetMaterialColors();
    }

    // ------------- NavMesh Update -------------

    private void NavMeshSync()
    {
        if (navMeshManager == null)
        {
            navMeshManager = FindAnyObjectByType<NavMeshSurface>();
        }

        if (navMeshManager != null)
        {
            navMeshManager.BuildNavMesh();
        }
        else
        {
            Debug.Log("No NavMeshSurface found in the scene.");
        }
    }

    // ------------- Material Setup -------------

    private void SetupMaterialInstances()
    {
        heatMaterials.Clear();

        foreach (var renderer in targetRenderers)
        {
            if (renderer == null) continue;

            // Erstelle Materialinstanzen (automatisch durch .materials getter)
            Material[] mats = renderer.materials;

            foreach (int index in targetMaterialSlots)
            {
                if (index >= 0 && index < mats.Length && mats[index].HasProperty("_Color"))
                {
                    heatMaterials.Add(mats[index]);
                }
            }

            // Weisen die Materialien wieder zu, um Instanzen zu verwenden
            renderer.materials = mats;
        }
    }

    // ------------- Building Events -------------

    public void BuildingHeatEvents(bool isActive)
    {
        HeatMaterialChange(isActive);

        if (heatParticles != null)
        {
            if (isActive && !heatParticles.isPlaying)
            {
                heatParticles.Play();
            }
            else if (!isActive && heatParticles.isPlaying)
            {
                heatParticles.Stop();
            }
        }

        if (!isActive)
        {
            ResetMaterialColors();
        }
    }

    private void HeatMaterialChange(bool isActive)
    {
        foreach (Material mat in heatMaterials)
        {
            if (mat != null && mat.HasProperty("_Color"))
            {
                mat.color = isActive ? activeColor : inactiveColor;
            }
        }
    }

    private void ResetMaterialColors()
    {
        foreach (Material mat in heatMaterials)
        {
            if (mat != null && mat.HasProperty("_Color"))
            {
                mat.color = inactiveColor;
            }
        }
    }
}
