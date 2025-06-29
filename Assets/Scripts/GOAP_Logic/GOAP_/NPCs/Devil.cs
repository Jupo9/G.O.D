using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : Agents, IUnitInterface
{
    [Header("Needs")]
    public float currentFeeling = 100f;

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

    public GameObject grave;

    private const string AvialableDevilKey = "Avail_devil";
    private const string UIAvialableDevilKey = "UI_Avail_devil";

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

        if (needEvil <= 0 || needChill <= 0 || needJoy <= 0 || needPower <= 0)
        {
            Instantiate(grave, transform.position, transform.rotation);
            Destroy(gameObject);
        }
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
}
