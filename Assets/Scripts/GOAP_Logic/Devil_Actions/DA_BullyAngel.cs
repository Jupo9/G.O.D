using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_BullyAngel : Actions
{
    private Devil devil;

    public bool done = false;


    private void Start()
    {
        devil = GetComponent<Devil>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Angel"))
        {
            Angel angelScript = other.GetComponent<Angel>();

            if (angelScript != null)
            {
                devil.bullyActive = true;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Angel"))
        {
            Angel angelScript = other.GetComponent<Angel>();

            if (angelScript != null)
            {
                devil.bullyActive = false;
            }
        }
    }

    public override bool PrePerform()
    {
        Dictionary<string, int> relevantState = GetRelevantState();

        if (relevantState.ContainsKey("evil"))
        {
            int evilValue = relevantState["evil"];
            Debug.Log($"PrePerform Check in Bully: Key 'evil' has value {evilValue}");

            if (evilValue == 1)
            {
                Debug.Log("Key 'evil' hat Wert 1. Aktion wird sofort beendet.");
                done = true;
                ApplyEffects(); 
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
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestAngel = angel;
            }
        }

        if (closestAngel == null)
        {
            return false;
        }

        target = closestAngel;
        agent.SetDestination(target.transform.position);

        return true;
    }

    public override bool PostPerform()
    {
        done = true;
        ApplyEffects();
        return true;
    }

}
