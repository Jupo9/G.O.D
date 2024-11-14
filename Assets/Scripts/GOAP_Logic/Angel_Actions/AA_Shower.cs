using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_Shower : Actions
{

    public new ParticleSystem particleSystem;

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
