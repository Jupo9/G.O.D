using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : Agents
{
    public GameObject objectCanvas;

    public GameObject targetRendererObject;
    public string targetMaterialName = "Outline_1";
    private MeshRenderer targetMeshRenderer;
    private int targetMaterialIndex = -1;

    [Header("Believes")]
    public float needEvil = 100f;
    public float needChill = 100f;
    public float needJoy = 100f;
    public float needPower = 100f;

    public float decayEvil = 1.0f;
    public float decayChill = 1.0f;
    public float decayJoy = 1.0f;
    public float decayPower = 1.0f;

    public float bullyCharge = 1.0f;
    public float punshPoints = 10f;

    public GameObject fireObject;

    public bool bullyActive = false;
    public bool punshedAngel = false;

    public WorldStates localStates;

    void Awake()
    {
        localStates = new WorldStates();
    }


    protected override void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("Survive", 1, true);
        goals.Add(s1, 3);

        if (targetRendererObject == null)
        {
            Debug.LogError("Kein Zielobjekt für den MeshRenderer zugewiesen!");
            return;
        }

        targetMeshRenderer = targetRendererObject.GetComponent<MeshRenderer>();
        if (targetMeshRenderer == null)
        {
            Debug.LogError($"Kein MeshRenderer im Zielobjekt '{targetRendererObject.name}' gefunden!");
            return;
        }

        Material[] materials = targetMeshRenderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].name.Contains(targetMaterialName))
            {
                targetMaterialIndex = i;
                break;
            }
        }

        if (targetMaterialIndex == -1)
        {
            Debug.LogWarning($"Kein Material mit dem Namen '{targetMaterialName}' gefunden!");
        }

        StartCoroutine("LostOverTimeDevil");
    }

    private void Update()
    {
        if (needEvil > 100)
        {
            needEvil = 100;
        }

        if (needChill > 100)
        {
            needChill = 100;
        }

        if (needJoy > 100)
        {
            needJoy = 100;
        }

        if (needPower > 100)
        {
            needPower = 100;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    ToggleCanvas(true); 
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) 
        {
            ToggleCanvas(false); 
        }
    }

    IEnumerator LostOverTimeDevil()
    {
        while (true)
        {
            needEvil -= decayEvil;
            needChill -= decayChill;
            needJoy -= decayJoy;
            needPower -= decayPower;

            if (bullyActive)
            {
                needEvil += bullyCharge;
            }

            if (punshedAngel)
            {
                needEvil += punshPoints;
                punshedAngel = false;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void ToggleCanvas(bool state)
    {
        if (objectCanvas != null)
        {
            objectCanvas.SetActive(state);
        }

        if (targetMeshRenderer != null && targetMaterialIndex != -1)
        {
            Material[] materials = targetMeshRenderer.materials;

            if (state)
            {
                materials[targetMaterialIndex].SetFloat("_Opacity", 1.0f); 
            }
            else
            {
                materials[targetMaterialIndex].SetFloat("_Opacity", 0.0f); 
            }
            targetMeshRenderer.materials = materials;
        }
    }
}
