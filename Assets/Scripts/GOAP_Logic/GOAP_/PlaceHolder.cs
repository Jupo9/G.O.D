using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHolder : MonoBehaviour
{
    /*
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
        public float spawnWaitingTime = 6f;

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

        //Spawning logic
        private bool getSpawned = true;
        private Transform spawnTarget;

        [Header("TargetArea")]
        [SerializeField] private float targetRadius = 1.0f;

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

        private void LateUpdate()
        {
            // Denied Actions, 
            if (getSpawned)
            {
                return;
            }

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


                    if (currentAction != null && currentAction.running)
                    {
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
                     }
    ~~~

                    if (currentAction != null && currentAction.running)
                    {
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

        private IEnumerator HandleInitialSpawn()
        {
            // moce to spawn location before action starts
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
                    }
    ~~~
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

        ------------------------------------------------------------------------DEVIL---------------------------------------------------------------

        using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.UIElements;

    public class Devil : Agents, IUnitInterface
    {
        [Header("Needs")]
        public float currentFeeling = 100f;

        [Header("UI")]
        public GameObject objectCanvas;
        public GameObject targetRendererObject;
        public GameObject targetBuildUI;
        public GameObject targetNeedUI;
        public string targetMaterialName = "Outline_1";

        public GameObject targetUIForBuildings;
        public GameObject targetUIForNeeds;
        //public GameObject targetUIForTransport;

        private MeshRenderer targetMeshRenderer;
        private int targetMaterialIndex = -1;

        [Header("Believes")]
        public float needEvil = 100f;
        public float needChill = 100f;
        public float needJoy = 100f;
        public float needPower = 100f;

        [Header("Decays")]
        public float decayEvil = 1.0f;
        public float decayChill = 1.0f;
        public float decayJoy = 1.0f;
        public float decayPower = 1.0f;

        [Header("Charge Power")]
        public float bullyCharge = 1.0f;
        public float chillCharge = 1.0f;
        public float punshPoints = 10f;

        [Header("Current State")]
        public bool bullyActive = false;
        public bool punshedAngel = false;
        public bool isChilled = false;

        public GameObject fireObject;

        public WorldStates localStates;

        [Header("SubGoalsBools")]
        public bool isWorking = false;
        public bool isTransporting = false;
        public bool isBuilding = false;

        [Header("BuildingStates")]
        public bool choosenOne = false;
        public bool buildingAction = false;

        //To choose if the unit search for farest or nearest Buildings when transport something 
        public bool preferClosest = true;
        public bool PreferClosest => preferClosest;

        private bool checkAction = false;

        private static Devil activeDevil;

        public GameObject grave;

        private const string AvialableDevilKey = "Avail_devil";
        private const string UIAvialableDevilKey = "UI_Avail_devil";

        void Awake()
        {
            localStates = new WorldStates();
        }

        protected override void Start()
        {
            base.Start();
            SubGoal s1 = new SubGoal("Survive", 1, true);
            goals.Add(s1, 3);

            StartCoroutine("LostOverTimeDevil");

            if (targetRendererObject == null)
            {
                Debug.LogError("Kein Zielobjekt für den MeshRenderer zugewiesen!");
                return;
            }

            targetMeshRenderer = targetRendererObject.GetComponent<MeshRenderer>();
            if (targetMeshRenderer == null)
            {
                Debug.LogError($"Kein MeshRenderer im Zielobjekt '{targetRendererObject.name}' gefunden!");
                return;
            }

            Material[] materials = targetMeshRenderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].name.Contains(targetMaterialName))
                {
                    targetMaterialIndex = i;
                    break;
                }
            }

            if (targetMaterialIndex == -1)
            {
                Debug.LogWarning($"Kein Material mit dem Namen '{targetMaterialName}' gefunden!");
            }
        }

        private void Update()
        {
            if (needEvil > 100)
            {
                needEvil = 100;
            }

            if (needChill > 100)
            {
                needChill = 100;
            }

            if (needJoy > 100)
            {
                needJoy = 100;
            }

            if (needPower > 100)
            {
                needPower = 100;
            }

            if (needEvil <= 0 || needChill <= 0 || needJoy <= 0 || needPower <= 0)
            {
                Instantiate(grave, transform.position, transform.rotation);
                Destroy(gameObject);
            }

            if (Input.GetMouseButtonDown(0) && !buildingAction)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        SetActiveDevil(this);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1) && !buildingAction) 
            {
                if (!isBuilding) 
                {
                    targetBuildUI.SetActive(false);
                }

                targetNeedUI.SetActive(false);

                ToggleCanvas(false);
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !buildingAction)
            {
                DisableUI();
            }

            if (buildingAction && !checkAction)
            {
                checkAction = true;
                DisableUI();
                ToggleCanvas(false);
            }
        }

        private void OnEnable()
        {
            AddDevilState();
            AddUIDevilState();
            AssignUIReferences();
        }

        private void OnDestroy()
        {
            RemoveDevilState();
            RemoveUIDevilState();
        }

        public void ActivateUIForBuildings()
        {
            ActivateUIElement(targetUIForBuildings);
        }

        public void ActivateUIForNeeds()
        {
            ActivateUIElement(targetUIForNeeds);
        }

        /*public void ActivateUIForTransport()
        {
            ActivateUIElement(targetUIForTransport);
        }

    private void ActivateUIElement(GameObject uiElement)
    {
        if (uiElement == null)
        {
            Debug.LogError("UI-Element ist null.");
            return;
        }

        DeactivateAllUIElements();

        uiElement.SetActive(true);
    }

    private void DeactivateAllUIElements()
    {
        if (targetUIForBuildings != null)
        {
            targetUIForBuildings.SetActive(false);
        }
        if (targetUIForNeeds != null)
        {
            targetUIForNeeds.SetActive(false);
        }
        if (targetUIForTransport != null)
        {
            targetUIForTransport.SetActive(false);
        }
    }

    /// <summary>
    /// Activate and Deactivate choosen Devil UI and Shader, so that only one Devil can be selected 
    /// </summary>

    public static void SetActiveDevil(Devil newActiveDevil)
    {
        if (activeDevil != null && activeDevil != newActiveDevil)
        {
            activeDevil.Deactivate();
        }

        foreach (Angel angel in FindObjectsByType<Angel>(FindObjectsSortMode.None))
        {
            angel.Deactivate();
        }

        activeDevil = newActiveDevil;
        newActiveDevil.Activate();
    }

    private void Activate()
    {
        choosenOne = true;

        if (targetMeshRenderer != null && targetMaterialIndex != -1)
        {
            Material[] materials = targetMeshRenderer.materials;
            materials[targetMaterialIndex].SetFloat("_Opacity", 1.0f);
            targetMeshRenderer.materials = materials;
        }

        ToggleCanvas(true);

        Debug.Log($"{name} is active.");
    }

    public void Deactivate()
    {
        choosenOne = false;
        DisableUI();

        if (targetMeshRenderer != null && targetMaterialIndex != -1)
        {
            Material[] materials = targetMeshRenderer.materials;
            materials[targetMaterialIndex].SetFloat("_Opacity", 0.0f);
            targetMeshRenderer.materials = materials;
        }

        ToggleCanvas(false);

        Debug.Log($"{name} was deactivate.");
    }

    private void DisableUI()
    {
        targetBuildUI.SetActive(false);
        targetNeedUI.SetActive(false);
    }

    public void AssignUIReferences()
    {
        GameObject canvas = GameObject.Find("MainCanvas");
        if (canvas != null)
        {

            targetBuildUI = canvas.transform.Find("DevilBuidlings").gameObject;
            targetNeedUI = canvas.transform.Find("ShowDevilUINeeds").gameObject;
            targetUIForBuildings = canvas.transform.Find("DevilBuidlings").gameObject;
            targetUIForNeeds = canvas.transform.Find("ShowDevilUINeeds").gameObject;
            //targetUIForTransport = canvas.transform.Find("Timer").gameObject;
        }
        else
        {
            Debug.LogError("Canvas nicht gefunden!");
        }
    }


    /// <summary>
    /// UI States are there for the visibility for the Player, because the other State will be removed when the Devil is not available
    /// So the UI shows always the current count of Devils and the normal Devil State is hidden for the player and only important for the System
    /// </summary>

    public void AddDevilState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(AvialableDevilKey))
        {
            worldStates.SetState(AvialableDevilKey, 1);
            Debug.Log($"Angel added. Current count: {worldStates.GetStates()[AvialableDevilKey]}");
        }
        else
        {
            worldStates.ModifyState(AvialableDevilKey, +1);
            Debug.Log($"Angel added. Current count: {worldStates.GetStates()[AvialableDevilKey]}");
        }
    }

    public void RemoveDevilState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(AvialableDevilKey))
        {
            int currentCount = worldStates.GetStates()["Avail_devil"];
            if (currentCount > 0)
            {
                worldStates.ModifyState(AvialableDevilKey, -1);
            }
        }

    }

    public void AddUIDevilState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (!worldStates.HasState(UIAvialableDevilKey))
        {
            worldStates.SetState(UIAvialableDevilKey, 1);
            Debug.Log($"Angel added. Current count: {worldStates.GetStates()[UIAvialableDevilKey]}");
        }
        else
        {
            worldStates.ModifyState(UIAvialableDevilKey, +1);
            Debug.Log($"Angel added. Current count: {worldStates.GetStates()[UIAvialableDevilKey]}");
        }
    }

    public void RemoveUIDevilState()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState(UIAvialableDevilKey))
        {
            int currentCount = worldStates.GetStates()["UI_Avail_devil"];
            if (currentCount > 0)
            {
                worldStates.ModifyState(UIAvialableDevilKey, -1);
            }
        }
    }
    IEnumerator LostOverTimeDevil()
    {
        while (true)
        {
            needEvil -= decayEvil;
            needChill -= decayChill;
            needJoy -= decayJoy;
            needPower -= decayPower;

            if (bullyActive)
            {
                needEvil += bullyCharge;
            }

            if (isChilled)
            {
                needChill += chillCharge;
            }

            if (punshedAngel)
            {
                needEvil += punshPoints;
                punshedAngel = false;
            }



            yield return new WaitForSeconds(1f);
        }
    }

    private void ToggleCanvas(bool state)
    {
        if (objectCanvas != null)
        {
            objectCanvas.SetActive(state);
        }

        if (targetMeshRenderer != null && targetMaterialIndex != -1)
        {
            Material[] materials = targetMeshRenderer.materials;

            if (state)
            {
                materials[targetMaterialIndex].SetFloat("_Opacity", 1.0f);
            }
            else
            {
                materials[targetMaterialIndex].SetFloat("_Opacity", 0.0f);
            }
            targetMeshRenderer.materials = materials;
        }
    }

    public void StartBuilding()
    {
        isBuilding = true;
    }

    public void StartWorking()
    {
        isWorking = true;
    }

    public void StartTransporting()
    {
        isTransporting = true;
    }
}

*/

}
