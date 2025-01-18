using System.Collections;
using UnityEngine;

public class Angel : Agents
{
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

    private const string AvialableAngelKey = "Avail_angel"; 
    private const string UIAvialableAngelKey = "UI_Avail_angel";

    protected override void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("Survive", 1, true);
        goals.Add(s1, 3);

        StartCoroutine("LostOverTime");
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
}
