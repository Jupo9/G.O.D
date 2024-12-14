using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : Agents
{
    [Header("Believes")]
    public float needEvil = 100f;
    public float needChill = 100f;
    public float needJoy = 100f;
    public float needPower = 100f;

    public float decayEvil = 1.0f;
    public float decayChill = 1.0f;
    public float decayJoy = 1.0f;
    public float decayPower = 1.0f;

    protected override void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("Survive", 1, true);
        goals.Add(s1, 3);

        StartCoroutine("LostOverTimeDevil");
    }

    IEnumerator LostOverTimeDevil()
    {
        needEvil -= decayEvil;
        needChill -= decayChill;
        needJoy -= decayJoy;
        needPower -= decayPower;

        yield return new WaitForSeconds(1f);
    }
}
