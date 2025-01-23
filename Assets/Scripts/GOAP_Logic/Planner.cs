using System.Collections.Generic;
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
                    currentState[eff.Key] = eff.Value;
                }

                Node node = new Node(parent, parent.cost + action.priorityValue, currentState, action);

                if (GoalAchieved(goal, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    List<Actions> subset = ActionSubset(usableActions, action);
                    if (BuildGraph(node, leaves, subset, goal))
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
        return actions.Where(a => !a.Equals(removeMe)).ToList();
    }
}

