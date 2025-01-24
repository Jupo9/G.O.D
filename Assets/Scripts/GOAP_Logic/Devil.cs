using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Devil : Agents
{
    [Header("UI")]
    public GameObject objectCanvas;
    public GameObject targetRendererObject;
    public GameObject targetBuildUI;
    public GameObject targetNeedUI;
    public string targetMaterialName = "Outline_1";

    public GameObject targetUIForBuildings;
    public GameObject targetUIForNeeds;
    //public GameObject targetUIForTransport;

    private MeshRenderer targetMeshRenderer;
    private int targetMaterialIndex = -1;

    [Header("Believes")]
    public float needEvil = 100f;
    public float needChill = 100f;
    public float needJoy = 100f;
    public float needPower = 100f;

    [Header("Decays")]
    public float decayEvil = 1.0f;
    public float decayChill = 1.0f;
    public float decayJoy = 1.0f;
    public float decayPower = 1.0f;

    [Header("Charge Power")]
    public float bullyCharge = 1.0f;
    public float chillCharge = 1.0f;
    public float punshPoints = 10f;

    [Header("Current State")]
    public bool bullyActive = false;
    public bool punshedAngel = false;
    public bool isChilled = false;

    public GameObject fireObject;

    public WorldStates localStates;

    [Header("SubGoalsBools")]
    public bool isWorking = false;
    public bool isTransporting = false;
    public bool isBuilding = false;

    [Header("BuildingStates")]
    public bool choosenOne = false;
    public bool buildingAction = false;

    private bool checkAction = false;

    private static Devil activeDevil;

    public GameObject grave;

    private const string AvialableDevilKey = "Avail_devil";
    private const string UIAvialableDevilKey = "UI_Avail_devil";

    void Awake()
    {
        localStates = new WorldStates();
    }

    protected override void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("Survive", 1, true);
        goals.Add(s1, 3);

        StartCoroutine("LostOverTimeDevil");

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

        if (needEvil <= 0 || needChill <= 0 || needJoy <= 0 || needPower <= 0)
        {
            Instantiate(grave, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        if (Input.GetMouseButtonDown(0) && !buildingAction)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    SetActiveDevil(this);
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && !buildingAction) 
        {
            if (!isBuilding) 
            {
                targetBuildUI.SetActive(false);
            }

            targetNeedUI.SetActive(false);

            ToggleCanvas(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !buildingAction)
        {
            DisableUI();
        }

        if (buildingAction && !checkAction)
        {
            checkAction = true;
            DisableUI();
            ToggleCanvas(false);
        }
    }

    private void OnEnable()
    {
        AddDevilState();
        AddUIDevilState();
        AssignUIReferences();
    }

    private void OnDestroy()
    {
        RemoveDevilState();
        RemoveUIDevilState();
    }

    public void ActivateUIForBuildings()
    {
        ActivateUIElement(targetUIForBuildings);
    }

    public void ActivateUIForNeeds()
    {
        ActivateUIElement(targetUIForNeeds);
    }

    /*public void ActivateUIForTransport()
    {
        ActivateUIElement(targetUIForTransport);
    }*/

    private void ActivateUIElement(GameObject uiElement)
    {
        if (uiElement == null)
        {
            Debug.LogError("UI-Element ist null.");
            return;
        }

        DeactivateAllUIElements();

        uiElement.SetActive(true);
    }

    private void DeactivateAllUIElements()
    {
        if (targetUIForBuildings != null)
        {
            targetUIForBuildings.SetActive(false);
        }
        if (targetUIForNeeds != null)
        {
            targetUIForNeeds.SetActive(false);
        }
        /*if (targetUIForTransport != null)
        {
            targetUIForTransport.SetActive(false);
        }*/
    }

    /// <summary>
    /// Activate and Deactivate choosen Devil UI and Shader, so that only one Devil can be selected 
    /// </summary>

    public static void SetActiveDevil(Devil newActiveDevil)
    {
        if (activeDevil != null && activeDevil != newActiveDevil)
        {
            activeDevil.Deactivate();
        }

        foreach (Angel angel in FindObjectsByType<Angel>(FindObjectsSortMode.None))
        {
            angel.Deactivate();
        }

        activeDevil = newActiveDevil; 
        newActiveDevil.Activate(); 
    }

    private void Activate()
    {
        choosenOne = true;

        if (targetMeshRenderer != null && targetMaterialIndex != -1)
        {
            Material[] materials = targetMeshRenderer.materials;
            materials[targetMaterialIndex].SetFloat("_Opacity", 1.0f);
            targetMeshRenderer.materials = materials;
        }

        ToggleCanvas(true);

        Debug.Log($"{name} is active.");
    }

    public void Deactivate()
    {
        choosenOne = false;
        DisableUI();

        if (targetMeshRenderer != null && targetMaterialIndex != -1)
        {
            Material[] materials = targetMeshRenderer.materials;
            materials[targetMaterialIndex].SetFloat("_Opacity", 0.0f); 
            targetMeshRenderer.materials = materials;
        }

        ToggleCanvas(false);

        Debug.Log($"{name} was deactivate.");
    }

    private void DisableUI()
    {
        targetBuildUI.SetActive(false);
        targetNeedUI.SetActive(false);
    }

    public void AssignUIReferences()
    {
        GameObject canvas = GameObject.Find("MainCanvas");
        if (canvas != null)
        {

            targetBuildUI = canvas.transform.Find("DevilBuidlings").gameObject;
            targetNeedUI = canvas.transform.Find("ShowDevilUINeeds").gameObject;
            targetUIForBuildings = canvas.transform.Find("DevilBuidlings").gameObject;
            targetUIForNeeds = canvas.transform.Find("ShowDevilUINeeds").gameObject;
            //targetUIForTransport = canvas.transform.Find("Timer").gameObject;
        }
        else
        {
            Debug.LogError("Canvas nicht gefunden!");
        }
    }


    /// <summary>
    /// UI States are there for the visibility for the Player, because the other State will be removed when the Devil is not available
    /// So the UI shows always the current count of Devils and the normal Devil State is hidden for the player and only important for the System
    /// </summary>

    public void AddDevilState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(AvialableDevilKey))
        {
            worldStates.SetState(AvialableDevilKey, 1);
            Debug.Log($"Angel added. Current count: {worldStates.GetStates()[AvialableDevilKey]}");
        }
        else
        {
            worldStates.ModifyState(AvialableDevilKey, +1);
            Debug.Log($"Angel added. Current count: {worldStates.GetStates()[AvialableDevilKey]}");
        }
    }

    public void RemoveDevilState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(AvialableDevilKey))
        {
            int currentCount = worldStates.GetStates()["Avail_devil"];
            if (currentCount > 0)
            {
                worldStates.ModifyState(AvialableDevilKey, -1);
            }
        }

    }

    public void AddUIDevilState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(UIAvialableDevilKey))
        {
            worldStates.SetState(UIAvialableDevilKey, 1);
            Debug.Log($"Angel added. Current count: {worldStates.GetStates()[UIAvialableDevilKey]}");
        }
        else
        {
            worldStates.ModifyState(UIAvialableDevilKey, +1);
            Debug.Log($"Angel added. Current count: {worldStates.GetStates()[UIAvialableDevilKey]}");
        }
    }

    public void RemoveUIDevilState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(UIAvialableDevilKey))
        {
            int currentCount = worldStates.GetStates()["UI_Avail_devil"];
            if (currentCount > 0)
            {
                worldStates.ModifyState(UIAvialableDevilKey, -1);
            }
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

            if (isChilled) 
            {
                needChill += chillCharge;
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

    public void StartBuilding()
    {
        isBuilding = true;
    }

    public void StartWorking()
    {
        isWorking = true;
    }

    public void StartTransporting()
    {
        isTransporting = true;
    }
}
