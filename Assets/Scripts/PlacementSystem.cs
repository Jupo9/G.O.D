using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private BuidlingDatabase database;
    private int selectedObjectIndex = -1;

    [SerializeField] private GameObject gridVisualization;

    [SerializeField] private Vector3Int lastDetectedPosition = Vector3Int.zero;

    /// <summary>
    /// this GridDatas have two options
    /// First, the floor is for placing other objects on it. You have to use the 0 as an ID or change the System here!
    /// Second, buildings are here for placing on floor or ground but you can't place something on it right now. Use IDs > 0!
    /// If you want to change this you need to change the logic "GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : buildingData;"
    /// </summary>
    private GridData floorData;
    private GridData buildingData;

    private List<GameObject> placedGameObject = new();

    [SerializeField] private PreviewSystem preview;

    private void Start()
    {
        StopPlacement();

        floorData = new();
        buildingData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);

        if (selectedObjectIndex < 0) 
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }

        gridVisualization.SetActive(true);
        preview.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab, database.objectsData[selectedObjectIndex].Size);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI()) 
        {
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        if (placementValidity == false) 
        {
            return;
        }

        GameObject gameObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        gameObject.transform.position = grid.CellToWorld(gridPosition);

        placedGameObject.Add(gameObject);

        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : buildingData;

        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            placedGameObject.Count - 1);

        preview.UpdatePosition(grid.CellToWorld(gridPosition), false);

    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : buildingData;

        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        preview.StopShowingPreview();

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;
    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
           bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
            mouseIndicator.transform.position = mousePosition;
            preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
            lastDetectedPosition = gridPosition;
        }
    }
}
