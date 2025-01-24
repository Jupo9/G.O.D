using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DA_Spawning : Actions
{
    public override bool PrePerform()
    {
        Dictionary<string, int> relevantState = GetRelevantDevilState();

        if (relevantState.ContainsKey("spawn"))
        {
            int evilValue = relevantState["spawn"];

            if (evilValue <= 1)
            {
                Debug.Log("Key 'spawn' has value 1. Action will be skipped.");
                ApplyDevilEffects();
                return false;
            }
        }
        else
        {
            Debug.Log("PrePerform Check in Bully: Key 'spawn' does not exist.");
        }

        StartCoroutine("SpawnDevil");
        return true;
    }

    public override bool PostPerform()
    {
        ApplyDevilEffects();
        return true;
    }

    IEnumerator SpawnDevil()
    {
        agent.isStopped = true;

        yield return new WaitForSeconds(10f);

        agent.isStopped = false;
    }
}
