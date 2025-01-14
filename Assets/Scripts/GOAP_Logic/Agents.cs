using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using Unity.VisualScripting;

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

    private Actions currentRunningAction;

    public bool playersWish = false;
    //private bool idleAction = false;

    /// <summary>
    /// Devil Area
    /// </summary>

    //private bool personalTarget = false;

    private bool noticeEvil = false;
    private bool noticeChill = false;
    //private bool noticePowerDevil = false;
    //private bool noticeJoy = false;

    private bool triggerEvil = false;
    private bool triggerChill = false;
    //private bool triggerPowerDevil = false; 
    //private bool triggerJoy = false;


    private DA_BullyAngel bullyAngel;
    private DA_PunshAngel punshAngel;
    private DA_Chilling chilling;
    private DA_PrepareAction prepareDevilAction;
    private DA_CleanAction cleanDevilAction;

    /// <summary>
    /// Angel Area
    /// </summary>

    //private bool noticePurity = false;
    //private bool noticeEnjoy = false;
    //private bool noticePowerAngel = false;
    //private bool noticeBelieve = false;

    //private bool triggerPurity = false;
    //private bool triggerEnjoy = false;
    //private bool triggerPowerAngel = false;
    //private bool triggerBelieve = false;

    private AA_PrepareAction prepareAngelAction;
    private AA_CleanAction cleanAngelAction;
    private AA_Shower shower;


    protected virtual void Start()
    {
        Actions[] acts = this.GetComponents<Actions>();
        foreach (Actions a in acts)
        {
            actions.Add(a);
        }

        bullyAngel = GetComponent<DA_BullyAngel>();
        punshAngel = GetComponent<DA_PunshAngel>();
        chilling = GetComponent<DA_Chilling>();
        prepareDevilAction = GetComponent<DA_PrepareAction>();
        cleanDevilAction = GetComponent<DA_CleanAction>();

        shower = GetComponent<AA_Shower>();
        prepareAngelAction = GetComponent<AA_PrepareAction>();
        cleanAngelAction = GetComponent<AA_CleanAction>();

        StartCoroutine("DevilBeliefs");
        //StartCoroutine("AngelBeliefs");
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
        //MonitorEvilKey();

        if (CompareTag("Angel"))
        {
            if (currentAction != null && currentAction.running)
            {
                if (currentAction is AA_PrepareAction)
                {
                    ///Implentieren von Methoden
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
                //Debug.Log("No actions in queue!");
            }


        }

        if (CompareTag("Devil"))
        {
            if (currentAction != null && currentAction.running)
            {
                WorldStates worldStates = Worlds.Instance.GetWorld();

                if (currentAction is DA_BullyAngel)
                {
                    if (worldStates.HasState("Avail_angel"))
                    {
                        int availableAngelsValue = worldStates.GetStates()["Avail_angel"];

                        if (availableAngelsValue == 0)
                        {
                            currentAction.running = false;
                            currentAction = null;
                            currentAction.priorityValue += 1;
                            return;
                        }
                    }

                    currentAction.agent.SetDestination(currentAction.target.transform.position);

                    /*GameObject[] angels = GameObject.FindGameObjectsWithTag("Angel");

                    List<GameObject> availableAngels = new List<GameObject>();
                    foreach (GameObject angel in angels)
                    {
                        Angel angelScript = angel.GetComponent<Angel>();
                        if (angelScript != null && angelScript.available)
                        {
                            availableAngels.Add(angel);
                        }
                    }

                    if (availableAngels.Count > 0)
                    {
                        //personalTarget = true;
                        GameObject nearestAngel = availableAngels
                            .OrderBy(angel => Vector3.Distance(transform.position, angel.transform.position))
                            .First();

                        currentAction.target = nearestAngel;

                        currentAction.agent.SetDestination(currentAction.target.transform.position);
                    }

                    if (availableAngels.Count == 0 && !personalTarget)
                    {
                        Debug.Log("Missing free Angel");
                    }*/
                }

                if (currentAction is DA_PunshAngel)
                {
                    if (worldStates.HasState("Avail_angel"))
                    {
                        int availableAngelsValue = worldStates.GetStates()["Avail_angel"];

                        if (availableAngelsValue == 0)
                        {
                            currentAction.running = false;
                            currentAction = null;
                            currentAction.priorityValue += 1;
                            return;
                        }
                    }

                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                }

                if (currentAction is DA_Chilling)
                {
                    if (worldStates.HasState("Build_iron"))
                    {
                        int availableAngelsValue = worldStates.GetStates()["Build_iron"];

                        if (availableAngelsValue == 0)
                        {
                            currentAction.running = false;
                            currentAction = null;
                            currentAction.priorityValue += 1;
                            return;
                        }
                    }
                }

                if (currentAction is DA_Working)
                {
                    if (worldStates.HasState("Build_iron"))
                    {
                        int buildIronValue = worldStates.GetStates()["Build_iron"];

                        if (buildIronValue == 0)
                        {
                            currentAction.running = false;
                            currentAction = null;
                            currentAction.priorityValue += 1;
                            return;
                        }
                    }
                }

                if (currentAction is DA_PrepareAction || currentAction is DA_CleanAction)
                {
                    if (worldStates.HasState("Build_iron"))
                    {
                        int buildIronValue = worldStates.GetStates()["Build_iron"];

                        if (buildIronValue == 0)
                        {
                            currentAction.running = false;
                            currentAction = null;
                            currentAction.priorityValue += 1;
                            return;
                        }
                    }
                    if (worldStates.HasState("Build_fire"))
                    {
                        int buildFireValue = worldStates.GetStates()["Build_fire"];

                        if (buildFireValue == 0)
                        {
                            currentAction.running = false;
                            currentAction = null;
                            currentAction.priorityValue += 1;
                            return;
                        }
                    }
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
                ResetPlanner();
            }

        }
    }

    private IEnumerator DevilBeliefs()
    {
        Devil devil = GetComponent<Devil>();

        while (true)
        {
            if (devil != null)
            {
                if (!noticeEvil)
                {
                    //Debug.Log("current Value of NeedEvil: " + devil.needEvil);

                    float evilIndicator = Random.Range(20f, 70f);

                    if (evilIndicator > devil.needEvil)
                    {
                        //Debug.Log("Random Number was: " + evilIndicator + " and was higher than " + devil.needEvil);
                        triggerEvil = true;
                        ResetEvil();
                    }
                    else
                    {
                        //Debug.Log("Random Number was: " + evilIndicator + " and was not high enough for " + devil.needEvil);
                    }
                }

                if (!noticeChill)
                {
                    float chillIndicator = Random.Range(20f, 70f);

                    if (chillIndicator > devil.needChill)
                    {
                        triggerChill = true;
                        ResetChill();
                    }
                }

                /*if (!noticeJoy)
                {
                    float joyIndicator = Random.Range(20f, 70f);
                }

                if (!noticePowerDevil)
                {
                    float powerDevilIndicator = Random.Range(20f, 70f);
                }*/
            }

            yield return new WaitForSeconds(5f);
        }
    }

    private void ResetEvil()
    {
        if (triggerEvil)
        {
            if (currentAction is DA_CleanAction || currentAction is DA_Transport)
            {
                noticeEvil = true;
                currentRunningAction = currentAction;
                Debug.Log("ResetEvil");
                StartCoroutine("WaitBeforeReset");
            }
            else
            {
                triggerEvil = false;
            }
        }
    }

    private IEnumerator WaitBeforeReset()
    {
        if (currentRunningAction == null)
        {
            Debug.LogError("[WaitBeforeReset] Keine laufende Aktion gefunden!");
            yield break;
        }

        while (currentRunningAction.running)
        {
            Debug.Log($"[WaitBeforeReset] Waiting for action {currentRunningAction.actionName} to finish.");
            yield return null;
        }

        Debug.Log($"[WaitBeforeReset] Action {currentRunningAction.actionName} has completed. Proceeding to reset.");

        Dictionary<string, int> relevantState = currentAction.GetRelevantState();

        if (relevantState.ContainsKey("evil"))
        {
            int evilValue = relevantState["evil"];

            if (evilValue == 1)
            {
                relevantState["evil"] = 0;
                Debug.Log("MonitorEvilKey: Key 'evil' set to 0");
            }
        }

        Debug.Log("Planner wird zurückgesetzt. Evil");

        triggerEvil = false;
        bullyAngel.done = false;
        //punshAngel.done = false;
        noticeEvil = false;
        ResetPlanner();
    }

    /*private void ResetPlannerOld()
    {
        planner = new Planner();

        actionQueue = null;
        currentAction = null;
        currentGoal = null;

        var sortedGoals = from entry in goals orderby entry.Value descending select entry;

        foreach (KeyValuePair<SubGoal, int> sg in sortedGoals)
        {
            actionQueue = planner.plan(actions, sg.Key.subGoals, null);
            if (actionQueue != null)
            {
                currentGoal = sg.Key;
                Debug.Log("Neuer Plan erstellt mit Ziel: " + sg.Key.subGoals.Keys.First());
                break;
            }
        }

        if (actionQueue == null)
        {
            Debug.LogWarning("Planung fehlgeschlagen: Kein Plan gefunden.");
        }
    }*/

    private void ResetChill()
    {
        if (triggerChill)
        {
            if (currentAction is DA_CleanAction || currentAction is DA_Transport || currentAction is DA_BullyAngel || currentAction is DA_PunshAngel)
            {
                Debug.Log("Planner wird zurückgesetzt. Chill");
                noticeChill = true;
                triggerChill = false;

                prepareDevilAction.done = false;
                chilling.done = false;
                cleanDevilAction.done = false;

                if (Worlds.Instance.GetWorld().HasState("preChill"))
                {
                    int value = Worlds.Instance.GetWorld().GetStates()["preChill"];
                    if (value == 1)
                    {
                        Worlds.Instance.GetWorld().ModifyState("preChill", 0);
                        Debug.Log("preChill wurde zu WorldStates entfernt.");
                    }
                }

                if (Worlds.Instance.GetWorld().HasState("chill"))
                {
                    int value = Worlds.Instance.GetWorld().GetStates()["chill"];
                    if (value == 1)
                    {
                        Worlds.Instance.GetWorld().ModifyState("chill", 0);
                        Debug.Log("chill wurde zu WorldStates entfernt.");
                    }
                }

                if (Worlds.Instance.GetWorld().HasState("cleanChill"))
                {
                    int value = Worlds.Instance.GetWorld().GetStates()["cleanChill"];
                    if (value == 1)
                    {
                        Worlds.Instance.GetWorld().ModifyState("cleanChill", 0);
                        Debug.Log("cleanChill wurde zu WorldStates entfernt.");
                    }
                }

                ResetPlanner();
                noticeChill = false;

            }
            else
            {
                triggerChill = false;
            }
        }
    }

    private void ResetPlanner()
    {
        planner = new Planner();

        actionQueue = null;
        currentAction = null;
        currentGoal = null;

        var sortedGoals = from entry in goals orderby entry.Value descending select entry;

        foreach (KeyValuePair<SubGoal, int> sg in sortedGoals)
        {
            actionQueue = planner.plan(actions, sg.Key.subGoals, null);
            if (actionQueue != null)
            {
                currentGoal = sg.Key;
                Debug.Log("Neuer Plan erstellt mit Ziel: " + sg.Key.subGoals.Keys.First());
                break;
            }
        }

        if (actionQueue == null)
        {
            Debug.LogWarning("Planung fehlgeschlagen: Kein Plan gefunden.");
        }
    }


    public void MonitorEvilKey()
    {
        if (currentAction != null)
        {
            Dictionary<string, int> relevantState = currentAction.GetRelevantState();

            if (relevantState.ContainsKey("evil"))
            {
                int evilValue = relevantState["evil"];
                Debug.Log($"MonitorEvilKey: Current value of 'evil' is {evilValue}");
            }
        }

    }
}
