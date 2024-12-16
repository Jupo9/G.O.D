using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Fire : MonoBehaviour
{
    [Header("Conditions")]
    public bool isAvailable = true;
    public bool devilInside = false;
    public bool calculate = false;

    [Header("Fire Inputs")]
    public float maxAmount = 4f;
    public float fireAmount = 0f;

    public GameObject fireResource;

    [Header("Waypoints")]
    public GameObject waypointOutside;
    public GameObject waypointInside;


    private void Update()
    {
        if (fireAmount == maxAmount)
        {
            isAvailable = false;
        }

        if (!devilInside)
        {
            if (fireAmount < maxAmount)
            {
                isAvailable = true;
            }
        }

        if (fireAmount > 0)
        {
            fireResource.SetActive(true);
        }

        if (fireAmount == 0)
        {
            fireResource.SetActive(false);
        }

    }

    public void IncreaseFireAmount()
    {
        fireAmount += 1f;
    }

    public void DecreaseFireAmount()
    {
        fireAmount -= 1f;
    }

    public void TestStandby()
    {
        if (isAvailable)
        {
            isAvailable = false;
        }
        else
        {
            isAvailable = true;
        }
    }
}
