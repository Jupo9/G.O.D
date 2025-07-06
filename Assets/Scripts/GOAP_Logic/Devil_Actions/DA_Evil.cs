using UnityEngine;

public class DA_Evil : Actions
{
    public override bool PrePerform()
    {
        FinishAction();
        return true;
    }

    public override bool PostPerform()
    {
        Debug.Log("Finished Action: " + actionName);
        return true;
    }
}
