using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Actions : MonoBehaviour
{
    public string actionName = "Action";
    public float wayCosts = 1f;
    public float timeCosts = 2f;
    public GameObject target;
    public string targetTag;
    public float duration = 0f;
    public WorldState[] preConditions;
    public WorldState[] afterEffects;
    public NavMeshAgent agent;

    public Dictionary<string, int> preconditions;
    public Dictionary<string, int> effect;

    public WorldStates agentBeliefs;

    public bool running = false;

    public Actions()
    {
        preconditions = new Dictionary<string, int>();
        effect = new Dictionary<string, int>();
    }

    public void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
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

    public Dictionary<string, int> GetRelevantState()
    {
        Devil devil = GetComponentInParent<Devil>(); 
        if (devil != null)
        {
            return devil.localStates.GetStates(); 
        }
        else
        {
            return Worlds.Instance.GetWorld().GetStates();
        }
    }

    public bool IsArchievable()
    {
        return true;
    }

    public bool IsArchievableGiven(Dictionary<string, int> conditions)
    {
        foreach(KeyValuePair<string, int> p in preconditions)
        {
            if(!conditions.ContainsKey(p.Key))
            {
                return false;
            }
        }
        return true;
    }

    public void ApplyEffects()
    {
        Dictionary<string, int> relevantState = GetRelevantState();

        foreach (KeyValuePair<string, int> eff in effect)
        {
            if (relevantState.ContainsKey(eff.Key))
            {
                relevantState[eff.Key] = 1; 
            }
            else
            {
                relevantState[eff.Key] = 1;
            }
        }
    }

    public abstract bool PrePerform();
    public abstract bool PostPerform();
}
