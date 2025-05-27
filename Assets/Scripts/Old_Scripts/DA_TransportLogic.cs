using System.Collections;
using UnityEngine;

public class DA_TransportLogic : Actions
{
    private GOD god;

    private GameObject fireResource;

    private Devil devilScript;

    private bool transportToGOD = false;
    private bool workToStorage = false;
    private bool transportDone = false;


    public override bool PrePerform()
    {
        transportDone = false;

        devilScript = GetComponent<Devil>();

        if (devilScript == null)
        {
            Debug.LogWarning("Devil script not found on this GameObject.");
            return false;
        }

        fireResource = devilScript.fireObject;

        if (fireResource == null)
        {
            Debug.LogWarning("No FIRE resource found to transport.");
            return false;
        }

        if (targetTag == "Devil")
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

            if (god.wantFire > 0)
            {
                GameObject[] fireBuildings = GameObject.FindGameObjectsWithTag("FireStore");
                GameObject fireTarget = null;

                foreach (GameObject building in fireBuildings)
                {
                    Building_Fire fireScript = building.GetComponentInParent<Building_Fire>();
                    if (fireScript != null && fireScript.fireAmount > 0 && !fireScript.calculate)
                    {
                        fireScript.calculate = true;
                        fireTarget = building;
                        break;
                    }
                }

                if (fireTarget == null)
                {
                    GameObject[] storageBuildings = GameObject.FindGameObjectsWithTag("Store");
                    foreach (GameObject building in storageBuildings)
                    {
                        Building_Storage storageScript = building.GetComponentInParent<Building_Storage>();
                        if (storageScript != null && storageScript.fireCounter > 0)
                        {
                            fireTarget = building;
                            break;
                        }
                    }
                }

                if (fireTarget == null)
                {
                    Debug.LogWarning("No available Building_Fire or Building_Storage found.");
                    return false;
                }

                target = fireTarget;
                agent.SetDestination(target.transform.position);

                StartCoroutine("PerformTargetActionWhenArrived");

                duration = 40f;

                return true;
            }
            else
            {
                GameObject[] fireStoreBuildings = GameObject.FindGameObjectsWithTag("FireStore");
                GameObject fireStoreTarget = null;

                foreach (GameObject building in fireStoreBuildings)
                {
                    Building_Fire fireStoreScript = building.GetComponentInParent<Building_Fire>();
                    if (fireStoreScript != null && !fireStoreScript.fullBuilding)
                    {
                        fireStoreTarget = building;
                        break;
                    }
                }

                if (fireStoreTarget == null)
                {
                    Debug.Log("No free Building_Storage available.");
                    return false;
                }

                target = fireStoreTarget;
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

        Debug.Log("finishedTransport");
        return true;
    }

    IEnumerator PerformTargetActionWhenArrived()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.3f)
        {
            yield return null; 
        }

        if (target.CompareTag("FireStore"))
        {
            Building_Fire fireBuilding = target.GetComponentInParent<Building_Fire>();
            if (fireBuilding != null)
            {
                fireBuilding.DecreaseFireAmount();
                fireBuilding.calculate = false;
                transportToGOD = true;
                Debug.Log("Decreased Fire Amount at FireStore.");
            }
        }
        else if (target.CompareTag("Store"))
        {
            Building_Storage storageBuilding = target.GetComponentInParent<Building_Storage>();
            if (storageBuilding != null)
            {
                storageBuilding.DecreaseFireCounter();
                transportToGOD = true;
                Debug.Log("Decreased Fire Counter at Store.");  
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

        if (target.CompareTag("FireStore"))
        {
            Building_Fire fireBuilding = target.GetComponentInParent<Building_Fire>();
            if (fireBuilding != null)
            {
                fireBuilding.DecreaseFireAmount();
                fireBuilding.calculate = false;
                workToStorage = true;
                Debug.Log("Decreased Fire Amount at FireStore.");
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

            GameObject[] transportFireToStorage = GameObject.FindGameObjectsWithTag("Store");
            GameObject newStorageTarget = null;

            foreach (GameObject building in transportFireToStorage)
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
                god.IncreaseFireRessource();
                transportDone = true;
                Debug.Log("Increased Fire Resource at GOD.");
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
                storageBuilding.IncreaseFireCounter();
                transportDone = true;
                Debug.Log("Increased Fire Resource at Store.");
            }
            else
            {
                Debug.LogWarning("No Building_Storage script found on target.");
            }
        }
    }
}
