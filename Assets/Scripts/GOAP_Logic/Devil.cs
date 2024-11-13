using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : Agents
{

    protected override void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("Survive", 1, true);
        goals.Add(s1, 3);
    }
}
