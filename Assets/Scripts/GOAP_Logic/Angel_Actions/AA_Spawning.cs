using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_Spawning : Actions
{
    public override bool PrePerform()
    {
        StartCoroutine("SpawnAngel");
        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }

    IEnumerator SpawnAngel()
    {
        agent.isStopped = true;

        yield return new WaitForSeconds(10f);

        agent.isStopped = false;
    }
}
