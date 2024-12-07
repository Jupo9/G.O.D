using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_CleanAction : Actions
{
    private Angel angelScript;
    private Building_Shower buildingShower;

    private void Start()
    {
        angelScript = GetComponent<Angel>();

        if (angelScript == null)
        {
            Debug.LogWarning("Angel script not found on this GameObject.");
        }

        GameObject showerBuilding = GameObject.FindWithTag("Shower");

        if (showerBuilding != null)
        {
            buildingShower = showerBuilding.GetComponent<Building_Shower>();
        }

        if (buildingShower == null)
        {
            Debug.LogWarning("Building_Shower script not found on ShowerBuilding.");
        }
    }

    public override bool PrePerform()
    {
        buildingShower.OpenDoorAnimation();
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

        buildingShower.CloseDoorAnimation();
    }
}
