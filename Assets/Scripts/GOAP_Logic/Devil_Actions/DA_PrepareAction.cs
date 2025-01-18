using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DA_PrepareAction : Actions
{
    private Building_IronMaiden buildingIronMaiden;
    private Building_Fire buildingFire;

    public bool doneChill = false;
    public bool doneWork = false;
    public bool foundBuilding = false;

    private void Start()
    {
        if (targetTag == "WO_Iron")
        {
            GameObject ironParent = GameObject.FindWithTag("Iron");

            if (ironParent != null)
            {
                buildingIronMaiden = ironParent.GetComponentInChildren<Building_IronMaiden>();
            }

            /*if (buildingIronMaiden == null)
            {
                Debug.LogWarning("Building_IronMaiden script not found on IronMaidenBuilding.");
            }*/
        }

        if (targetTag == "WO_Fire")
        {
            GameObject fireParent = GameObject.FindWithTag("FIRE");

            if (fireParent != null)
            {
                buildingFire = fireParent.GetComponentInChildren<Building_Fire>();
            }

            /*if (buildingFire == null)
            {
                Debug.LogWarning("Building_Fire script not found on FireBuilding.");
            }*/
        }
    }

    public override bool PrePerform()
    {
        Dictionary<string, int> relevantState = GetRelevantDevilState();

        if (relevantState.ContainsKey("cleanChill"))
        {
            int evilValue = relevantState["cleanChill"];

            if (evilValue <= 1)
            {
                Debug.Log("Key 'cleanChill' has value 1. Action will be skipped.");
                doneChill = true;
                ApplyEffects();
                return false;
            }
        }
        else
        {
            Debug.Log("PrePerform Check in Bully: Key 'cleanChill' does not exist.");
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

            if (targetTag == "WO_Iron")
            {
                Building_IronMaiden buildingIronMaidenScript = build.GetComponentInParent<Building_IronMaiden>();
                if (buildingIronMaidenScript != null && buildingIronMaidenScript.isAvailable)
                {
                    float distance = Vector3.Distance(this.transform.position, build.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestBuilding = build;
                    }
                }
                else
                {
                    Debug.Log("No building found");
                    return false;
                }
            }

            if (targetTag == "WO_Fire")
            {
                Building_Fire buildingFireScript = build.GetComponentInParent<Building_Fire>();

                if (buildingFireScript != null && buildingFireScript.fireIsOpen)
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

        if (targetTag == "WO_Iron")
        {
            buildingIronMaiden = closestBuilding.GetComponentInParent<Building_IronMaiden>();
            if (buildingIronMaiden == null)
            {
                Debug.LogWarning("Building_IronMaiden script not found on the closest building.");
                return false;
            }

            WorldStates worldStates = Worlds.Instance.GetWorld();

            if (worldStates.HasState("Build_iron"))
            {
                int buildIronValue = worldStates.GetStates()["Build_iron"];

                if (buildIronValue == 0)
                {
                    Debug.Log("no iron found");
                    foundBuilding = false;
                }

                else if (buildIronValue >= 1)
                {
                    Debug.Log("iron found");
                    buildingIronMaiden.isAvailable = false;
                    buildingIronMaiden.RemoveBuilding();
                    foundBuilding = true;
                }
            }
        }

        if (targetTag == "WO_Fire")
        {
            buildingFire = closestBuilding.GetComponentInParent<Building_Fire>();
            if (buildingFire == null)
            {
                Debug.LogWarning("Building_Fire script not found on the closest building.");
                return false;
            }

            buildingFire.fireIsOpen = false;
            buildingFire.devilInside = true;
        }

        return true;
    }

    public override bool PostPerform()
    {
        if (targetTag == "WO_Iron")
        {
            doneChill = true;
        }

        if (targetTag == "WO_Fire")
        {
            doneWork = true;
        }

        return true;
    }

}


