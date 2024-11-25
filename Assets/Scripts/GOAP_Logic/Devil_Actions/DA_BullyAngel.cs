using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_BullyAngel : Actions
{
    private Agents agentsScript;

    private void Start()
    {
        agentsScript = GetComponent<Agents>();

        if (agentsScript == null)
        {
            Debug.Log("Missing Agents Script!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Angel angelScript = other.GetComponent<Angel>();

        if (angelScript != null && angelScript.available)
        {
            agentsScript.FillEvil();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Angel"))
        {
            Angel angelScript = other.GetComponent<Angel>();

            if (angelScript != null && angelScript.available)
            {
                 agentsScript.StopFillEvil();
            }
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
            if (distance < closestDistance)
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
