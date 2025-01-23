using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    private bool noticePurity = false;
    //private bool noticeEnjoy = false;
    //private bool noticePowerAngel = false;
    //private bool noticeBelieve = false;

    private bool triggerPurity = false;
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

        if (CompareTag("Angel"))
        {
            shower = GetComponent<AA_Shower>();
            prepareAngelAction = GetComponent<AA_PrepareAction>();
            cleanAngelAction = GetComponent<AA_CleanAction>();

            StartCoroutine("AngelBeliefs");
        }

        if (CompareTag("Devil"))
        {
            bullyAngel = GetComponent<DA_BullyAngel>();
            punshAngel = GetComponent<DA_PunshAngel>();
            chilling = GetComponent<DA_Chilling>();
            prepareDevilAction = GetComponent<DA_PrepareAction>();
            cleanDevilAction = GetComponent<DA_CleanAction>();

            StartCoroutine("DevilBeliefs");
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
            if (((Angel)this).isBuilding)
            {
                if (currentAction != null && currentAction.running)
                {
                    Debug.Log("Abbruch der aktuellen Aktion für Build-Modus.");
                    CompleteAction();
                }

                ResetPlanner();
                ((Angel)this).isBuilding = false;
                SubGoal buildGoal = new SubGoal("Build", 1, false);
                goals.Add(buildGoal, 5);
            }

            if (currentAction != null && currentAction.running)
            {
                if (currentAction is AA_Building)
                {
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
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
                else
                {
                    Debug.Log($"Action {currentAction.actionName} failed in PrePerform.");

                    if (currentAction is AA_PrepareAction && !prepareAngelAction.foundBuilding ||
                        currentAction is AA_Shower ||
                        currentAction is AA_CleanAction && !cleanAngelAction.foundBuilding )
                    {
                        currentAction.priorityValue += 1;
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
            if (((Devil)this).isBuilding)
            {
                if (currentAction != null && currentAction.running)
                {
                    Debug.Log("Abbruch der aktuellen Aktion für Build-Modus.");
                    CompleteAction();
                }

                ResetPlanner();
                ((Devil)this).isBuilding = false; 
                SubGoal buildGoal = new SubGoal("Build", 1, false);
                goals.Add(buildGoal, 5);
            }

            if (currentAction != null && currentAction.running)
            {
                if (currentAction is DA_BullyAngel)
                {
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                }

                if (currentAction is DA_PunshAngel)
                {
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                }

                if (currentAction is DA_Building)
                {
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                }

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

                    if (currentAction is DA_PrepareAction && !prepareDevilAction.foundBuilding ||
                        currentAction is DA_Chilling ||
                        currentAction is DA_CleanAction && !cleanDevilAction.foundBuilding ||
                        currentAction is DA_BullyAngel ||
                        currentAction is DA_PunshAngel)
                    {
                        currentAction.priorityValue += 1;
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
            if (currentAction is DA_CleanAction || currentAction is DA_Transport || currentAction is GA_MoveAround)
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
        if (triggerChill)
        {
            if (currentAction is DA_BullyAngel || currentAction is GA_MoveAround)
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
            Debug.LogError("[WaitBeforeResetDevil] Keine laufende Aktion gefunden!");
            yield break;
        }

        while (currentRunningAction.running)
        {
            Debug.Log($"[WaitBeforeResetDevil] Waiting for action {currentRunningAction.actionName} to finish.");
            yield return null;
        }

        Debug.Log($"[WaitBeforeResetDevil] Action {currentRunningAction.actionName} has completed. Proceeding to reset.");

        Dictionary<string, int> relevantState = currentAction.GetRelevantDevilState();

        if (relevantState.ContainsKey("evil"))
        {
            int evilValue = relevantState["evil"];

            if (evilValue <= 1)
            {
                relevantState["evil"] = 0;
                Debug.Log("MonitorEvilKey: Key 'evil' set to 0");
            }

            Debug.Log("Planner wird zurückgesetzt. Evil");
            triggerEvil = false;
            bullyAngel.done = false;
            //punshAngel.done = false;
            noticeEvil = false;
        }

        if (relevantState.ContainsKey("cleanChill"))
        {
            int chillValue = relevantState["cleanChill"];

            if (chillValue <= 1)
            {
                relevantState["cleanChill"] = 0;
                Debug.Log("MonitorChillKey: Key 'cleanChill' set to 0");
            }

            Debug.Log("Planner wird zurückgesetzt. cleanChill");
            triggerChill = false;
            prepareDevilAction.doneChill = false;
            cleanDevilAction.doneChill = false;
            chilling.done = false;
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

                //Add more notices
            }

            yield return new WaitForSeconds(5f);
        }
    }

    private void ResetShower()
    {
        if (triggerPurity)
        {
            if (currentAction is AA_CleanAction || currentAction is GA_MoveAround)
            {
                noticePurity = true;
                currentRunningAction = currentAction;
                Debug.Log("ResetShower");
                StartCoroutine("WaitBeforeResetAngel");
            }
            else
            {
                triggerPurity = false;
            }
        }
    }

    private IEnumerator WaitBeforeResetAngel()
    {
        if (currentRunningAction == null)
        {
            Debug.LogError("[WaitBeforeResetAngel] Keine laufende Aktion gefunden!");
            yield break;
        }

        while (currentRunningAction.running)
        {
            Debug.Log($"[WaitBeforeResetAngel] Waiting for action {currentRunningAction.actionName} to finish.");
            yield return null;
        }

        Debug.Log($"[WaitBeforeResetAngel] Action {currentRunningAction.actionName} has completed. Proceeding to reset.");

        Dictionary<string, int> relevantState = currentAction.GetRelevantAngelState();

        if (relevantState.ContainsKey("cleanShower"))
        {
            int showerValue = relevantState["cleanShower"];

            if (showerValue <= 1)
            {
                relevantState["cleanShower"] = 0;
                Debug.Log("MonitorShowerKey: Key 'cleanShower' set to 0");
            }

            Debug.Log("Planner wird zurückgesetzt. Shower");
            triggerPurity = false;
            shower.done = false;
            prepareAngelAction.doneShower = false;
            cleanAngelAction.doneShower = false;
            noticePurity = false;
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
                Debug.Log("Neuer Plan erstellt mit Ziel: " + sg.Key.subGoals.Keys.First());
                break;
            }
        }

        if (actionQueue == null)
        {
            Debug.LogWarning("Planung fehlgeschlagen: Kein Plan gefunden.");
        }
    }
}
