using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingState : MonoBehaviour
{
    public bool isFree = true;

    public void EnterBuilding()
    {
        isFree = false;
    }

    public void ExitBuilding()
    {
        isFree = true;
    }
}
