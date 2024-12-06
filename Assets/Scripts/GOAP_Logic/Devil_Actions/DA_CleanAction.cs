using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_CleanAction : Actions
{
    public Animator openDoors;

    private void Start()
    {
        if (openDoors == null)
        {
            Transform childTransform = transform.Find("Doors");

            if (childTransform != null)
            {
                openDoors = childTransform.GetComponent<Animator>();
            }
        }
    }

    public override bool PrePerform()
    {
        openDoors.Play("DoubleDoorsBackwards");
        StartCoroutine(WaitBeforeAction());
        return false;
    }

    public override bool PostPerform()
    {
        return true;
    }

    private IEnumerator WaitBeforeAction()
    {
        yield return new WaitForSeconds(4);

        agent.isStopped = false;
        agent.SetDestination(target.transform.position);
    }
}
