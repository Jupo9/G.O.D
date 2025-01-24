using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_Spawning : Actions
{
    public override bool PrePerform()
    {
        StartCoroutine("SpawnDevil");
        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }

    IEnumerator SpawnDevil()
    {
        //agent.isStopped = true;

        yield return new WaitForSeconds(10f);

        //agent.isStopped = false;
    }
}
