using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Light : MonoBehaviour
{
    [Header("Conditions")]
    public bool isAvailable = true;
    public bool angelInside = false;
    public bool calculate = false;

    [Header("Light Inputs")]
    public float maxAmount = 4f;
    public float lightAmount = 0f;

    public GameObject lightResource;

    [Header("Waypoints")]
    public GameObject waypointOutside;
    public GameObject waypointInside;


    private void Update()
    {
        if (lightAmount == maxAmount)
        {
            isAvailable = false;
        }

        if (!angelInside)
        {
            if (lightAmount < maxAmount)
            {
                isAvailable = true;
            }
        }

        if(lightAmount > 0)
        {
            lightResource.SetActive(true);
        }

        if(lightAmount == 0)
        {
            lightResource.SetActive(false);
        }

    }

    public void IncreaseLightAmount()
    {
        lightAmount += 1f;
    }

    public void DecreaseLightAmount()
    {
        lightAmount -= 1f;
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
