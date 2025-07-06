using System.Collections;
using UnityEngine;

public class Devil : Agents, IUnitInterface
{
    [Header("Needs")]
    public float currentFeeling = 100f;

    [Header("Believes")]
    public float evil = 100f;
    public float summon = 100f;
    public float stain = 100f;
    public float heat = 100f; 

    public GameObject fireObject;

    public GameObject grave;

    //World keys
    private const string AvialableDevilKey = "Avail_devil";
    private const string UIAvialableDevilKey = "UI_Avail_devil";

    public WorldStates localStates;

    //To choose if the unit search for farest or nearest Buildings when transport something 
    public bool preferClosest = true;
    public bool PreferClosest => preferClosest;

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
        NeedLostOverTime();
        DevilEnds();
    }

    private void OnEnable()
    {
        AddDevilState();
        AddUIDevilState();
    }

    private void OnDestroy()
    {
        RemoveDevilState();
        RemoveUIDevilState();
    }

    private void NeedLostOverTime()
    {
        stain -= Time.deltaTime * 0.01f;
        stain = Mathf.Clamp01(stain);

        summon -= Time.deltaTime * 0.01f;
        summon = Mathf.Clamp01(summon);

        heat -= Time.deltaTime * 0.01f;
        heat = Mathf.Clamp01(heat);

        evil -= Time.deltaTime * 0.01f;
        evil = Mathf.Clamp01(evil);
    }

    private void DevilEnds()
    {
        if (evil <= 0 || stain <= 0 || summon <= 0 || heat <= 0)
        {
            Instantiate(grave, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

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
}
