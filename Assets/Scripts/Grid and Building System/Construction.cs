using System.Collections;
using System.Linq;
using UnityEngine;

public class Construction : MonoBehaviour
{
    public PreviewData previewData;
    private Agents assignedBuilder;

    private bool isBeingBuilt  = false;

    private Coroutine builderCheckCoroutine;

    private void Start()
    {
        builderCheckCoroutine = StartCoroutine(CheckForBuilderRoutine());
    }

    public void AssignBuilder(Agents builder)
    {
        assignedBuilder = builder;
    }

    public void StartConstruction()
    {
        if (isBeingBuilt || assignedBuilder == null)
        {
            return;
        }

        isBeingBuilt = true;

        Actions buildAction = assignedBuilder.actions.FirstOrDefault(a => a is GA_Building);

        if (buildAction != null)
        {
            ((GA_Building)buildAction).SetConstructionTarget(this);

            Debug.Log("Call Temporary Action for " + buildAction.actionName);
            assignedBuilder.TemporaryAction(buildAction);
        }
        else
        {
            Debug.LogWarning("there is no correct building action!");
        }
    }

    public BuilderType GetBuilderType() => previewData.builderType;

    private IEnumerator CheckForBuilderRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            if (assignedBuilder == null)
            {
                Debug.Log("[Construction] No builder assigned, trying to find a new one...");
                TryFindNewBuilder();
                continue;
            }

            if (assignedBuilder.gameObject == null || !assignedBuilder.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("[Construction] Assigned builder no longer exists!");
                assignedBuilder = null;
                isBeingBuilt = false;
                TryFindNewBuilder();
                continue;
            }

            if (!assignedBuilder.HasActiveTemporaryAction)
            {
                Debug.LogWarning("[Construction] Builder is idle – restarting assignment.");
                assignedBuilder = null;
                isBeingBuilt = false;
                TryFindNewBuilder();
            }
        }
    }

    private void TryFindNewBuilder()
    {
        Agents newBuilder = null;

        if (previewData.builderType == BuilderType.Angel)
        {
            newBuilder = RegisterAngelDevil.Instance.GetBestAvailableAngel();
        }
        else if (previewData.builderType == BuilderType.Devil)
        {
            newBuilder = RegisterAngelDevil.Instance.GetWorstAvailableDevil();
        }

        if (newBuilder != null)
        {
            Debug.Log("[Construction] New builder assigned: " + newBuilder.name);
            AssignBuilder(newBuilder);
            StartConstruction();
        }
        else
        {
            Debug.Log("[Construction] Still no available builder.");
        }
    }
}
