using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SubGoal
{
    /// <summary>
    /// the Agent is the one of the most importants script in this game
    /// it controlls a lot of the actions and resets the planner
    /// also add new goals and switched between them.
    /// The reason why the Angel and Devil use the same Agents is pretty easy.
    /// using the same script make the script bigger but also make it easy to copy and read
    /// the problem with 2 main Agents could be performance and misleadings when it comes to
    /// interactions between angels and devils but using both here also can caused errors
    /// The Idea here is that the planner get a reset when a need is to low. Needs that doesn't get a reset will be ignored
    /// but the Planner is not that optimised yet. So many Action caused still problem but this can be surely solved
    /// this script was one of the Script that get somehow corrupted and i couldn't be opend. Still not sure how this could happen but maybe
    /// two diffrent sub agent could make some trouble if the reset at the same time
    /// </summary>
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
    private DA_ChillComplete chilling;

    /// <summary>
    /// Angel Area
    /// </summary>

    private bool noticePurity = false;
    //private bool noticeEnjoy = false;
    //private bool noticePowerAngel = false;
    private bool noticeBelieve = false;

    private bool triggerPurity = false;
    //private bool triggerEnjoy = false;
    //private bool triggerPowerAngel = false;
    private bool triggerBelieve = false;

    private AA_ShowerComplete shower;
    private AA_PrayComplete prayComplete;

    //find all Actions
    protected virtual void Start()
    {
        Actions[] acts = this.GetComponents<Actions>();
        foreach (Actions a in acts)
        {
            actions.Add(a);
        }

        if (CompareTag("Angel"))
        {
            shower = GetComponent<AA_ShowerComplete>();
            prayComplete = GetComponent<AA_PrayComplete>();

            StartCoroutine("AngelBeliefs");
        }

        if (CompareTag("Devil"))
        {
            bullyAngel = GetComponent<DA_BullyAngel>();
            punshAngel = GetComponent<DA_PunshAngel>();
            chilling = GetComponent<DA_ChillComplete>();

            StartCoroutine("DevilBeliefs");
        }
    }
    //when a task is completed or should be finished it jumps to there preperform
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

        if (currentAction is DA_WorkingComplete ||
            currentAction is DA_TransportLogic ||
            currentAction is AA_WorkingComplete ||
            currentAction is AA_TransportLogic ||
            currentAction is AA_Building)
        {
            foreach (var goal in goals.Keys.ToList())
            {
                Debug.Log("Entferne Ziel: " + goal.subGoals.Keys.First());
                goals.Remove(goal);
            }

            ResetPlanner();
            SubGoal MainGoal = new SubGoal("Survive", 1, false);
            goals.Add(MainGoal, 5);
            Debug.Log("Survive Goal startet neu");
        }
    }

    /// <summary>
    /// In Late Update is the most important Part, and cost a lot of performance
    /// here the devils and angels get specific orders what they have to do and how they get new actions or goals
    /// also the planner and actions are checked and get input the current states 
    /// </summary>
    private void LateUpdate()
    {
        if (CompareTag("Angel"))
        {

            if (((Angel)this).isBuilding)
            {
                if (currentAction != null && currentAction.running)
                {
                    Debug.Log("Abbruch der aktuellen Aktion für Build-Modus.");
                    CompleteAction();

                    foreach (var goal in goals.Keys.ToList())
                    {
                        Debug.Log("Entferne Ziel: " + goal.subGoals.Keys.First());
                        goals.Remove(goal);
                    }
                }

                ResetPlanner();
                ((Angel)this).isBuilding = false;
                SubGoal buildGoal = new SubGoal("Build", 1, false);
                goals.Add(buildGoal, 5);
            }

            if (((Angel)this).isWorking)
            {
                if (currentAction != null && currentAction.running)
                {
                    Debug.Log("Abbruch der aktuellen Aktion für Work-Modus.");
                    CompleteAction();

                    foreach (var goal in goals.Keys.ToList())
                    {
                        Debug.Log("Entferne Ziel: " + goal.subGoals.Keys.First());
                        goals.Remove(goal);
                    }
                }

                ResetPlanner();
                ((Angel)this).isWorking = false;
                SubGoal workGoal = new SubGoal("Work", 1, false);
                goals.Add(workGoal, 5);
            }

            if (((Angel)this).isTransporting)
            {
                if (currentAction != null && currentAction.running)
                {
                    Debug.Log("Abbruch der aktuellen Aktion für Transport-Modus.");
                    CompleteAction();

                    foreach (var goal in goals.Keys.ToList())
                    {
                        Debug.Log("Entferne Ziel: " + goal.subGoals.Keys.First());
                        goals.Remove(goal);
                    }
                }

                ResetPlanner();
                ((Angel)this).isTransporting = false;
                SubGoal transportGoal = new SubGoal("Transport", 1, false);
                goals.Add(transportGoal, 5);
            }


            if (currentAction != null && currentAction.running)
            {
                if (currentAction is AA_Building ||
                    currentAction is AA_PrayComplete ||
                    currentAction is AA_TransportLogic ||
                    currentAction is AA_ShowerComplete ||
                    currentAction is AA_WorkingComplete)
                {
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                }

                /*if (currentAction is AA_IDLEFinish && idleAction)
                {
                    ((Angel)this).buildingAction = false;

                    idleAction = false;

                    foreach (var goal in goals.Keys.ToList())
                    {
                        Debug.Log("Entferne Ziel: " + goal.subGoals.Keys.First());
                        goals.Remove(goal);
                    }

                    SubGoal s1 = new SubGoal("Survive", 1, false);
                    goals.Add(s1, 5);
                    Debug.Log("Ziel 'Survive' hinzugefügt.");
                    ResetPlanner();
                }*/

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
                else
                {
                    Debug.Log($"Action {currentAction.actionName} failed in PrePerform.");

                    if (currentAction is AA_ShowerComplete ||
                        currentAction is AA_PrayComplete)
                    {
                        currentAction.priorityValue += 0.01f;
                    }

                    if (currentAction is AA_TransportLogic || 
                        currentAction is AA_WorkingComplete)
                    {
                        foreach (var goal in goals.Keys.ToList())
                        {
                            Debug.Log("Entferne Ziel: " + goal.subGoals.Keys.First());
                            goals.Remove(goal);
                        }

                        ResetPlanner();
                        SubGoal MainGoal = new SubGoal("Survive", 1, false);
                        goals.Add(MainGoal, 5);
                        Debug.Log("Survive Goal starts new");
                    }
                }
            }
            else
            {
                ResetPlanner();
            }
        }

        if (CompareTag("Devil"))
        {
            //controll if a bool is true and would switch the goal if it is so
            if (((Devil)this).isBuilding)
            {
                if (currentAction is DA_ChillComplete ||
                    currentAction is DA_BullyAngel ||
                    currentAction is GA_MoveAround)
                {
                    Debug.Log("Abbruch der aktuellen Aktion für Build-Modus.");
                    CompleteAction();

                    foreach (var goal in goals.Keys.ToList())
                    {
                        Debug.Log("Entferne Ziel: " + goal.subGoals.Keys.First());
                        goals.Remove(goal);
                    }
                }

                // reset planner means restart the planner with the first action or calculate new and order the actions new
                ResetPlanner();
                ((Devil)this).isBuilding = false; 
                SubGoal buildGoal = new SubGoal("Build", 1, false);
                goals.Add(buildGoal, 5);
            }

            if (((Devil)this).isWorking)
            {
                if (currentAction != null && currentAction.running)
                {
                    Debug.Log("Abbruch der aktuellen Aktion für Work-Modus.");
                    CompleteAction();

                    foreach (var goal in goals.Keys.ToList())
                    {
                        Debug.Log("Entferne Ziel: " + goal.subGoals.Keys.First());
                        goals.Remove(goal);
                    }
                }

                ResetPlanner();
                ((Devil)this).isWorking = false;
                SubGoal workGoal = new SubGoal("Work", 1, false);
                goals.Add(workGoal, 5);
            }

            if (((Devil)this).isTransporting)
            {
                if (currentAction != null && currentAction.running)
                {
                    Debug.Log("Abbruch der aktuellen Aktion für Transport-Modus.");
                    CompleteAction();

                    foreach (var goal in goals.Keys.ToList())
                    {
                        Debug.Log("Entferne Ziel: " + goal.subGoals.Keys.First());
                        goals.Remove(goal);
                    }
                }

                ResetPlanner();
                ((Devil)this).isTransporting = false;
                SubGoal transportGoal = new SubGoal("Transport", 1, false);
                goals.Add(transportGoal, 5);
            }

            if (currentAction != null && currentAction.running)
            {
                if (currentAction is DA_BullyAngel ||
                    currentAction is DA_PunshAngel ||
                    currentAction is DA_Spawning ||
                    currentAction is DA_Building ||
                    currentAction is DA_ChillComplete ||
                    currentAction is DA_TransportLogic)
                {
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                }

                /*if (currentAction is DA_IDLEFinish && idleAction)
                {
                    ((Devil)this).isWorking = false;
                    idleAction = false;

                    foreach (var goal in goals.Keys.ToList())
                    {
                        Debug.Log("Entferne Ziel: " + goal.subGoals.Keys.First());
                        goals.Remove(goal);
                    }

                    SubGoal s1 = new SubGoal("Survive", 1, false);
                    goals.Add(s1, 5);
                    Debug.Log("Ziel 'Survive' hinzugefügt.");
                    ResetPlanner();
                }*/

                //WICHTIG FÜR PRIOR CHANGE + und - !!!
                /* if (currentAction is DA_Working)
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

                 if ((currentAction is DA_PrepareAction && !prepareDevilAction.doneWork) ||
                     (currentAction is DA_CleanAction && !cleanDevilAction.doneWork))
                 {
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
                 }*/

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

                //sort goals when a new Planner was choosed
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
                //queue prepares the next actions
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
                
                else
                {
                    /// <summary>
                    /// when an actions failed is cost get higher, higher cost means that the action will sort on a other position with
                    ///the next planner reset, this is used for dynamics and unpredictible moves but also means that actions need always
                    ///recalculate and this caused a lot of performance
                    /// </summary>
                    Debug.Log($"Action {currentAction.actionName} failed in PrePerform.");
                    
                    if (currentAction is DA_ChillComplete ||
                        currentAction is DA_BullyAngel ||
                        currentAction is DA_PunshAngel)
                    {
                        currentAction.priorityValue += 0.01f;
                    }

                    if (currentAction is DA_TransportLogic ||
                        currentAction is DA_WorkingComplete)
                    {
                        foreach (var goal in goals.Keys.ToList())
                        {
                            Debug.Log("Entferne Ziel: " + goal.subGoals.Keys.First());
                            goals.Remove(goal);
                        }

                        ResetPlanner();
                        SubGoal MainGoal = new SubGoal("Survive", 1, false);
                        goals.Add(MainGoal, 5);
                        Debug.Log("Survive Goal starts new");
                    }
                }
            }
            else
            {
                ResetPlanner();
            }

        }
    }

    /// <summary>
    /// Devil Beliefs works very simple a bool get activated when the current need if a devil is fine
    /// as lower as the need falls over time, that so higher is the chance for a reset. when a need resets
    /// it clean his own states other needs and their action will be ignored if they are fine. in the later process this
    /// should work more unpredictabel and more actions will have the same keys so the devil can choose between diffrent opition like the
    /// example of bully angel and punsh angel
    /// </summary>
    /// <returns></returns>
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

            yield return new WaitForSeconds(10f);
        }
    }

    private void ResetEvil()
    {
        if (triggerEvil)
        {
            if (currentAction is GA_MoveAround || currentAction is DA_ChillComplete)
            {
                noticeEvil = true;
                currentRunningAction = currentAction;
                Debug.Log("ResetEvil");
                StartCoroutine("WaitBeforeResetDevil");
            }
            else
            {
                triggerEvil = false;
            }
        }
    }

    private void ResetChill()
    {
        if (triggerChill && !(currentAction is DA_ChillComplete))
        {
            if (currentAction is GA_MoveAround || currentAction is DA_BullyAngel)
            {
                noticeChill = true;
                currentRunningAction = currentAction;
                Debug.Log("ResetChill");
                StartCoroutine("WaitBeforeResetDevil");
            }
            else
            {
                triggerEvil = false;
            }
        }
    }


    private IEnumerator WaitBeforeResetDevil()
    {
        if (currentRunningAction == null)
        {
            Debug.LogError("[WaitBeforeResetDevil] no current Action is running!");
            yield break;
        }

        while (currentRunningAction.running && !(currentRunningAction is GA_MoveAround))
        {
            Debug.Log($"[WaitBeforeResetDevil] Waiting for action {currentRunningAction.actionName} to finish.");
            yield return null;
        }

        Debug.Log($"[WaitBeforeResetDevil] Action {currentRunningAction.actionName} has completed. Proceeding to reset.");

        Dictionary<string, int> relevantState = currentAction.GetRelevantDevilState();

        if (relevantState.ContainsKey("evil"))
        {
            relevantState["evil"] = 0;
            Debug.Log("MonitorEvilKey: Key 'evil' set to 0");

            Debug.Log("Planner reset because Evil");
            triggerEvil = false;
            bullyAngel.done = false;
            //punshAngel.done = false;
            noticeEvil = false;
        }

        if (relevantState.ContainsKey("chilling"))
        {
            relevantState["chilling"] = 0;
            Debug.Log("MonitorChillKey: Key 'chilling' set to " + relevantState["chilling"]);

            Debug.Log("Planner reset because chilling");
            triggerChill = false;
            noticeChill = false;
        }

        ResetPlanner();
    }

    private IEnumerator AngelBeliefs()
    {
        Angel angel = GetComponent<Angel>();
        
        while (true)
        {
            if (angel != null)
            {
                if (!noticePurity)
                {
                    float showerIndicator = Random.Range(20f, 70f);

                    if (showerIndicator > angel.needPurity)
                    {
                        triggerPurity = true;
                        ResetShower();
                    }
                }

                if (!noticeBelieve)
                {
                    float prayIndicator = Random.Range(20f, 70f);

                    if (prayIndicator > angel.needBelieve)
                    {
                        triggerBelieve = true;
                        ResetPray();
                    }
                }
            }

            yield return new WaitForSeconds(10f);
        }
    }

    private void ResetShower()
    {
        if (triggerPurity && !(currentAction is AA_ShowerComplete))
        {
            if (currentAction is GA_MoveAround || currentAction is AA_PrayComplete)
            {
                noticePurity = true;
                currentRunningAction = currentAction;
                Debug.Log("ResetShower");
                StartCoroutine("WaitBeforeResetAngelShower");
            }
            else
            {
                triggerPurity = false;
            }
        }
    }

    private void ResetPray()
    {
        if (triggerBelieve && !(currentAction is AA_PrayComplete))
        {
            if (currentAction is GA_MoveAround || currentAction is AA_ShowerComplete)
            {
                noticeBelieve = true;
                currentRunningAction = currentAction;
                Debug.Log("ResetPray");
                StartCoroutine("WaitBeforeResetAngelPray");
            }
            else
            {
                triggerBelieve = false;
            }
        }
    }

    private IEnumerator WaitBeforeResetAngelShower()
    {
        if (currentRunningAction == null)
        {
            Debug.LogError("[WaitBeforeResetAngel] no current Action is running!");
            yield break;
        }

        while (currentRunningAction.running && !(currentRunningAction is GA_MoveAround))
        {
            //Debug.Log($"[WaitBeforeResetAngel] Waiting for action {currentRunningAction.actionName} to finish.");
            yield return null;
        }

        Debug.Log($"[WaitBeforeResetAngel] Action {currentRunningAction.actionName} has completed. Proceeding to reset.");

        Dictionary<string, int> relevantState = currentAction.GetRelevantAngelState();

        if (relevantState.ContainsKey("shower"))
        {
            relevantState["shower"] = 0;
            Debug.Log("MonitorShowerKey: Key 'shower' set to " + relevantState["shower"]);

            Debug.Log("Planner reset because shower");
            triggerPurity = false;
            noticePurity = false;
        }

        ResetPlanner();
    }

    private IEnumerator WaitBeforeResetAngelPray()
    {
        if (currentRunningAction == null)
        {
            Debug.LogError("[WaitBeforeResetAngel] no current Action is running!");
            yield break;
        }

        while (currentRunningAction.running && !(currentRunningAction is GA_MoveAround))
        {
            //Debug.Log($"[WaitBeforeResetAngel] Waiting for action {currentRunningAction.actionName} to finish.");
            yield return null;
        }

        Debug.Log($"[WaitBeforeResetAngel] Action {currentRunningAction.actionName} has completed. Proceeding to reset.");

        Dictionary<string, int> relevantState = currentAction.GetRelevantAngelState();

        if (relevantState.ContainsKey("pray"))
        {
            relevantState["pray"] = 0;
            Debug.Log("MonitorPrayKey: Key 'pray' set to " + relevantState["pray"]);

            Debug.Log("Planner reset because pray");
            triggerBelieve = false;
            noticeBelieve = false;
        }

        ResetPlanner();
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
                Debug.Log("New plan with goal: " + sg.Key.subGoals.Keys.First());
                break;
            }
        }

        if (actionQueue == null)
        {
            Debug.LogWarning("new planning failed!");
        }
    }
}
