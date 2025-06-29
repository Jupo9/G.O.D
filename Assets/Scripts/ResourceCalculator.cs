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
        var usable = allResources
                     .Where(r => r.key == key && r.resourceType != ResourceType.Locked && r.provider.HasAvailableResource(KeyToSimpleName(key)))
                     .OrderBy(r => r.resourceType == ResourceType.Mine ? 1 : 0)
                     .ToList();

        int remaining = amount;

        foreach (var res in usable)
        {
            if (res.provider.LockResource(KeyToSimpleName(key)))
            {
                res.provider.ConsumeLockedRessource(KeyToSimpleName(key));
                remaining--;

                if (remaining <= 0)
                    return true;
            }
        }

        return false;
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
