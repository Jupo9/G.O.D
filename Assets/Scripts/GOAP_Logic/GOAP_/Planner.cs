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

        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0, Worlds.Instance.GetWorld().GetStates(), null);

        bool success = BuildGraph(start, leaves, usableActions, goal);

        if(!success)
        {
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

                    Node child = new Node(currentNode, currentNode.cost + 1f, currentState, action);
                    openList.Enqueue(child, child.cost);
                }
            }
        }

        return false;
    }

    public class PriorityQueue<T>
    {
        private List<(T item, float priority)> elements = new();

        public int Count => elements.Count;

        public void Enqueue(T item, float priority)
        {
            elements.Add((item, priority));
            int c = elements.Count - 1;

            while (c > 0)
            {
                int parent = (c - 1) / 2;
                if (elements[c].priority >= elements[parent].priority)
                    break;

                (elements[c], elements[parent]) = (elements[parent], elements[c]);
                c = parent;
            }
        }

        public T Dequeue()
        {
            int li = elements.Count - 1;
            (T item, float priority) frontItem = elements[0];
            elements[0] = elements[li];
            elements.RemoveAt(li);

            --li;
            int pi = 0;
            while (true)
            {
                int ci = pi * 2 + 1;
                if (ci > li) break;
                int rc = ci + 1;
                if (rc <= li && elements[rc].priority < elements[ci].priority)
                    ci = rc;

                if (elements[pi].priority <= elements[ci].priority)
                    break;

                (elements[pi], elements[ci]) = (elements[ci], elements[pi]);
                pi = ci;
            }

            return frontItem.item;
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
