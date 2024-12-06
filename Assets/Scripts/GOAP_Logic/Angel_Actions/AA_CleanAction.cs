using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_CleanAction : Actions
{
    public Animator openDoor;

    private Angel angelScript;

    private void Start()
    {
        if (openDoor == null)
        {
            Transform childTransform = transform.Find("ShowerDoor");

            if (childTransform != null)
            {
                openDoor = childTransform.GetComponent<Animator>();
            }
        }

        angelScript = GetComponent<Angel>();

        if (angelScript == null)
        {
            Debug.LogWarning("Angel script not found on this GameObject.");
        }
    }

    public override bool PrePerform()
    {
        openDoor.Play("Shower Door Prototyp");
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

        yield return new WaitForSeconds(2);

        openDoor.Play("Shower Door Close Prototyp");
    }
}
