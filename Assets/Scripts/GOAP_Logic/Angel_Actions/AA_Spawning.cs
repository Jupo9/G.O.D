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
            Debug.Log("PrePerform Check in Bully: Key 'spawn' does not exist.");
        }

        StartCoroutine("SpawnAngel");
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

        yield return new WaitForSeconds(10f);

        agent.isStopped = false;
    }
}
