using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    /// <summary>
    /// Represents a node in the planning graph, used for goal-oriented action planning.
    /// Each node stores its parent, cost, current state, and the action leading to this state.
    /// </summary>
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

/// <summary>
/// Handles goal-oriented actions. planning them by constructing a plan (a sequence of actions) 
/// that reach a given goal based on the world's current state and available actions. All actions get sorted by there cost and useage
/// the planner, like the world states and world not really are in the scenes they communicate thourgh other scripts like Agents and Actions
/// </summary>
public class Planner
{
    public Queue<Actions> plan(List<Actions> actions, Dictionary<string, int> goal, WorldStates states)
    {
        List<Actions> usableActions = new List<Actions>();

        foreach (Actions a in actions)
        {
            if (a.IsArchievable())
            {
                usableActions.Add(a);
            }
        }

        usableActions.Sort((a, b) => b.priorityValue.CompareTo(a.priorityValue));

        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0, Worlds.Instance.GetWorld().GetStates(), null);

        bool success = BuildGraph(start, leaves, usableActions, goal);

        if(!success)
        {
           /// Debug.Log("NO PLAN");
            return null;
        }

        Node cheapest = null;
        foreach (Node leaf in leaves)
        {
            if (cheapest == null || leaf.cost < cheapest.cost)
            {
                cheapest = leaf;
            }
        }

        List<Actions> result = new List<Actions>();
        Node n = cheapest;
        while (n != null)
        {
            if (n.action != null)
            {
                result.Insert(0, n.action);
            }
            n = n.parent;
        }

        Queue<Actions> queue = new Queue<Actions>(result);
        return queue.Count > 0 ? queue : null;
    }

    // Builds a graph of nodes to explore potential plans, starting from the initial state.
    private bool BuildGraph(Node start, List<Node> leaves, List<Actions> usableActions, Dictionary<string, int> goal)
    {
        PriorityQueue<Node> openList = new PriorityQueue<Node>();
        openList.Enqueue(start, 0);

        while (openList.Count > 0)
        {
            Node currentNode = openList.Dequeue();

            if (GoalAchieved(goal, currentNode.state))
            {
                leaves.Add(currentNode);
                return true;
            }

            foreach (Actions action in usableActions)
            {
                if (action.IsArchievableGiven(currentNode.state))
                {
                    Dictionary<string, int> currentState = new Dictionary<string, int>(currentNode.state);
                    foreach (KeyValuePair<string, int> eff in action.effect)
                    {
                        currentState[eff.Key] = eff.Value;
                    }

                    Node child = new Node(currentNode, currentNode.cost + action.priorityValue, currentState, action);
                    openList.Enqueue(child, child.cost);
                }
            }
        }

        return false;
    }

    // Priority queue implementation for handling nodes by priority.
    public class PriorityQueue<T>
    {
        private List<(T item, float priority)> elements = new List<(T, float)>();

        public int Count => elements.Count;

        public void Enqueue(T item, float priority)
        {
            elements.Add((item, priority));
            elements.Sort((a, b) => a.priority.CompareTo(b.priority)); 
        }

        public T Dequeue()
        {
            var item = elements[0];
            elements.RemoveAt(0);
            return item.item;
        }
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
        return actions.Where(a => !a.Equals(removeMe)).ToList();
    }
}