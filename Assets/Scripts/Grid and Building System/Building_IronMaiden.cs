using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_IronMaiden : MonoBehaviour
{
    public bool isAvailable = true;

    public Animator doubleDoors;

    public GameObject waypointOutside;
    public GameObject waypointInside;

    private void Start()
    {
        if (doubleDoors == null)
        {
            Transform childTransfrom = transform.Find("Doors");

            if (childTransfrom != null)
            {
                doubleDoors = childTransfrom.GetComponent<Animator>();
            }
        }
    }

    public void OpenDoubleDoors()
    {
        doubleDoors.Play("DoubleDoorsBackwards");
    }

    public void CloseDoubleDoors()
    {
        doubleDoors.Play("DoubleDoors");
    }
}
