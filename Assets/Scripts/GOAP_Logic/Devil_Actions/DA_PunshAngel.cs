using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_PunshAngel : Actions
{
    private void OnTriggerEnter(Collider other)
    {
        Angel angelScript = other.GetComponent<Angel>();
        if (angelScript != null)
        {
            angelScript.isStunned = true; 
            Debug.Log($"{angelScript.name} is now punshable by {this.name}.");
        }
    }

    public override bool PrePerform()
    {
        GameObject[] angels = GameObject.FindGameObjectsWithTag("Angel");
        if (angels.Length == 0) return false;

        GameObject closestAngel = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject angel in angels)
        {
            float distance = Vector3.Distance(this.transform.position, angel.transform.position);
            Angel angelScript = angel.GetComponent<Angel>();

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
        return true;
    }

}
