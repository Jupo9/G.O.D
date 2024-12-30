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
        Angel angelScript = other.GetComponent<Angel>();

        if (angelScript != null && angelScript.available)
        {
            devil.bullyActive = true; 
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
        Dictionary<string, int> worldStates = Worlds.Instance.GetWorld().GetStates();

        if (worldStates.ContainsKey("evil") && worldStates["evil"] == 1)
        {
            Debug.Log("DA_BullyAngel: 'evil' is already 1. Marking action as complete.");
            ApplyEffects();
            done = true;
            return false; 
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
        Worlds.Instance.GetWorld().SetState("evil", 1);
        done = true;
        return true;
    }

}
