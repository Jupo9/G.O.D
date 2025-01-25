using UnityEngine;

public class ChangeBuildState : MonoBehaviour
{
    /// <summary>
    /// this is for the building and preview systema after placing important. it fives bools to the buidlings
    /// an enum, and switch states could be a better solution but this work quite well
    /// </summary>
    public MonoBehaviour buildingManagerObject;

    public bool changeBuildingState = false;
    public bool buildingIsComplete = false;

    void Update()
    {
        if (changeBuildingState)
        {
            if (buildingManagerObject != null)
            {
                var method = buildingManagerObject.GetType().GetMethod("ChangePreviewState");
                if (method != null)
                {
                    method.Invoke(buildingManagerObject, null);
                    changeBuildingState = false;
                }
            }
        }
    }
}
