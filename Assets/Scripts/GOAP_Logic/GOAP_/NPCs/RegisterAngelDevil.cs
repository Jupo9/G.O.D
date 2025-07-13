using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegisterAngelDevil : MonoBehaviour
{
    public static RegisterAngelDevil Instance;

    private List<Agents> angels = new List<Agents>();
    private List<Agents> devils = new List<Agents>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //StartCoroutine(LogNPCCounts());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterNPC(Agents agent)
    {
        if (agent.unitType == Agents.UnitType.Angel)
        {
            if (!angels.Contains(agent))
                angels.Add(agent);
        }
        else if (agent.unitType == Agents.UnitType.Devil)
        {
            if (!devils.Contains(agent))
                devils.Add(agent);
        }
    }

    public void UnregisterNPC(Agents agent)
    {
        angels.Remove(agent);
        devils.Remove(agent);
    }

    public Agents GetBestAvailableAngel()
    {
        return angels
        .OfType<Angel>() 
        .Where(a => !a.HasActiveTemporaryAction)
        .OrderByDescending(a => a.currentFeeling)
        .FirstOrDefault();
    }

    public Agents GetWorstAvailableDevil()
    {
        return devils
        .OfType<Devil>()
        .Where(d => !d.HasActiveTemporaryAction)
        .OrderBy(d => d.currentFeeling)
        .FirstOrDefault();
    }

    public List<Agents> GetAllAngels() => angels;
    public List<Agents> GetAllDevils() => devils;

    // ------------- Debug Testing -------------

    /*private IEnumerator LogNPCCounts()
    {
        while (true)
        {
            yield return new WaitForSeconds(20f);
            Debug.Log($"[NPCRegistry] Angels: {angels.Count} | Devils: {devils.Count}");
        }
    }*/
}
