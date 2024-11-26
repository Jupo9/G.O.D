using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angel : Agents
{
    public bool available = true;

    protected override void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("Survive", 1, true);
        goals.Add(s1, 3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Devil"))
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
    
    private void PrepareStun()
    {
        available = false;
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
