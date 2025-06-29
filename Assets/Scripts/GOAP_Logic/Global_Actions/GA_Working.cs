using System.Collections;
using UnityEngine;

public class GA_Working : Actions
{
    enum UnitType
    {
        Angel,
        Devil
    }

    [Tooltip("WorkTime need to be less than duration to avoid conflicts")]
    [Header("WorkTime")]
    [SerializeField] private float workingTime = 0f;

    private UnitType unitType;

    private string workingTag;
    private string ressourceName;

    private GameObject checkoutTarget;
    private Building_Mine mineScript;

    public override bool PrePerform()
    {
        DetermineUnitType();
        SetupTagsAndVariables();
        StartWorking();

        return true;
    }

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
            ressourceName = "Light";
        }
        else if (unitType == UnitType.Devil)
        {
            workingTag = "Mine";
            ressourceName = "Fire";
        }
    }

    private void StartWorking()
    {
        target = GetWorkingTarget();

        if (target != null)
        {
            mineScript = target.GetComponent<Building_Mine>();

            if (mineScript != null)
            {
                mineScript.SetBlocked(true);
            }

            agent.SetDestination(target.transform.position);
            StartCoroutine(WorkingRoutine());
        }
        else
        {
            Debug.LogWarning("no valid work target found!");
        }
    }

    private GameObject GetWorkingTarget()
    {
        GameObject[] mines = GameObject.FindGameObjectsWithTag(workingTag);
        Debug.Log(workingTag);

        if (mines.Length == 0)
        {
            return null;
        }

        GameObject best = null;
        float bestDistance = GetPreferClosest() ? float.MaxValue : float.MinValue;

        foreach (GameObject mine in mines)
        {
            Building_Mine building = mine.GetComponent<Building_Mine>();
            if (building != null && building.isAvailable)
            {
                float dist = Vector3.Distance(transform.position, mine.transform.position);
                if (GetPreferClosest() && dist < bestDistance)
                {
                    best = mine;
                    bestDistance = dist;
                }
                else if (!GetPreferClosest() && dist > bestDistance)
                {
                    best = mine;
                    bestDistance = dist;
                }
            }
        }

        return best;
    }

    private IEnumerator WorkingRoutine()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        agent.isStopped = true;
        yield return new WaitForSeconds(workingTime);

        checkoutTarget = mineScript.GetCheckoutPoint()?.gameObject;
        target = checkoutTarget;

        if (mineScript != null)
        {
            if (ressourceName == "Fire")
            {
                mineScript.IncreaseFireAmount();
            }
            else if (ressourceName == "Light")
            {
                mineScript.IncreaseLightAmount();
            }
            else
            {
                Debug.LogWarning("Unknown resource type: " + ressourceName);
            }
        }

        checkoutTarget = mineScript.GetCheckoutPoint()?.gameObject;

        if (checkoutTarget != null)
        {
            agent.SetDestination(checkoutTarget.transform.position);
            StartCoroutine(PerformCheckout());
            yield break;
        }
        else
        {
            Debug.LogWarning("No CheckoutPoint assigned to Mine!");
        }
    }

    private bool GetPreferClosest()
    {
        IUnitInterface unit = GetComponent<IUnitInterface>();
        return unit != null && unit.PreferClosest;
    }

    // ------------- Checkout and finish Action ------------- 

    private IEnumerator PerformCheckout()
    {
        agent.isStopped = false;

        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return new WaitForSeconds(0.3f);
        }

        mineScript.SetBlocked(false);
    }

    public override bool PostPerform()
    {
        return true;
    }


}
