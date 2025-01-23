using System.Collections;
using UnityEngine;

public class Angel : Agents
{
    [Header("UI")]
    public GameObject objectCanvas;
    public GameObject targetRendererObject;
    public GameObject targetBuildUI;
    public GameObject targetNeedUI;
    public string targetMaterialName = "Outline_1";

    public GameObject targetUIForBuildings;
    public GameObject targetUIForNeeds;
    public GameObject targetUIForTransport;

    private MeshRenderer targetMeshRenderer;
    private int targetMaterialIndex = -1;

    [Header("Believes")]
    public float needEnjoy = 100f;
    public float needBelieve = 100f;
    public float needPower = 100f;
    public float needPurity = 100f;

    [Header("Decays")]
    public float decayEnjoy = 1.0f;
    public float decayBelieve = 1.0f;
    public float decayPower = 1.0f;
    public float decayPurity = 1.0f;

    [Header("Charge Power")]
    public float purityCharge = 1f;

    [Header("Current State")]
    public bool available = true;
    public bool isStunned = false;
    public bool isPurity = false;

    public GameObject lightResource;

    public WorldStates localStates;

    private static Angel activeAngel;

    [Header("BuildingStates")]
    public bool isBuilding = false;
    public bool choosenOne = false;
    public bool buildingAction = false;

    private bool checkAction = false;

    private const string AvialableAngelKey = "Avail_angel"; 
    private const string UIAvialableAngelKey = "UI_Avail_angel";

    void Awake()
    {
        localStates = new WorldStates();
    }

    protected override void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("Survive", 1, true);
        goals.Add(s1, 3);

        StartCoroutine("LostOverTime");

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
        if (needEnjoy > 100)
        {
            needEnjoy = 100;
        }

        if (needBelieve > 100)
        {
            needBelieve = 100;
        }

        if (needPower > 100)
        {
            needPower = 100;
        }

        if (needPurity > 100)
        {
            needPurity = 100;
        }

        if (Input.GetMouseButtonDown(0) && !buildingAction)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    SetActiveAngel(this);
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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Devil") && isStunned)
        {
            Invoke("PrepareStun", 2f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Devil"))
        {
            available = true;
        }

    }
    private void OnEnable()
    {
        AddAngelState();
        AddUIAngelState();
    }

    private void OnDestroy()
    {
        RemoveAngelState();
        RemoveUIAngelState();
    }

    public void ActivateUIForBuildings()
    {
        ActivateUIElement(targetUIForBuildings);
    }

    public void ActivateUIForNeeds()
    {
        ActivateUIElement(targetUIForNeeds);
    }

    public void ActivateUIForTransport()
    {
        ActivateUIElement(targetUIForTransport);
    }

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
        if (targetUIForTransport != null)
        {
            targetUIForTransport.SetActive(false);
        }
    }

    public static void SetActiveAngel(Angel newActiveAngel)
    {
        if (activeAngel != null && activeAngel != newActiveAngel)
        {
            activeAngel.Deactivate();
        }

        activeAngel = newActiveAngel;
        newActiveAngel.Activate();
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

    private void Deactivate()
    {
        choosenOne = false;

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

    /// <summary>
    /// UI States are there for the visibility for the Player, because the other State will be removed when the Angel is not available
    /// So the UI shows always the current count of Angels and the normal Angel State is hidden for the player and only important for the System
    /// </summary>

    public void AddAngelState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(AvialableAngelKey))
        {
            worldStates.SetState(AvialableAngelKey, 1);
        }
        else
        {
            worldStates.ModifyState(AvialableAngelKey, +1);
        }
    }

    public void RemoveAngelState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(AvialableAngelKey))
        {
            int currentCount = worldStates.GetStates()["Avail_angel"];
            if (currentCount > 0) 
            {
                worldStates.ModifyState(AvialableAngelKey, -1);
            }
        }
    }

    public void AddUIAngelState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(UIAvialableAngelKey))
        {
            worldStates.SetState(UIAvialableAngelKey, 1);
        }
        else
        {
            worldStates.ModifyState(UIAvialableAngelKey, +1);
        }
    }

    public void RemoveUIAngelState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(UIAvialableAngelKey))
        {
            int currentCount = worldStates.GetStates()["UI_Avail_angel"];
            if (currentCount > 0)
            {
                worldStates.ModifyState(UIAvialableAngelKey, -1);
            }
        }
    }

    IEnumerator LostOverTime()
    {
        while (true) 
        {
            needEnjoy -= decayEnjoy;
            needBelieve -= decayBelieve;
            needPower -= decayPower;
            needPurity -= decayPurity;

            if (isPurity)
            {
                needPurity += purityCharge;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void PrepareStun()
    {
        available = false;
        isStunned = false;
        StartCoroutine("StunAngel");
    }

    IEnumerator StunAngel()
    {
        while (!available)
        {
            if (currentAction != null)
            {
                currentAction.agent.isStopped = true;
            }

            yield return new WaitForSeconds(10f);
            available = true;
        }

        if (currentAction != null)
        {
            currentAction.agent.isStopped = false;
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
}

