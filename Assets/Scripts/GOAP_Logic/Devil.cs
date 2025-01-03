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

    public float bullyCharge = 1.0f;
    public float punshPoints = 10f;

    public GameObject fireObject;

    public bool bullyActive = false;
    public bool punshedAngel = false;

    public WorldStates localStates;

    void Awake()
    {
        localStates = new WorldStates();
    }


    protected override void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("Survive", 1, true);
        goals.Add(s1, 3);

        StartCoroutine("LostOverTimeDevil");
    }

    private void Update()
    {
        if (needEvil > 100)
        {
            needEvil = 100;
        }

        if (needChill > 100)
        {
            needChill = 100;
        }

        if (needJoy > 100)
        {
            needJoy = 100;
        }

        if (needPower > 100)
        {
            needPower = 100;
        }
    }

    IEnumerator LostOverTimeDevil()
    {
        while (true)
        {
            needEvil -= decayEvil;
            needChill -= decayChill;
            needJoy -= decayJoy;
            needPower -= decayPower;

            if (bullyActive)
            {
                needEvil += bullyCharge;
            }

            if (punshedAngel)
            {
                needEvil += punshPoints;
                punshedAngel = false;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
