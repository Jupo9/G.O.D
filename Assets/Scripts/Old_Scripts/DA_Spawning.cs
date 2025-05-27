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
            int spawnValue = relevantState["spawn"];

            if (spawnValue >= 1)
            {
                Debug.Log("Key 'spawn' has value" + spawnValue + "Action will be skipped.");
                ApplyDevilEffects();
                return false;
            }
        }
        else
        {
            Debug.Log("PrePerform Check in spawn: Key 'spawn' does not exist.");
        }

        StartCoroutine("SpawnDevil");


        duration = 7f;
        return true;
    }

    public override bool PostPerform()
    {
        ApplyDevilEffects();
        Debug.Log("PostPerformApplied");
        return true;
    }

    IEnumerator SpawnDevil()
    {
        agent.isStopped = true;

        yield return new WaitForSeconds(5f);

        agent.isStopped = false;
    }
}
