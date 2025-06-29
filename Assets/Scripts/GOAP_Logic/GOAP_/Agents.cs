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
    private Actions queuedTemporaryAction = null;

    public bool playersWish = false;
    public float spawnWaitingTime = 6f;

    //Types
    public enum UnitType
    {
        Angel,
        Devil
    }

    public UnitType unitType;

    //Spawning logic
    private bool getSpawned = true;
    private Transform spawnTarget;

    [Header("TargetArea")]
    [SerializeField] private float targetRadius = 1.0f;

    [Header("Temporary Action Visual")]
    public GameObject temporaryActionIndicator;


    //find all Actions
    protected virtual void Start()
    {
        string spawnName = CompareTag("Angel") ? "AngelSpawnPoint" : "DevilSpawnPoint";
        GameObject spawnPoint = GameObject.Find(spawnName);

        if (spawnPoint != null)
        {
            spawnTarget = spawnPoint.transform;
            StartCoroutine(HandleInitialSpawn());
        }
        else
        {
            Debug.LogError("Spawn point not found: " + spawnName);
        }

        Actions[] acts = this.GetComponents<Actions>();
        foreach (Actions a in acts)
        {
            actions.Add(a);
        }

        unitType = CompareTag("Angel") ? UnitType.Angel : UnitType.Devil;

        RegisterAngelDevil.Instance?.RegisterNPC(this);
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
           
            ResetPlanner();

            SubGoal repeatedGoal = new SubGoal("Survive", 1, false);  
            if (!goals.Keys.Any(g => g.subGoals.Keys.First() == "Survive"))  
            {
                goals.Add(repeatedGoal, 5);
            }

            Debug.Log("Goal reached, reset Planner");
        }

        // task behaviour
        if (queuedTemporaryAction != null)
        {
            Debug.Log("Executing queued temporary action: " + queuedTemporaryAction.actionName);
            ExecuteTemporaryAction(queuedTemporaryAction);
            queuedTemporaryAction = null;
            return;
        }

        if (temporaryActionIndicator != null)
        {
            temporaryActionIndicator.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (getSpawned)
        {
            return; 
        }

        HandleAgent();
    }

    private IEnumerator HandleInitialSpawn()
    {
        // move to spawn location before action starts
        if (spawnTarget != null)
        {
            UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            agent.SetDestination(spawnTarget.position);

            // Wait until unit reached destination
            while (Vector3.Distance(transform.position, spawnTarget.position) > 1.5f)
            {
                yield return null;
            }
        }

        // waiting before Action starts
        yield return new WaitForSeconds(spawnWaitingTime);

        getSpawned = false;
    }

    private void HandleAgent()
    {
        if (currentAction != null && currentAction.running)
        {
            if (currentAction.target == null)
            {
                Debug.LogWarning("target was destroyed, action is done: " + currentAction.actionName);
                CompleteAction();
                return;
            }

            if (ShouldMoveToTarget(currentAction))
            {
                currentAction.agent.SetDestination(currentAction.target.transform.position);
            }

            float distanceToTarget = Vector3.Distance(currentAction.target.transform.position, this.transform.position);
            if (currentAction.agent.hasPath && distanceToTarget < targetRadius) ///currentAction.agent.remainingDistance < 1f
            {
                if (!invoked)
                {
                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true;
                }
            }

            return;
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

                AdjustPriorityValueOrResetPlanner(currentAction);
            }
        }
        else
        {
            ResetPlanner();
        }
    }

    private bool ShouldMoveToTarget(Actions action)
    {
        switch (unitType)
        {
            case UnitType.Angel:
                return action is AA_Building || 
                       action is GA_Working ||
                       action is GA_TransportLogic ||
                       action is AA_PrayComplete ||
                       action is AA_TransportLogic ||
                       action is AA_ShowerComplete ||
                       action is AA_WorkingComplete;
            case UnitType.Devil:
                return action is DA_BullyAngel ||
                       action is GA_Working ||
                       action is GA_TransportLogic ||
                       action is DA_PunshAngel ||
                       action is DA_Spawning ||
                       action is DA_Building ||
                       action is DA_ChillComplete ||
                       action is DA_TransportLogic;
            default:
                return false;
        }
    }

    private void AdjustPriorityValueOrResetPlanner(Actions action)
    {
        if (
           (unitType == UnitType.Angel && (action is AA_ShowerComplete || action is AA_PrayComplete)) ||
           (unitType == UnitType.Devil && (action is DA_ChillComplete || action is DA_BullyAngel || action is DA_PunshAngel))
           )
        {
            action.priorityValue += 0.01f;
        }
    }

    public bool HasActiveTemporaryAction => currentAction != null && currentAction.running && IsTemporaryAction(currentAction);

    private bool IsTemporaryAction(Actions action)
    {
        return action is GA_Building; // später ersetzten durch: return action is GA_Building || action is GA_Working || action is GA_TransportLogic;
    }

    public void TemporaryAction(Actions tempAction)
    {
        if (queuedTemporaryAction != null)
        {
            Debug.LogWarning("Temporary action is already in queued. Ignoring new temporary action");
            return;
        }

        if (currentAction == null || !currentAction.running)
        {
            Debug.Log("Starting temporary action immediately: " + tempAction.actionName);
            ExecuteTemporaryAction(tempAction);
        }
        else
        {
            Debug.Log("Queuing temporary action for later: " + tempAction.actionName);
            queuedTemporaryAction = tempAction;
        }
    }

    private void ExecuteTemporaryAction(Actions tempAction)
    {
        ResetPlanner();
        currentAction = tempAction;
        currentAction.running = true;
        currentAction.agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (currentAction.PrePerform())
        {
            if (currentAction.target == null && !string.IsNullOrEmpty(currentAction.targetTag))
            {
                currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
            }

            if (currentAction.target != null)
            {
                currentAction.agent.SetDestination(currentAction.target.transform.position);
            }
            else
            {
                Debug.LogWarning($"ExecuteTemporaryAction: no target was found {currentAction.actionName}");
            }
        }
        else
        {
            Debug.LogWarning($"ExecuteTemporaryAction: PrePerform failed in {currentAction.actionName}");
        }

        Debug.Log($"Started temporary action: {currentAction.actionName}");

        if (temporaryActionIndicator != null && IsTemporaryAction(currentAction))
        {
            temporaryActionIndicator.SetActive(true);
        }
    }

    public void Die()
    {
        RegisterAngelDevil.Instance?.UnregisterNPC(this);
        Destroy(this.gameObject, 0.25f);
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
