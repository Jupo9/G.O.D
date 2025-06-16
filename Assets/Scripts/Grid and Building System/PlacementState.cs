using UnityEngine;

public class PlacementState : IBuildingState
{
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
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        if (!placementValidity)
        {
            return;
        }

        GameObject placedObject = previewSystem.CurrentPreviewObject;

        if (placedObject == null)
        {
            throw new System.Exception("Preview object is not available.");
        }

        previewSystem.StopShowingPreview();

        Vector2Int size = database.objectsData[selectedObjectIndex].Size;
        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        Vector3 centeredPosition = new Vector3(
            worldPosition.x + size.x * 0.5f,
            0.5f,
            worldPosition.z + size.y * 0.5f
        );

        placedObject.transform.position = centeredPosition;

        int index = objectPlacer.RegisterPlacedObject(placedObject);

        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : buildingData;

        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index);

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