using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angel : Agents
{
    [Header("Believes")]
    public float needEnjoy = 100f;
    public float needBelieve = 100f;
    public float needPower = 100f;
    public float needPurity = 100f;

    public float decayEnjoy = 1.0f;
    public float decayBelieve = 1.0f;
    public float decayPower = 1.0f;
    public float decayPurity = 1.0f;

    [Header("Status")]
    public bool available = true;
    public bool isStunned = false;

    public GameObject lightResource;

    private const string AvialableAngelKey = "Avail_angel";

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
    }

    private void OnDestroy()
    {
        RemoveAngelState();
    }

    public void AddAngelState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(AvialableAngelKey))
        {
            worldStates.SetState(AvialableAngelKey, 1);
            Debug.Log($"Angel added. Current count: {worldStates.GetStates()[AvialableAngelKey]}");
        }
        else
        {
            worldStates.ModifyState(AvialableAngelKey, 1);
            Debug.Log($"Angel added. Current count: {worldStates.GetStates()[AvialableAngelKey]}");
        }
    }

    public void RemoveAngelState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(AvialableAngelKey))
        {
            int currentCount = worldStates.GetStates()["Avail_angel"];
            worldStates.ModifyState(AvialableAngelKey, -1);
            Debug.Log($"Angel removed. Remaining: {currentCount - 1}");
        }
        else
        {
            Debug.LogWarning("Cannot remove 'Avail_angel'. State does not exist.");
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
