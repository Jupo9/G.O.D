using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Shower : MonoBehaviour
{
    public bool isAvailable = true;

    public new ParticleSystem particleSystem;

    public Animator openDoor;

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
    }

    public void StartSteam()
    {
        particleSystem.Play();
    }

    public void StopSteam()
    {
        particleSystem.Stop();
    }

    public void OpenDoorAnimation()
    {
        openDoor.Play("Shower Door Prototyp");
    }

    public void CloseDoorAnimation()
    {
        openDoor.Play("Shower Door Close Prototyp");
    }
}
