using UnityEngine;

public class GA_MoveToWaypoint : Actions
{
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        Debug.Log("Reached");
        return true;
    }

}
