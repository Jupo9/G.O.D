using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class Building_LightCharge : MonoBehaviour
{
    [Header("NavMeshUpdate")]
    public GameObject updateParts;
    public NavMeshSurface navMeshManager;

    public bool isAvailable = true;

    [Header("Spirit Particle Effect")]
    [SerializeField] private ParticleSystem spiritParticles;

    [Header("Renderer & Material Slots")]
    [Tooltip("Renderer for Materials from building parts")]
    [SerializeField] private Renderer[] targetRenderers;

    [Tooltip("Which Material-Slots (in Renderer) should change")]
    [SerializeField] private int[] materialIndices;

    private Coroutine colorCoroutine;
    private Color[] colors = new Color[] { Color.white, Color.yellow, Color.cyan};

    private void Start()
    {
        NavMeshSync();
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
            Debug.LogWarning("No NavMeshSurface found in the scene.");
        }
    }

    // ------------- Building Events -------------

    public void BuildingSpiritEvents(bool isActive)
    {
        if (isActive)
        {
            EnsureMaterialInstances();
            if (colorCoroutine == null)
            {
                colorCoroutine = StartCoroutine(ColorPulseEffect());
            }
            if (spiritParticles != null && !spiritParticles.isPlaying)
            {
                spiritParticles.Play();
            }
        }
        else
        {
            if (colorCoroutine != null)
            {
                StopCoroutine(colorCoroutine);
                colorCoroutine = null;
                ResetMaterialColors();
            }

            if (spiritParticles != null && spiritParticles.isPlaying)
            {
                spiritParticles.Stop();
            }
        }
    }

    // ---------- Material Instance ----------

    private void EnsureMaterialInstances()
    {
        foreach (var renderer in targetRenderers)
        {
            Material[] mats = renderer.materials; 
            renderer.materials = mats;
        }
    }

    // ---------- Color Changes ----------

    private IEnumerator ColorPulseEffect()
    {
        int index = 0;
        while (true)
        {
            foreach (var renderer in targetRenderers)
            {
                var mats = renderer.materials;

                foreach (int matIndex in materialIndices)
                {
                    if (matIndex >= 0 && matIndex < mats.Length && mats[matIndex].HasProperty("_Color"))
                    {
                        mats[matIndex].color = colors[index];
                    }
                }
            }

            index = (index + 1) % colors.Length;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // ---------- Reset ----------

    private void ResetMaterialColors()
    {
        foreach (var renderer in targetRenderers)
        {
            var mats = renderer.materials;
            foreach (int matIndex in materialIndices)
            {
                if (matIndex >= 0 && matIndex < mats.Length && mats[matIndex].HasProperty("_Color"))
                {
                    mats[matIndex].color = Color.black;
                }
            }
        }
    }
}

