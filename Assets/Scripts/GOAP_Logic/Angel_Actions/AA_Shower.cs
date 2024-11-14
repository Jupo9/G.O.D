using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_Shower : Actions
{

    public new ParticleSystem particleSystem;


    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shower"))
        {
            particleSystem.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Shower"))
        {
            particleSystem.Stop();
        }
    }*/


    public override bool PrePerform()
    {
        particleSystem.Play();
        return true;
    }

    public override bool PostPerform()
    {
        particleSystem.Stop();
        return true;
    }

}
