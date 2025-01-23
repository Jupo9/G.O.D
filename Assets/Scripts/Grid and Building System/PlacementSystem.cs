using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance { get; private set; }

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private BuidlingDatabase database;

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

    [SerializeField] private PreviewSystem previewSystem;
    [SerializeField] private ObjectPlacer objectPlacer;

    IBuildingState buildingState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        gridVisualization.SetActive(false);
        floorData = new();
        buildingData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);

        buildingState = new PlacementState(ID, grid, previewSystem, database, floorData, buildingData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
        inputManager.OnRotate += RotatePreview;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, previewSystem, floorData, buildingData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);

    }

    public void StopPlacement()
    {
        if (buildingState == null)
        {
            return;
        }

        gridVisualization.SetActive(false);

        buildingState.EndState();
        previewSystem.StopShowingPreview();

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;

        buildingState = null;
    }

    private void RotatePreview()
    {
        if (buildingState != null && buildingState is PlacementState placementState)
        {
            placementState.RotatePreview();
        }
    }

    private void Update()
    {
        if (buildingState == null)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }
}