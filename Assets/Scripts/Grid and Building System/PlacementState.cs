using UnityEngine;

public class PlacementState : IBuildingState
{
    private bool hasPlacedObject = false;

    private int selectedObjectIndex = -1;
    private Quaternion currentRotation = Quaternion.identity;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    BuidlingDatabase database;
    GridData floorData;
    GridData buildingData;
    ObjectPlacer objectPlacer;

    public PlacementState(int iD, Grid grid, PreviewSystem previewSystem, BuidlingDatabase database, GridData floorData, GridData buildingData, ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.buildingData = buildingData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);

        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab, database.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"No object with ID {iD}");
        }
    }

    public void EndState()
    {
        if (!hasPlacedObject)
        {
            previewSystem.StopShowingPreview();
        }
    }

    public void OnAction(Vector3Int gridPosition)
    {
        hasPlacedObject = true;

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        if (!placementValidity)
        {
            return;
        }

        var objectData = database.objectsData[selectedObjectIndex];
        var previewData = objectData.previewData;

        int fireCost = previewData.buildFireCosts;
        int lightCost = previewData.buildLightCosts;

        bool fireOK = ResourceCalculator.Instance.TypConsumeResources("Res_fire", fireCost);
        bool lightOK = ResourceCalculator.Instance.TypConsumeResources("Res_light", lightCost);

        if (!fireOK || !lightOK)
        {
            Debug.LogWarning("Not enough resources for: " + previewData.buildingName);
            return;
        }

        Vector2Int size = objectData.Size;
        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        Vector3 centeredPosition = new Vector3(worldPosition.x + size.x * 0.5f, 0.5f, worldPosition.z + size.y * 0.5f);

        GameObject previewInstance = previewSystem.CurrentPreviewObject;

        if (previewInstance == null)
        {
            Debug.LogError("No preview instance available.");
            return;
        }

        previewInstance.transform.position = centeredPosition;
        previewInstance.transform.rotation = currentRotation;

        Construction construction = previewInstance.GetComponent<Construction>();

        if (construction != null)
        {
            construction.previewData = previewData;

            BuildingTaskManager.Instance.EnqueueTask(construction);

            Agents assignedAgent = null;

            if (previewData.builderType == BuilderType.Angel)
            {
                assignedAgent = RegisterAngelDevil.Instance.GetBestAvailableAngel();
            }
            else if (previewData.builderType == BuilderType.Devil)
            {
                assignedAgent = RegisterAngelDevil.Instance.GetWorstAvailableDevil();
            }

            if (assignedAgent != null)
            {
                construction.AssignBuilder(assignedAgent);
                construction.StartConstruction();
            }
            else
            {
                Debug.LogWarning($"No available {previewData.builderType} for {previewData.buildingName}. Task remains in queue.");
            }
        }
        else
        {
            Debug.LogError("Preview-Object missing Constuction Script!");
        }

        int index = objectPlacer.RegisterPlacedObject(previewInstance);
        GridData selectedData = objectData.ID == 0 ? floorData : buildingData;
        selectedData.AddObjectAt(gridPosition, size, objectData.ID, index);

        previewSystem.HideCellIndicator();

        PlacementSystem.Instance.StopPlacement();
    }

    public void RotatePreview()
    {
        currentRotation *= Quaternion.Euler(0, 90, 0);
        previewSystem.UpdateRotation(currentRotation);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        Vector2Int size = database.objectsData[selectedObjectIndex].Size;

        bool isFreeOnFloor = floorData.CanPlaceObjectAt(gridPosition, size);
        bool isFreeOnBuildings = buildingData.CanPlaceObjectAt(gridPosition, size);
        return isFreeOnFloor && isFreeOnBuildings;
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        Vector2Int size = database.objectsData[selectedObjectIndex].Size;
        Vector3 worldPosition = grid.CellToWorld(gridPosition);

        previewSystem.UpdatePosition(worldPosition, size, placementValidity);
    }
}