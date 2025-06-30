using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_Social : Actions
{
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        Debug.Log("Finished Action: " + actionName);
        return true;
    }
}
