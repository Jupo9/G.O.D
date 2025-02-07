using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_TransportLogic : Actions
{
    private GOD god;

    private GameObject lightObject;

    private Angel angelScript;

    private bool transportToGOD = false;
    private bool workToStorage = false;
    private bool transportDone = false;


    public override bool PrePerform()
    {
        transportDone = false;

        angelScript = GetComponent<Angel>();

        if (angelScript == null)
        {
            Debug.LogWarning("Angel script not found on this GameObject.");
            return false;
        }

        lightObject = angelScript.lightResource;

        if (lightObject == null)
        {
            Debug.LogWarning("No Light resource found to transport.");
            return false;
        }

        if (targetTag == "Angel")
        {
            GameObject[] godObjects = GameObject.FindGameObjectsWithTag("GOD_WO");
            if (godObjects.Length == 0)
            {
                Debug.LogWarning("No GOD found.");
                return false;
            }

            god = godObjects[0].GetComponentInParent<GOD>();
            if (god == null)
            {
                Debug.LogWarning("GOD script missing on the selected target or its children.");
                return false;
            }

            if (god.wantLight > 0)
            {
                GameObject[] lightBuildings = GameObject.FindGameObjectsWithTag("LightStore");
                GameObject lightTarget = null;

                foreach (GameObject building in lightBuildings)
                {
                    Building_Light lightScript = building.GetComponentInParent<Building_Light>();
                    if (lightScript != null && lightScript.lightAmount > 0 && !lightScript.calculate)
                    {
                        lightScript.calculate = true;
                        lightTarget = building;
                        break;
                    }
                }

                if (lightTarget == null)
                {
                    GameObject[] storageBuildings = GameObject.FindGameObjectsWithTag("Store");
                    foreach (GameObject building in storageBuildings)
                    {
                        Building_Storage storageScript = building.GetComponentInParent<Building_Storage>();
                        if (storageScript != null && storageScript.lightCounter > 0)
                        {
                            lightTarget = building;
                            break;
                        }
                    }
                }

                if (lightTarget == null)
                {
                    Debug.LogWarning("No available Building_Fire or Building_Storage found.");
                    return false;
                }

                target = lightTarget;

                agent.SetDestination(target.transform.position);


                StartCoroutine("PerformTargetActionWhenArrived");

                duration = 40f;

                return true;
            }
            else
            {
                GameObject[] lightStoreBuildings = GameObject.FindGameObjectsWithTag("LightStore");
                GameObject lightStoreTarget = null;

                foreach (GameObject building in lightStoreBuildings)
                {
                    Building_Light lightStoreScript = building.GetComponentInParent<Building_Light>();
                    if (lightStoreScript != null && !lightStoreScript.fullBuilding)
                    {
                        lightStoreTarget = building;
                        break;
                    }
                }

                if (lightStoreTarget == null)
                {
                    Debug.Log("No free Building_Storage available.");
                    return false;
                }

                target = lightStoreTarget;

                agent.SetDestination(target.transform.position);

                StartCoroutine("PerformNormalTransportToStorage");

                duration = 40f;

                return true;
            }
        }
        return true;
    }

    public override bool PostPerform()
    {
        if (!transportDone)
        {
            return false;
        }

        return true;
    }

    IEnumerator PerformTargetActionWhenArrived()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.3f)
        {
            yield return null;
        }

        if (target.CompareTag("LightStore"))
        {
            Building_Light lightBuilding = target.GetComponentInParent<Building_Light>();
            if (lightBuilding != null)
            {
                lightBuilding.DecreaseLightAmount();
                lightBuilding.calculate = false;
                transportToGOD = true;
                Debug.Log("Decreased Light Amount at LightStore.");
            }
        }
        else if (target.CompareTag("Store"))
        {
            Building_Storage storageBuilding = target.GetComponentInParent<Building_Storage>();
            if (storageBuilding != null)
            {
                storageBuilding.DecreaseLightCounter();
                transportToGOD = true;
                Debug.Log("Decreased Light Counter at Store.");
            }
        }
        if (transportToGOD)
        {
            ChangeTargetAndStartNewCoroutine();
        }
    }

    IEnumerator PerformNormalTransportToStorage()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.3f)
        {
            yield return null;
        }

        if (target.CompareTag("LightStore"))
        {
            Building_Light lightBuilding = target.GetComponentInParent<Building_Light>();
            if (lightBuilding != null)
            {
                lightBuilding.DecreaseLightAmount();
                lightBuilding.calculate = false;
                workToStorage = true;
                Debug.Log("Decreased Light Amount at LightStore.");
            }
        }

        if (workToStorage)
        {
            ChangeTargetAndStartNewCoroutine();
        }
    }

    void ChangeTargetAndStartNewCoroutine()
    {
        if (transportToGOD)
        {
            transportToGOD = false;

            GameObject[] godStore = GameObject.FindGameObjectsWithTag("GOD_WO");
            GameObject newTarget = null;

            foreach (GameObject building in godStore)
            {
                GOD godScript = building.GetComponentInParent<GOD>();
                if (godScript != null)
                {
                    newTarget = building;
                    break;
                }
            }

            if (newTarget != null)
            {
                target = newTarget;
                agent.SetDestination(target.transform.position);

                StartCoroutine("TransportToTargetGOD");
            }
            else
            {
                Debug.Log("No GOD_WO target found for transport.");
            }
        }

        if (workToStorage)
        {
            workToStorage = false;

            GameObject[] transportLightToStorage = GameObject.FindGameObjectsWithTag("Store");
            GameObject newStorageTarget = null;

            foreach (GameObject building in transportLightToStorage)
            {
                Building_Storage workToStorageScript = building.GetComponentInParent<Building_Storage>();
                if (workToStorageScript != null && !workToStorageScript.fullFire)
                {
                    newStorageTarget = building;
                    break;
                }
            }

            if (newStorageTarget != null)
            {
                target = newStorageTarget;
                agent.SetDestination(target.transform.position);

                StartCoroutine("TransportToTargetStorage");
            }
            else
            {
                Debug.Log("No Store target found for transport.");
            }

        }
    }

    IEnumerator TransportToTargetGOD()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.3f)
        {
            yield return null;
        }

        if (target.CompareTag("GOD_WO"))
        {
            if (god != null)
            {
                god.IncreaseLightRessource();
                transportDone = true;
                Debug.Log("Increased Light Resource at GOD.");
            }
        }
    }

    IEnumerator TransportToTargetStorage()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.3f)
        {
            yield return null;
        }

        if (target.CompareTag("Store"))
        {
            Building_Storage storageBuilding = target.GetComponentInParent<Building_Storage>();

            if (storageBuilding != null)
            {
                storageBuilding.IncreaseLightCounter();
                transportDone = true;
                Debug.Log("Increased Light Resource at Store.");
            }
            else
            {
                Debug.LogWarning("No Building_Storage script found on target.");
            }
        }
    }
}
