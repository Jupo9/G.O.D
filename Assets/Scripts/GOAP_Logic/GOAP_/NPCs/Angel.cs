using UnityEngine;
using UnityEngine.AI;

public class Angel : Agents, IUnitInterface
{
    [Header("Needs")]
    public float currentFeeling = 100f;

    [Header("Believes")]
    public float spirit = 100f;
    public float social = 100f;
    public float believe = 100f;
    public float purity = 100f;

    [Header("Decay Needs")]
    public float spiritDecay = 1.0f;
    public float socialDecay = 1.0f;
    public float believeDecay = 1.0f;
    public float purityDecay = 1.0f;

    [Header("Current State")]
    public bool isAvailable = true;

    public NavMeshAgent agent;
    public GameObject lightResource;
    public GameObject revive;
    public GameObject stunEffect;

    [Header("Visual Feedback")]
    public Renderer feelingIndicatorRenderer;

    //World keys
    private const string AvialableAngelKey = "Avail_angel";
    private const string UIAvialableAngelKey = "UI_Avail_angel";

    public WorldStates localStates;

    //To choose if the unit search for farest or nearest Buildings when transport something 
    public bool preferClosest = true;
    public bool PreferClosest => preferClosest;

    private bool triggeredNeedBelowThreshold = false;

    void Awake()
    {
        localStates = new WorldStates();
    }

    protected override void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("Survive", 1, true);
        goals.Add(s1, 3);
    }

    private void Update()
    {
        if (!HasActiveTemporaryAction)
        {
            NeedLostOverTime();
        }

        CheckedNeedBelowThreshold();
        UpdateCurrentFeeling();
        UpdateFeelingVisuals();
        AngelEnds();
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

    public void TogglePreferClosest()
    {
        preferClosest = !preferClosest;
    }

    private void NeedLostOverTime()
    {
        social -= Time.deltaTime * socialDecay;
        social = Mathf.Clamp01(social);

        purity -= Time.deltaTime * purityDecay;
        purity = Mathf.Clamp01(purity);

        spirit -= Time.deltaTime * spiritDecay;
        spirit = Mathf.Clamp01(spirit);

        believe -= Time.deltaTime * believeDecay;
        believe = Mathf.Clamp01(believe);
    }

    private void CheckedNeedBelowThreshold()
    {
        if (!triggeredNeedBelowThreshold && (social < 0.5f ||
                                             purity < 0.5f ||
                                             spirit < 0.5f ||
                                             believe < 0.5f))
        {
            //Debug.Log("Angel Needs are lower than 50%, starts need behaviour attention");
            needBehaviour = true;
            triggeredNeedBelowThreshold = true;
        }

        if (triggeredNeedBelowThreshold && social >= 0.5f &&
                                           purity >= 0.5f &&
                                           spirit >= 0.5f &&
                                           believe >= 0.5f)
        {
           //Debug.Log("reset triggeredNeedBelowThreshold");
            triggeredNeedBelowThreshold = false;
        }
    }

    private void UpdateCurrentFeeling()
    {
        currentFeeling = ((spirit + social + believe + purity) / 4f) * 100f;
    }

    private void UpdateFeelingVisuals()
    {
        if (feelingIndicatorRenderer == null)
        {
            return;
        }

        Color newColor;

        if (currentFeeling >= 75)
        {
            newColor = Color.green;
        }
        else if (currentFeeling >= 50)
        {
            newColor = Color.yellow;
        }
        else if (currentFeeling >= 25)
        {
            newColor = new Color(1f, 0.5f, 0f);
        }
        else
        {
            newColor = Color.red;
        }

        newColor = Color.Lerp(Color.black, newColor, currentFeeling / 100f);

        feelingIndicatorRenderer.material.color = newColor;
    }

    private void AngelEnds()
    {
        if (social <= 0 || spirit <= 0 || purity <= 0 || believe <= 0)
        {
            Instantiate(revive, transform.position, transform.rotation);

            if (UIForCurrentNeeds.Instance != null)
            {
                UIForCurrentNeeds.Instance.ClearTarget();
            }

            if (InputManager.Instance != null)
            {
                InputManager.Instance.DeselectUnit();
            }

            Destroy(gameObject);
        }
    }

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
}

