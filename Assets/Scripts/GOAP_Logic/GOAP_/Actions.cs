using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Actions : MonoBehaviour
{
    public string actionName = "Action";
    public GameObject target;
    public string targetTag;

    //public float duration = 0f;
    public WorldState[] preConditions;
    public WorldState[] afterEffects;
    public NavMeshAgent agent;

    public Dictionary<string, int> preconditions;
    public Dictionary<string, int> effect;

    public WorldStates agentBeliefs;
    public Agents agentScriptReference;

    public bool running = false;

    public Actions()
    {
        preconditions = new Dictionary<string, int>();
        effect = new Dictionary<string, int>();
    }

    public void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>() ?? this.gameObject.AddComponent<NavMeshAgent>();

        if (agent == null)
        {
            agent = this.gameObject.AddComponent<NavMeshAgent>();
        }

        if (preconditions != null)
        {
            foreach (WorldState w in preConditions)
            {
                preconditions.Add(w.key, w.value);
            }
        }

        if (afterEffects != null)
        {
            foreach (WorldState w in afterEffects)
            {
                effect.Add(w.key, w.value);
            }
        }
    }

    public Dictionary<string, int> GetRelevantDevilState()
    {
        Devil devil = GetComponentInParent<Devil>();
        if (devil != null && devil.localStates != null)
        {
            return devil.localStates.GetStates();
        }
        return Worlds.Instance.GetWorld().GetStates();
    }

    public Dictionary<string, int> GetRelevantAngelState()
    {
        Angel angel = GetComponentInParent<Angel>();
        if (angel != null && angel.localStates != null)
        {
            return angel.localStates.GetStates();
        }
        return Worlds.Instance.GetWorld().GetStates();
    }

    public bool IsArchievable()
    {
        return true;
    }

    public bool IsArchievableGiven(Dictionary<string, int> conditions)
    {
        foreach (KeyValuePair<string, int> p in preconditions)
        {
            if (!conditions.ContainsKey(p.Key) || conditions[p.Key] != p.Value)
            {
                return false;
            }
        }
        return true;
    }

    public void ApplyDevilEffects()
    {
        Dictionary<string, int> relevantState = GetRelevantDevilState();

        foreach (KeyValuePair<string, int> eff in effect)
        {
            relevantState[eff.Key] = eff.Value;
        }
    }

    public void ApplyAngelEffects()
    {
        Dictionary<string, int> relevantState = GetRelevantAngelState();

        foreach (KeyValuePair<string, int> eff in effect)
        {
            relevantState[eff.Key] = eff.Value;
        }
    }

    public void FinishAction()
    {
        running = false;
        agentScriptReference?.CompleteAction();
    }

    public abstract bool PrePerform();
    public abstract bool PostPerform();
}

