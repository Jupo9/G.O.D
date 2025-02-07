using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_Spawning : Actions
{
    public override bool PrePerform()
    {
        Dictionary<string, int> relevantState = GetRelevantDevilState();

        if (relevantState.ContainsKey("spawn"))
        {
            int angelValue = relevantState["spawn"];

            if (angelValue <= 1)
            {
                Debug.Log("Key 'spawn' has value 1. Action will be skipped.");
                ApplyDevilEffects();
                return false;
            }
        }
        else
        {
            Debug.Log("PrePerform Check in Spawn: Key 'spawn' does not exist.");
        }

        StartCoroutine("SpawnAngel");

        duration = 7f;
        return true;
    }

    public override bool PostPerform()
    {
        ApplyDevilEffects();
        return true;
    }

    IEnumerator SpawnAngel()
    {
        agent.isStopped = true;

        yield return new WaitForSeconds(5f);

        agent.isStopped = false;
    }
}
