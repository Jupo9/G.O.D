using UnityEngine;

public class ChangeBuildState : MonoBehaviour
{
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
