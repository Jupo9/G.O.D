using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceCalculator : MonoBehaviour
{
    public static ResourceCalculator Instance { get; private set; }

    [SerializeField] private List<ResourceManager> allResources = new List<ResourceManager>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterResourceSource(ResourceManager resourceManager)
    {
        if (!allResources.Contains(resourceManager))
        {
            allResources.Add(resourceManager);
        }
    }

    public bool TypConsumeResources(string key, int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        string simpleKey = KeyToSimpleName(key);

        var usable = allResources
                     .Where(r => r.key == key && r.resourceType != ResourceType.Locked && r.provider.HasAvailableResource(simpleKey))
                     .OrderBy(r => r.resourceType == ResourceType.Mine ? 1 : 0)
                     .ToList();


        foreach (var res in usable)
        {
            int availableCount = res.provider.HasAvailableResource(simpleKey) ? 1 : 0;
        }

        int remaining = amount;

        foreach (var res in usable)
        {
            while (remaining > 0 && res.provider.HasAvailableResource(simpleKey))
            {
                bool locked = res.provider.LockResource(simpleKey);
                if (!locked)
                {
                    break;
                }

                res.provider.ConsumeLockedRessource(simpleKey);
                remaining--;
            }

            if (remaining <= 0)
            {
                break;
            }
        }

        if (remaining > 0)
        {
            return false;
        }

        return true;
    }


    public int GetTotalAvailable(string key)
    {
        return allResources
               .Where(r => r.key == key && r.resourceType != ResourceType.Locked)
               .Sum(r => r.provider.HasAvailableResource(KeyToSimpleName(key)) ? 1 : 0);
    }

    public void AddResource(ResourceManager resource)
    {
        allResources.Add(resource);
    }

    private string KeyToSimpleName(string key)
    {
        if (key == "Res_fire")
        {
            return "Fire";
        }

        if (key == "Res_light")
        {
            return "Light";
        }

        return key;
    }
}
