using System.Collections.Generic;
using UnityEngine;

public class AA_PrepareAction : Actions
{
    private bool wantShower = false;

    private Angel angelScript;
    private Building_Shower buildingShower;
    private Building_Light buildingLight;

    public bool foundBuilding = false;
    public bool doneShower = false;
    public bool doneWork = false;

    private void Start()
    {
        angelScript = GetComponent<Angel>();

        if (angelScript == null)
        {
            Debug.LogWarning("Angel script not found on this GameObject.");
        }

        if (targetTag == "WO_Shower")
        {
            GameObject showerParent = GameObject.FindWithTag("Shower");

            if (showerParent != null)
            {
                buildingShower = showerParent.GetComponentInChildren<Building_Shower>();
            }

            /*if (buildingShower == null)
            {
                Debug.LogWarning("Building_Shower script not found on ShowerBuilding.");
            }*/
        }

        if (targetTag == "WO_Light")
        {
            GameObject lightParent = GameObject.FindWithTag("LIGHT");

            if (lightParent != null)
            {
                buildingLight = lightParent.GetComponentInParent<Building_Light>();
            }

            /*if (buildingLight == null)
            {
                Debug.LogWarning("Building_Light script not found on LightBuilding.");
            }*/
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shower") && wantShower)
        {
            Invoke("OpenShowerDoor", 2f);
        }

    }

    private void OpenShowerDoor()
    {
        buildingShower.OpenDoorAnimation();
    }


    public override bool PrePerform()
    {
        if (targetTag == "WO_Shower")
        {
            Dictionary<string, int> relevantState = GetRelevantAngelState();

            if (relevantState.ContainsKey("cleanShower"))
            {
                int evilValue = relevantState["cleanShower"];

                if (evilValue <= 1)
                {
                    Debug.Log("Key 'cleanShower' has value 1. Action will be skipped.");
                    doneShower = true;
                    ApplyAngelEffects();
                    return false;
                }
            }
            else
            {
                Debug.Log("PrePerform Check: Key 'cleanShower' does not exist.");
            }
        }

        if (angelScript != null)
        {
            angelScript.available = false;
        }

        if (targetTag == "WO_Shower")
        {
            wantShower = true;
        }

        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);
        if (buildings.Length == 0)
        {
            return false;
        }

        GameObject closestBuilding = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject build in buildings)
        {
            if (targetTag == "WO_Shower")
            {
                Building_Shower buildingShowerScript = build.GetComponentInParent<Building_Shower>();

                if (buildingShowerScript != null && buildingShowerScript.isAvailable)
                {
                    float distance = Vector3.Distance(this.transform.position, build.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestBuilding = build;
                    }
                }
            }

            if (targetTag == "WO_Light")
            {
                Building_Light buildingLightScript = build.GetComponentInParent<Building_Light>();

                if (buildingLightScript != null && buildingLightScript.isAvailable)
                {
                    float distance = Vector3.Distance(this.transform.position, build.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestBuilding = build;
                    }
                }
            }
        }

        if (closestBuilding == null)
        {
            return false;
        }

        target = closestBuilding;
        agent.SetDestination(target.transform.position);

        if (targetTag == "WO_Shower")
        {
            buildingShower = closestBuilding.GetComponentInParent<Building_Shower>();
            if (buildingShower == null)
            {
                Debug.LogWarning("Building_Shower script not found on the closest building.");
                return false;
            }

            buildingShower.isAvailable = false;
        }
        
        if (targetTag == "WO_Light")
        {
            buildingLight = closestBuilding.GetComponentInParent<Building_Light>();
            if (buildingLight == null)
            {
                Debug.LogWarning("Building_Light script not found on the closest building.");
                return false;
            }

            buildingLight.angelInside = true;
            buildingLight.isAvailable = false;
        }

        return true;
    }

    public override bool PostPerform()
    {
        angelScript.isStunned = true;

        if (targetTag == "WO_Shower")
        {
            ApplyAngelEffects();
            wantShower = false;
            doneShower = true;
        }
        return true;
    }
}