using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Actions : MonoBehaviour
{
    /// <summary>
    /// this is the ground contruct of all actions
    /// every Action will have access to this informations for example the target or costs
    /// </summary>
    public string actionName = "Action";
    public float priorityValue = 1f;
    public float timeCosts = 2f;
    public GameObject target;
    public string targetTag;
    /// <summary>
    /// duration caused some issues with running corountines, to fix this in some action the duration get overide for now
    /// to fix this with corountines always calculate how much time the corountine need to success
    /// else the States will not apply correct because the PostPerform will be scipped!
    /// </summary>
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

    //Releventstates are states thats only count for the Individual, there is no share with others or World States
    // A example would be the State of chilling, when one Devil is chilling no other devil will get the same State
    // with that there all are individiuals
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

    //Applys effect, States for the Individual
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

    /// <summary>
    /// PrePerform and PostPerform are there for the precondissions and effect
    /// all Actions have the same logic, they lock for the goal for example "Survive"
    /// then all Actions that brings the NPC to his Goal will be prepared.
    /// the Goal is an effect that the NPC will try to reached as fast and effectiv as possibel
    /// Preconditiions say if the the NPC need a other effect or can use them freely.
    /// So for example there are 3 Actions chill, shower and work
    /// chill ( has "clean" as precondition and "Survive" as effect)
    /// shower( has "dirty" as preconidition and "clean" as effect)
    /// work ( has no precondition and "dirty" as effect)
    /// the Action and Planner will sort them like that [Goal "Survive" for that need "clean" and "clean" must be "dirty" before]
    /// so the plan would be work - shower - chill
    /// What happens in the precondition is not important for the system only the effect is that what the planner want to reache the next
    /// to the goal. That's how all Actions will work, in the easiest way to explain
    /// </summary>
    /// <returns></returns>
    public abstract bool PrePerform();
    public abstract bool PostPerform();
}

