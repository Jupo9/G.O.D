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

    private Queue<Actions> currentPlan;

    public bool playersWish = false;
    //private bool idleAction = false;

    /// <summary>
    /// Devil Area
    /// </summary>

    private bool personalTarget = false;

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

        //Worlds.Instance.GetWorld().SetState("evil", 1);

        bullyAngel = GetComponent<DA_BullyAngel>();
        punshAngel = GetComponent<DA_PunshAngel>();
        chilling = GetComponent<DA_Chilling>();
        prepareDevilAction = GetComponent<DA_PrepareAction>();
        cleanDevilAction = GetComponent<DA_CleanAction>();

        shower = GetComponent<AA_Shower>();
        prepareAngelAction = GetComponent<AA_PrepareAction>();
        cleanAngelAction = GetComponent<AA_CleanAction>();

        StartCoroutine("DevilBeliefs");
        StartCoroutine("AngelBeliefs");
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

        if (currentAction.effect != null)
        {
            Dictionary<string, int> relevantState = Worlds.Instance.GetWorld().GetStates();

            foreach (KeyValuePair<string, int> eff in currentAction.effect)
            {
                if (relevantState.ContainsKey(eff.Key))
                {
                    relevantState[eff.Key] += eff.Value;
                }
                else
                {
                    relevantState[eff.Key] = eff.Value;
                }
            }

            Debug.Log(currentAction.actionName + "Action effects applied to state.");
        }
    }

    private void LateUpdate()
    {
        if (playersWish)
        {
            ResetPlanner();
            playersWish = false; 
        }

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
                        currentPlan = new Queue<Actions>(actionQueue);
                        break;
                    }
                }

                Debug.Log("ActionQueue generated: " + (actionQueue != null ? "Yes" : "No"));
            }

            if (actionQueue != null && actionQueue.Count > 0)
            {
                currentAction = actionQueue.Dequeue();
                Debug.Log("Assigned Action: " + currentAction.actionName);

                Dictionary<string, int> relevantState = Worlds.Instance.GetWorld().GetStates();
                bool canPerform = true;

                foreach (KeyValuePair<string, int> precondition in currentAction.preconditions)
                {
                    if (relevantState.ContainsKey(precondition.Key) && relevantState[precondition.Key] != 0)
                    {
                        Debug.Log($"Skipping Action '{currentAction.actionName}' due to unmet state condition: {precondition.Key}");
                        canPerform = false;
                        break;
                    }
                }

                if (canPerform && currentAction.PrePerform())
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
                //Debug.Log("No Action in Queue");
            }


        }

        if (CompareTag("Devil"))
        {
            if (currentAction != null && currentAction.running)
            {
                if (currentAction is DA_BullyAngel)
                {
                    GameObject[] angels = GameObject.FindGameObjectsWithTag("Angel");

                    List<GameObject> availableAngels = new List<GameObject>();
                    foreach (GameObject angel in angels) 
                    {
                        Angel angelScript = angel.GetComponent<Angel>();
                        if(angelScript != null &&  angelScript.available)
                        {
                            availableAngels.Add(angel);
                        }
                    }

                    if (availableAngels.Count == 0 && !personalTarget)
                    {
                        Debug.Log("Missing free Angel");
                    }

                    if (availableAngels.Count > 0)
                    {
                        personalTarget = true;
                        GameObject nearestAngel = availableAngels
                            .OrderBy(angel => Vector3.Distance(transform.position, angel.transform.position))
                            .First();

                        currentAction.target = nearestAngel;

                        currentAction.agent.SetDestination(currentAction.target.transform.position);
                    }

                    //punshAngel.done = true;
                }

                if (currentAction is DA_PunshAngel)
                {
                    currentAction.agent.SetDestination(currentAction.target.transform.position);

                    bullyAngel.done = true;
                }

                if(currentAction is GA_MoveAround) 
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
                        currentPlan = new Queue<Actions>(actionQueue);
                        break;
                    }
                }

               Debug.Log("ActionQueue generated: " + (actionQueue != null ? "Yes" : "No"));
            }

            if (actionQueue != null && actionQueue.Count > 0)
            {
                currentAction = actionQueue.Dequeue();
                Debug.Log("Assigned Action: " + currentAction.actionName);

                Dictionary<string, int> relevantState = Worlds.Instance.GetWorld().GetStates();

                /*bool canPerform = true;

                foreach (KeyValuePair<string, int> precondition in currentAction.preconditions)
                {
                    if (relevantState.ContainsKey(precondition.Key) && relevantState[precondition.Key] != 0)
                    {
                        Debug.Log($"Skipping Action '{currentAction.actionName}' due to unmet state condition: {precondition.Key}");
                        canPerform = false;
                        break;
                    }
                }*/

                if (currentAction.PrePerform()) // (/*canPerform &&*/ currentAction.PrePerform()) 
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
                //Debug.Log("No Action in Queue");
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
                    /*else
                    {
                        Debug.Log("Random Number was: " + evilIndicator + " and was not high enough for " + devil.needEvil);
                    }*/
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
                        ResetPurity();
                    }
                }

                /*if (!noticeBelieve)
                {
                    float believeIndicator = Random.Range(20f, 70f);
                }

                if (!noticeEnjoy)
                {
                    float enjoyIndicator = Random.Range(20f, 70f);
                }

                if (!noticePowerAngel)
                {
                    float powerAngelIndicator = Random.Range(20f, 70f);
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
                Debug.Log("ResetEvil");
                StartCoroutine(WaitBeforeResetEvil());
            }
            else
            {
                triggerEvil = false;
            }
        }
    }

    private IEnumerator WaitBeforeResetEvil()
    {
        Debug.Log("Start Corountine Evil");

        while (!currentAction.PostPerform()) 
        {
            yield return null;
        }

        Debug.Log("Planner wird zurückgesetzt. Evil");
        personalTarget = false;
        triggerEvil = false;

        bullyAngel.done = false;
        //punshAngel.done = false;

        if (Worlds.Instance.GetWorld().HasState("evil"))
        {
            Worlds.Instance.GetWorld().SetState("evil", 0);
            Debug.Log("State 'evil' wurde auf 0 gesetzt.");
        }

        noticeEvil = false;

        ResetPlanner();
    }

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
                    Worlds.Instance.GetWorld().SetState("preChill", 0);
                }

                if (Worlds.Instance.GetWorld().HasState("chill"))
                {
                    Worlds.Instance.GetWorld().SetState("chill", 0);
                }

                if (Worlds.Instance.GetWorld().HasState("cleanChill"))
                {
                    Worlds.Instance.GetWorld().SetState("cleanChill", 0);
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

    private void ResetPurity()
    {
        if (triggerPurity)
        {
            if (currentAction is AA_CleanAction || currentAction is AA_Transport)
            {
                Debug.Log("Planner wird zurückgesetzt. Purity");
                noticePurity = true;
                triggerPurity = false;

                prepareAngelAction.done = false;
                shower.done = false;
                cleanAngelAction.done = false;

                if (Worlds.Instance.GetWorld().HasState("preShower"))
                {
                    int value = Worlds.Instance.GetWorld().GetStates()["preShower"];
                    if (value == 1)
                    {
                        Worlds.Instance.GetWorld().ModifyState("preShower", 0);
                        Debug.Log("preShower wurde zu WorldStates entfernt.");
                    }
                }

                if (Worlds.Instance.GetWorld().HasState("shower"))
                {
                    int value = Worlds.Instance.GetWorld().GetStates()["shower"];
                    if (value == 1)
                    {
                        Worlds.Instance.GetWorld().ModifyState("shower", 0);
                        Debug.Log("shower wurde zu WorldStates entfernt.");
                    }
                }

                if (Worlds.Instance.GetWorld().HasState("cleanShower"))
                {
                    int value = Worlds.Instance.GetWorld().GetStates()["cleanShower"];
                    if (value == 1)
                    {
                        Worlds.Instance.GetWorld().ModifyState("cleanShower", 0);
                        Debug.Log("cleanShower wurde zu WorldStates entfernt.");
                    }
                }


                noticePurity = false;

            }
            else
            {
                triggerChill = false;
            }
        }
    }

    /*public void CancelCurrentAction()
    {
        if (currentAction != null && currentAction.running)
        {
            currentAction.running = false;
            currentAction.agent.ResetPath();
            Debug.Log("Current action abgebrochen.");
        }
    }*/

    public void ResetPlanner()
    {
        //CancelCurrentAction();

        if (currentPlan == null || currentPlan.Count == 0)
        {
            Debug.Log("ResetPlanner: Kein gültiger Plan vorhanden.");
            return;
        }

        actionQueue = new Queue<Actions>(currentPlan);

        Debug.Log("ResetPlanner: Plan wurde zurückgesetzt. Der Agent startet wieder mit der ersten Action.");
    }



    /// <summary>
    /// Delete Plannner is an option to delete Items complete from the list
    /// to bring back a deleted action for example:
    ///                 if (!actions.Contains(bullyAngel))
    ///                 {
    ///                 actions.Add(bullyAngel);
    ///                 }
    /// unfortunately this caused a lot of performance and it turns out is not the right choice for many npc's
    /// so if there is any action that should be complete remove for a list than this can be used, but only as a quick solution
    /// </summary>
    /*private void DeletePlanner()
    {
        for (int i = actions.Count - 1; i >= 0; i--)
        {
            Actions action = actions[i];

            if (action is DA_BullyAngel bullyAngelAction && bullyAngelAction.done)
            {
                actions.RemoveAt(i); 
                Debug.Log("DA_BullyAngel Aktion entfernt, da 'done' == true");
            }
            else if (action is DA_PunshAngel punshAngelAction && punshAngelAction.done)
            {
                actions.RemoveAt(i);
                Debug.Log("DA_PunshAngel Aktion entfernt, da 'done' == true");
            }
        }



        if (actionQueue != null)
        {
            actionQueue.Clear();
            actionQueue = null;
        }

        currentAction = null;
        currentGoal = null;
        test = false;
    }*/

}
