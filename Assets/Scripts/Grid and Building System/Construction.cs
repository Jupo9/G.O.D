using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Construction : MonoBehaviour
{
    public PreviewData previewData;
    private Agents assignedBuilder;

    private bool isBeingBuilt  = false;

    /*[System.Obsolete]
    void Start()
    {
        Agents devilBuilder = FindObjectsOfType<Agents>().FirstOrDefault(a => a.unitType == Agents.UnitType.Devil);

        if (devilBuilder != null)
        {
            StartConstruction(devilBuilder);
        }
    }*/

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

        //StartCoroutine(BuildTimer());
    }

    /*private IEnumerator BuildTimer()
    {
        yield return new WaitForSeconds(previewData.buildTime);

        CompleteConstruction();
    }

    private void CompleteConstruction()
    {
        Instantiate(previewData.buildingPrefab, transform.position,transform.rotation);
        Destroy(gameObject);
    }*/

    public BuilderType GetBuilderType() => previewData.builderType;
}
