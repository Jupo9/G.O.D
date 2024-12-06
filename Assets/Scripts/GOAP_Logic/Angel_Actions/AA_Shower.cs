using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_Shower : Actions
{

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

    public override bool PrePerform()
    {
        Invoke("CloseDoor", 2f);
        return true;
    }

    private void CloseDoor()
    {
        openDoor.Play("Shower Door Close Prototyp");
        particleSystem.Play();
    }

    public override bool PostPerform()
    {
        particleSystem.Stop();
        return true;
    }

}
