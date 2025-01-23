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
        placedObject.transform.position = grid.CellToWorld(gridPosition);
        placedObject.transform.rotation = currentRotation;

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
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : buildingData;

        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}