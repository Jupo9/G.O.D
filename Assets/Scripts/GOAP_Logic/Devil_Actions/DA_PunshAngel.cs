using System.Collections.Generic;
using UnityEngine;

public class DA_PunshAngel : Actions
{
    private Devil devil;

    public bool done = false;

    private void Start()
    {
        devil = GetComponent<Devil>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Angel angelScript = other.GetComponent<Angel>();
        if (angelScript != null)
        {
            angelScript.isStunned = true;
            devil.punshedAngel = true;
            Debug.Log($"{angelScript.name} is now punshable by {this.name}.");
        }
    }

    public override bool PrePerform()
    {
        Dictionary<string, int> relevantState = GetRelevantDevilState();

        if (relevantState.ContainsKey("evil"))
        {
            int evilValue = relevantState["evil"];

            if (evilValue <= 1)
            {
                Debug.Log("Key 'evil' has value 1. Action will be skipped.");
                done = true;
                ApplyDevilEffects();
                return false;
            }
        }
        else
        {
            Debug.Log("PrePerform Check in Bully: Key 'evil' does not exist.");
        }

        GameObject[] angels = GameObject.FindGameObjectsWithTag("Angel");
        if (angels.Length == 0) return false;

        GameObject closestAngel = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject angel in angels)
        {
            float distance = Vector3.Distance(this.transform.position, angel.transform.position);
            Angel angelScript = angel.GetComponent<Angel>();

            //Implement angelScript Remove and ADD functions!

            if (distance < closestDistance && angelScript != null && !angelScript.isStunned)
            {
                closestDistance = distance;
                closestAngel = angel;
            }
        }

        if (closestAngel == null) return false;

        target = closestAngel;
        agent.SetDestination(target.transform.position);
        return true;
    }

    public override bool PostPerform()
    {
        done = true;
        ApplyDevilEffects();
        return true;
    }

}
