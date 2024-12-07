using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_Shower : Actions
{
    private Building_Shower buildingShower;

    private void Start()
    {
        GameObject showerParent = GameObject.FindWithTag("Shower");

        if (showerParent != null)
        {
            buildingShower = showerParent.GetComponentInChildren<Building_Shower>();
        }

        if (buildingShower == null)
        {
            Debug.LogWarning("Building_Shower script not found on ShowerBuilding.");
        }
    }

    public override bool PrePerform()
    {
        Invoke("CloseDoor", 2f);
        return true;
    }

    private void CloseDoor()
    {
        buildingShower.CloseDoorAnimation();
        buildingShower.StartSteam();
    }

    public override bool PostPerform()
    {
        buildingShower.StopSteam();
        return true;
    }

}
