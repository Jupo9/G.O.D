using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GA_TransportLogic : Actions
{
    /// <summary>
    /// THIS SCRIPT CONTAINS:
    /// Transport Resource to God or Storage by finding resources in Work - Buildings or Storages
    /// Enums for Units and Transport steps
    /// Check God needs and choose transport route
    /// methode to choose between closest or farest target
    /// Collect correct resource and show while transport resource
    /// </summary>

    [Header("TargetDistance")]
    [SerializeField] private float targetDistance = 2.0f;

    //Important for Buidling and Ressource finding
    private GOD god;
    private bool godNeedsResource = false;

    private string storeTag;
    private string workingTag;
    private string ressourceName;

    private GameObject checkoutTarget;

    // Interface for buildings
    private IResourceManager resourceManager = null;
    private string lockedRessourceType = "";

    private GameObject finalTarget;

    private bool isCarryingResource = false;

    private Coroutine currentTransportCoroutine;

    // ------------- Enums ------------- 

    //enum is a list/enumeration type, it is used to understand which unit is selected
    enum UnitType
    {
        Angel,
        Devil
    }

    private UnitType unitType;

    private enum TransportPhase
    {
        ToPickup,
        ToDelivery,
        ToCheckout
    }

    private TransportPhase currentPhase = TransportPhase.ToPickup;

    // ------------- Debug testing ------------- 

    /*void Update()
    {
        
        Debug.DrawLine(transform.position, target != null ? target.transform.position : transform.position, Color.yellow);

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            Debug.Log($"Aktuelle Entfernung zu Target: {distance:F2}");
        }
    }*/

    // ------------- PrePerform ------------- 

    public override bool PrePerform()
    {
        DetermineUnitType();
        SetupTagsAndVariables();
        FindGOD();

        godNeedsResource = GODNeedsResource();

        GameObject targetBuilding = null;

        if (godNeedsResource)
        {
            //FindTargetBuilding is an GameObject bool which true stands for Work_Building and false for Storage
            //If no available Work_Building exist, it will search for a Storage. 

            //Debug.Log("GOD needs " + ressourceName);
            targetBuilding = FindTargetBuilding(true);

            if (targetBuilding == null)
            {
                Debug.LogWarning("No suitable Work_Building, search for Storage instead");
                targetBuilding = FindTargetBuilding(false);
            }

            if (targetBuilding == null)
            {
                Debug.LogWarning("No suitable Storage found, stop Action");
                return false;
            }
        }
        else
        {
            GameObject deliverStorage = FindStorageToDeliver();

            if (deliverStorage == null)
            {
                Debug.LogWarning("No valid Storage available");
                return false;
            }

            targetBuilding = FindTargetBuilding(true);

            if (targetBuilding == null)
            {
                Debug.LogWarning("No Work_Building available for Storage");
                return false;
            }
        }

        if (targetBuilding.CompareTag("Mine"))
        {
            Building_Mine mineScript = targetBuilding.GetComponent<Building_Mine>();
            if (mineScript != null && mineScript.GetMineStorePoint() != null)
            {
                target = mineScript.GetMineStorePoint().gameObject;
            }
            else
            {
                Debug.LogWarning("MineStorePoint not set on Mine! Going to Mine directly.");
                target = targetBuilding;
            }
        }
        else
        {
            target = targetBuilding;
        }


        agent.SetDestination(target.transform.position);
        Debug.Log(target);

        //if coroutine runs
        if (currentTransportCoroutine != null)
        {
            StopCoroutine(currentTransportCoroutine);
        }

        currentTransportCoroutine = StartCoroutine(PerformPickupAndContinue());

        return true;
    }

    // ------------- Find and Set Unit with behaviour ------------- 

    // Find out which Unit is Selected
    private void DetermineUnitType()
    {
        if (GetComponent<Angel>() != null)
        {
            unitType = UnitType.Angel;
        }
        else if (GetComponent<Devil>() != null)
        {
            unitType = UnitType.Devil;
        }
        else
        {
            Debug.LogError("Unknown Unit Detected!");
        }
    }

    //Setup correct Tags/Ressources by Unit Type
    private void SetupTagsAndVariables()
    {
        if (unitType == UnitType.Angel)
        {
            //resourceName is important not the Tag, check Name!!
            workingTag = "Mine";
            storeTag = "Storage";
            ressourceName = "Light";
        }
        else if (unitType == UnitType.Devil)
        {
            workingTag = "Mine";
            storeTag = "Storage";
            ressourceName = "Fire";
        }

    }

    //Find GOD
    private void FindGOD()
    {
        GameObject godObject = GameObject.FindWithTag("GOD");

        if (godObject != null)
        {
            god = godObject.GetComponent<GOD>();
        }
        else
        {
            Debug.LogError("GOD does not exist!");
        }
    }

    // ------------- Check Target needs and Find fitting building ------------- 

    //Check GOD needs by Unit Type
    private bool GODNeedsResource()
    {
        if (unitType == UnitType.Angel)
        {
            //Debug.Log($"GOD Light Need: wantLight={god.wantLight}, fullLight={god.fullLight}");
            return god != null && god.wantLight > 0 && !god.fullLight;
        }
        else if (unitType == UnitType.Devil)
        {
            //Debug.Log($"GOD Fire Need: wantFire={god.wantFire}, fullFire={god.fullFire}");
            return god != null && god.wantFire > 0 && !god.fullFire;
        }

        return false;
    }


    // Find Target Building with diffrent distance logic (close or near)
    private GameObject FindTargetBuilding(bool godNeedsResource)
    {
        // Explained: needed Tag is State ? if true : else (short version for if/else logic)
        string searchTag = godNeedsResource ? workingTag : storeTag;

        GameObject[] candidates = GameObject.FindGameObjectsWithTag(searchTag);

        //Debug.Log("the current Tag is " + searchTag);

        if (candidates.Length == 0)
        {
            return null;
        }

        List<GameObject> validTargets = new List<GameObject>();

        foreach (GameObject obj in candidates)
        {
            var lockable = obj.GetComponent<IResourceManager>();
            if (lockable != null && lockable.HasAvailableResource(ressourceName))
            {
                validTargets.Add(obj);
            }
        }

        if (validTargets.Count == 0)
        {
            return null;
        }

        GameObject best = null;
        float bestDistance = GetPreferClosest() ? float.MaxValue : float.MinValue;

        foreach (var candidate in validTargets)
        {
            float dist = Vector3.Distance(transform.position, candidate.transform.position);
            if (GetPreferClosest() && dist < bestDistance)
            {
                best = candidate;
                bestDistance = dist;
            }
            else if (!GetPreferClosest() && dist > bestDistance)
            {
                best = candidate;
                bestDistance = dist;
            }
        }

        if (best != null)
        {
            var lockable = best.GetComponent<IResourceManager>();
            if (lockable != null && lockable.LockResource(ressourceName))
            {
                resourceManager = lockable;
                lockedRessourceType = ressourceName;

                return best;
            }
        }
        return null;
    }

    private GameObject FindStorageToDeliver()
    {
        GameObject[] storages = GameObject.FindGameObjectsWithTag(storeTag);

        foreach (var storage in storages)
        {
            Building_Altar storageScript = storage.GetComponent<Building_Altar>();

            if (storageScript != null)
            {
                if (unitType == UnitType.Angel && storageScript.lightAmount < storageScript.maxResourceStacksLight)
                {
                    return storage;
                }

                if (unitType == UnitType.Devil && storageScript.fireAmount < storageScript.maxResourceStacksFire)
                {
                    return storage;
                }
            }
        }

        return null;
    }

    // ------------- Start Pick Up progress ------------- 

    private IEnumerator PerformPickupAndContinue()
    {
        // Wait until Unit reached Target position
        while (Vector3.Distance(transform.position, target.transform.position) > targetDistance)
        {
            yield return null;
        }

        // Take ressource
        if (resourceManager != null && !string.IsNullOrEmpty(lockedRessourceType))
        {
            resourceManager.ConsumeLockedRessource(lockedRessourceType);
            isCarryingResource = true;

            //Debug.Log($"Unit picked up {lockedRessourceType} from {resourceManager}.");
        }

        // Reset Lock
        resourceManager = null;
        lockedRessourceType = "";

        //deny that action counts as finish by delete current target
        target = null;

        // Start to go to the final target
        ShowCarriedResource(true);
        currentPhase = TransportPhase.ToDelivery;
        GoToDeliveryPoint();
    }

    // ------------- Start delievery progress ------------- 

    // Transport Way
    private void GoToDeliveryPoint()
    {
        finalTarget = FindDeliveryTarget();

        if (finalTarget != null)
        {
            agent.SetDestination(finalTarget.transform.position);
            StartCoroutine(PerformDelivery());
            //Debug.Log($"Heading to delivery point: {finalTarget.name}");
        }
        else
        {
            Debug.LogWarning("No valid delivery target found.");
        }
    }

    private GameObject FindDeliveryTarget()
    {
        if (godNeedsResource)
        {
            return GameObject.FindWithTag("GOD");
        }
        else
        {
            GameObject[] storages = GameObject.FindGameObjectsWithTag(storeTag);
            if (storages.Length == 0)
            {
                return null;
            }

            GameObject best = null;
            float bestDistance = GetPreferClosest() ? float.MaxValue : float.MinValue;

            foreach (var storage in storages)
            {
                float dist = Vector3.Distance(transform.position, storage.transform.position);
                if (GetPreferClosest() && dist < bestDistance)
                {
                    best = storage;
                    bestDistance = dist;
                }
                else if (!GetPreferClosest() && dist > bestDistance)
                {
                    best = storage;
                    bestDistance = dist;
                }
            }

            return best;
        }
    }

    private IEnumerator PerformDelivery()
    {
        target = finalTarget;

        while (Vector3.Distance(transform.position, finalTarget.transform.position) > targetDistance)
        {
            yield return new WaitForSeconds(0.3f);
        }

        agent.isStopped = true;
        yield return new WaitForSeconds(5f);
        agent.isStopped = false;

        if (isCarryingResource)
        {
            if (finalTarget.CompareTag("GOD"))
            {
                if (unitType == UnitType.Angel)
                {
                    god.IncreaseLightRessource();
                }
                else
                {
                    god.IncreaseFireRessource();
                }

                checkoutTarget = god.GetCheckoutPoint()?.gameObject;
            }
            else if (finalTarget.CompareTag("Storage"))
            {
                var storage = finalTarget.GetComponent<Building_Altar>();

                if (storage != null)
                {
                    if (unitType == UnitType.Angel)
                    {
                        storage.IncreaseLightAmount();
                    }
                    else
                    {
                        storage.IncreaseFireAmount();
                    }

                    checkoutTarget = storage.GetCheckoutPoint()?.gameObject;
                }
            }

            isCarryingResource = false;

            if (checkoutTarget != null)
            {
                 ShowCarriedResource(false);
                 currentPhase = TransportPhase.ToCheckout;
                 agent.SetDestination(checkoutTarget.transform.position);
                 StartCoroutine(PerformCheckout());
                 yield break;
            }
        }

        //Debug.Log($"{this.name} has delivered the ressource to {finalTarget.name}");
        currentPhase = TransportPhase.ToPickup;
    }

    // ------------- Checkout and finish Action ------------- 

    private IEnumerator PerformCheckout()
    {
        target = checkoutTarget;

        while (Vector3.Distance(transform.position, target.transform.position) > targetDistance)
        {
            yield return new WaitForSeconds(0.3f);
        }
    }

    public override bool PostPerform()
    {
        return true;
    }

    // ------------- Other Methodes for tranport ------------- 

    // choose between closest or farest building
    private bool GetPreferClosest()
    {
        IUnitInterface pref = GetComponent<IUnitInterface>();
        return pref != null && pref.PreferClosest;
    }

    // find and show Ressource for correct unit
    private void ShowCarriedResource(bool show)
    {
        string tagToShow = unitType == UnitType.Angel ? "LIGHT" : "FIRE";

        Transform resourceObj = FindChildWithTag(transform, tagToShow);

        if (resourceObj != null)
        {
            resourceObj.gameObject.SetActive(show);
        }
        else
        {
            Debug.LogWarning($"No resource GameObject with tag {tagToShow} found as child of {gameObject.name}");
        }
    }

    private Transform FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag(tag))
            {
                return child;
            }
        }
        return null;
    }

    // Logic for the case that unit dies on the way
    private void OnDestroy()
    {
        ReleaseLockedResource();
    }

    private void ReleaseLockedResource()
    {
        if (resourceManager != null && !string.IsNullOrEmpty(lockedRessourceType))
        {
            resourceManager.ReleaseLock(lockedRessourceType);
            //Debug.Log($"Free Locked Ressource: {lockedRessourceType} from {resourceManager}");
            resourceManager = null;
            lockedRessourceType = "";
        }
    }
}
