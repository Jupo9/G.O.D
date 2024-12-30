using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node
{
    public Node parent;
    public float cost;
    public Dictionary<string, int> state;
    public Actions action;

    public Node(Node parent, float cost, Dictionary<string, int> allstates, Actions action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allstates);
        this.action = action;
    }
}

public class Planner
{
    public Queue<Actions> plan(List<Actions> actions, Dictionary<string, int> goal, WorldStates states)
    {
        List<Actions> usableActions = new List<Actions>();

        foreach (Actions a in actions)
        {
            Dictionary<string, int> relevantState = Worlds.Instance.GetWorld().GetStates();


            bool stateCheck = true;

            foreach (KeyValuePair<string, int> precondition in a.preconditions)
            {
                if (relevantState.ContainsKey(precondition.Key) && relevantState[precondition.Key] != precondition.Value)
                {
                    if (precondition.Key == "evil" && relevantState[precondition.Key] == 1)
                    {
                        Debug.Log($"Action '{a.actionName}': Treating 'evil == 1' as completed for precondition.");
                        a.ApplyEffects();
                        relevantState[precondition.Key] = 0; 
                        continue; 
                    }

                    Debug.Log($"Skipping Action '{a.actionName}' due to state '{precondition.Key}' != {precondition.Value} (current: {relevantState[precondition.Key]}).");
                    stateCheck = false;
                    break;
                }
            }

            if (a.IsArchievable() && stateCheck)
            {
                usableActions.Add(a);
            }
        }

        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0, Worlds.Instance.GetWorld().GetStates(), null);

        bool success = BuildGraph(start, leaves, usableActions, goal);

        if(!success)
        {
           /// Debug.Log("NO PLAN");
            return null;
        }

        Node cheapest = null;
        foreach(Node leaf in leaves)
        {
            if(cheapest == null)
            {
                cheapest = leaf;
            }
            else
            {
                if(leaf.cost < cheapest.cost)
                {
                    cheapest = leaf;
                }
            }
        }

        List<Actions> result = new List<Actions>();
        Node n = cheapest;
        while(n != null) 
        {
            if(n.action != null)
            {
                result.Insert(0, n.action);
            }
            n = n.parent;
        }

        Queue<Actions> queue = new Queue<Actions>();

        if (result.Count == 0)
        {
            Debug.LogError("No actions available to achieve the goal. Plan failed.");
            return null;
        }

        foreach (Actions a in result)
        {
            queue.Enqueue(a);
        }

        ///Debug.Log("The Plan is: ");
        foreach(Actions a in queue)
        {
            ///Debug.Log("Q: " + a.actionName);
        }

        return queue;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, List<Actions> usableActions, Dictionary<string, int> goal)
    {
        bool foundPath = false;
        foreach(Actions action in usableActions)
        {
            if(action.IsArchievableGiven(parent.state))
            {
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);
                foreach(KeyValuePair<string, int> eff in action.effect)
                {
                    if (!currentState.ContainsKey(eff.Key))
                    {
                        currentState.Add(eff.Key, eff.Value);
                    }
                    else
                    {
                        currentState[eff.Key] = eff.Value;
                    }
                }
                Node node = new Node(parent, parent.cost + action.wayCosts, currentState, action);

                if(GoalAchieved(goal, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    List<Actions> subset = ActionSubset(usableActions, action);
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if(found)
                    {
                        foundPath = true;
                    }
                }
            }
        }
        return foundPath;
    }

    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
    {
        foreach (KeyValuePair<string, int> g in goal)
        {
            if (!state.ContainsKey(g.Key))
            {
                return false;
            }
        }
        return true;
    }

    private List<Actions> ActionSubset(List<Actions> actions, Actions removeMe)
    {
        List<Actions> subset = new List<Actions>();

        foreach(Actions a in actions)
        {
            if (!a.Equals(removeMe))
            {
                subset.Add(a);
            }
        }
        return subset;
    }
}

