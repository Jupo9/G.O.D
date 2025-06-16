using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance { get; private set; }

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private BuidlingDatabase database;

    [SerializeField] private GameObject gridVisualization;

    [SerializeField] private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObstacleData obstacleData;

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

        if (obstacleData != null)
        {
            floorData.BlockCells(obstacleData.GetAllBlockedCells());
        }
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();

        // to make sure that only one time the inputs are called and to avoid bugs,
        // the inputManager will first get deselect(-) and than seleceted (+) -> alternative OnEnable and OnDisable
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        inputManager.OnRotate -= RotatePreview;

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
        inputManager.OnRotate += RotatePreview;

        gridVisualization.SetActive(true);

        buildingState = new PlacementState(ID, grid, previewSystem, database, floorData, buildingData, objectPlacer);
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