using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class SubGoal
{
    public Dictionary<string, int> subGoals;
    public bool remove;

    public SubGoal(string s, int i, bool r)
    {
        subGoals = new Dictionary<string, int>();
        subGoals.Add(s, i);
        remove = r;
    }
}

public class Agents : MonoBehaviour
{
    public List<Actions> actions = new List<Actions>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();

    Planner planner;
    Queue<Actions> actionQueue;
    public Actions currentAction;
    SubGoal currentGoal;

    public float needChill = 100f;
    public float needEvil = 100f;

    public float needShower = 100f;

    public float lostOverTime = 1f;

    private static List<Agents> calmingAgents = new List<Agents>();

    private bool isInChillingZone = false;

    protected virtual void Start()
    {
        Actions[] acts = this.GetComponents<Actions>();
        foreach(Actions a in acts)
        {
            actions.Add(a);
        }

        StartCoroutine(NeedDecay());
    }

    /// <summary>
    /// This funktions (OnTrigggerEnter and OnTriggerExit) was for testings and need to be implementet in the action DA_Chilling!!
    /// </summary>
    /// <param name="other"></param>
    /// 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chilling"))
        {
            isInChillingZone = true; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Chilling"))
        {
            isInChillingZone = false;
        }
    }

    private IEnumerator NeedDecay()
    {
        while (needChill > 0f || needEvil > 0f || needShower > 0f)
        {
            needChill -= lostOverTime;
            needEvil -= lostOverTime;
            needShower -= lostOverTime;

            yield return new WaitForSeconds(1f);

            if (needShower <= 20f && currentGoal == null)
            {
                SubGoal s1 = new SubGoal("Survive", 1, true);
                goals[s1] = 3; 
            }

            if (needChill <= 0 || needEvil <= 0f || needShower <= 0f)
            {
                Destroy(this.gameObject);
                yield break;
            }
        }
    }


    bool invoked = false;
    void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;

        if (actionQueue.Count == 0 && currentGoal != null)
        {
            if (currentGoal.remove)
            {
                goals.Remove(currentGoal);
            }
            currentGoal = null;
        }
    }

    private void LateUpdate()
    {
        if (CompareTag("Angel"))
        {
            if (currentAction != null && currentAction.running)
            {
                if (currentAction is AA_Shower)
                {
                    /// Implement Shower function
                }

                if (currentAction != null && currentAction.running)
                {
                    float distanceToTarget = Vector3.Distance(currentAction.target.transform.position, this.transform.position);
                    if (currentAction.agent.hasPath && distanceToTarget < 2f) ///currentAction.agent.remainingDistance < 1f
                    {
                        if (!invoked)
                        {
                            Invoke("CompleteAction", currentAction.duration);
                            invoked = true;
                        }
                    }
                    return;
                }
            }


            if (planner == null || actionQueue == null)
            {
                planner = new Planner();

                var sortedGoals = from entry in goals orderby entry.Value descending select entry;

                foreach (KeyValuePair<SubGoal, int> sg in sortedGoals)
                {
                    actionQueue = planner.plan(actions, sg.Key.subGoals, null);
                    if (actionQueue != null)
                    {
                        currentGoal = sg.Key;
                        break;
                    }
                }

               Debug.Log("ActionQueue generated: " + (actionQueue != null ? "Yes" : "No"));
            }

            if (actionQueue != null && actionQueue.Count > 0)
            {
                currentAction = actionQueue.Dequeue();
               Debug.Log("Assigned Action: " + currentAction.actionName);
                if (currentAction.PrePerform())
                {
                    if (currentAction.target == null && currentAction.targetTag != "")
                    {
                        currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                    }

                    if (currentAction.target != null)
                    {
                        currentAction.running = true;
                        currentAction.agent.SetDestination(currentAction.target.transform.position);
                    }
                }
            }
            else
            {
                Debug.Log("No actions in queue!");
            }


        }

        if (CompareTag("Devil"))
        {
            if (currentAction != null && currentAction.running)
            {
                if (currentAction is DA_Chilling && isInChillingZone)
                {
                    needChill = Mathf.Min(100f, needChill + Time.deltaTime * 10f);
                    if (needChill > 100f) needChill = 100f;
                }

                if (currentAction is DA_BullyAngel)
                {
                    currentAction.agent.SetDestination(currentAction.target.transform.position); 
                }

                if(currentAction is DA_MoveAround) 
                {
                    ///Überprüfen von Werten um neuen Plan aufzustellen
                }

                if (currentAction != null && currentAction.running)
                {
                    float distanceToTarget = Vector3.Distance(currentAction.target.transform.position, this.transform.position);
                    if (currentAction.agent.hasPath && distanceToTarget < 2f) ///currentAction.agent.remainingDistance < 1f
                    {
                        if (!invoked)
                        {
                            Invoke("CompleteAction", currentAction.duration);
                            invoked = true;
                        }
                    }
                    return;
                }
            }


            if (planner == null || actionQueue == null)
            {
                planner = new Planner();

                var sortedGoals = from entry in goals orderby entry.Value descending select entry;

                foreach (KeyValuePair<SubGoal, int> sg in sortedGoals)
                {
                    actionQueue = planner.plan(actions, sg.Key.subGoals, null);
                    if (actionQueue != null)
                    {
                        currentGoal = sg.Key;
                        break;
                    }
                }

               Debug.Log("ActionQueue generated: " + (actionQueue != null ? "Yes" : "No"));
            }

            if (actionQueue != null && actionQueue.Count > 0)
            {
                currentAction = actionQueue.Dequeue();
                Debug.Log("Assigned Action: " + currentAction.actionName);
                if (currentAction.PrePerform())
                {
                    if (currentAction.target == null && currentAction.targetTag != "")
                    {
                        currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                    }

                    if (currentAction.target != null)
                    {
                        currentAction.running = true;
                        currentAction.agent.SetDestination(currentAction.target.transform.position);
                    }
                }
            }
            else
            {
                 Debug.Log("No actions in queue!");
            }

        }
    }

}
