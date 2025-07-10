using System.Collections.Generic;
using UnityEngine;

public class ObstacleData : MonoBehaviour
{
    [HideInInspector]
    public List<ObstacleArea> obstacleAreas = new();

    [Tooltip("Only activated to see placed obstacle pillars!")]
    [Header("Visualisierung")]
    [SerializeField] private bool showCornerMarkers = true;
    [SerializeField] private Material markerMaterial;

    private List<GameObject> spawnedMarkers = new();

    private void Awake()
    {
        PlacedObstacleAreas();
    }

    private void Start()
    {
        if (showCornerMarkers)
        {
            SpawnCornerMarkers();
        }
    }

    public IEnumerable<Vector3Int> GetAllBlockedCells()
    {
        foreach (var area in obstacleAreas)
        {
            foreach (var cell in area.GetAllCoveredCells())
            {
                yield return cell;
            }
        }
    }

    private void SpawnCornerMarkers()
    {
        foreach (var area in obstacleAreas)
        {
            CreateMarker(area.CornerA);
            CreateMarker(area.CornerB);
        }
    }


    private void CreateMarker(Vector3Int gridPosition)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = gridPosition + new Vector3(0.5f, 2.5f, 0.5f);
        cube.transform.localScale = new Vector3(0.5f, 5f, 0.5f);
        cube.transform.SetParent(this.transform);

        if (markerMaterial != null)
        {
            var renderer = cube.GetComponent<Renderer>();
            renderer.material = markerMaterial;
        }
        else
        {
            cube.GetComponent<Renderer>().material.color = Color.red;
        }

        spawnedMarkers.Add(cube);
    }

    private void PlacedObstacleAreas()
    {
        obstacleAreas = new List<ObstacleArea>
        {
            new ObstacleArea //Summon
            {
                CornerA = new Vector3Int(2, 0, 20),
                CornerB = new Vector3Int(0, 0, 18)
            },
            new ObstacleArea //Heat
            {
                CornerA = new Vector3Int(10, 0, 14),
                CornerB = new Vector3Int(7, 0, 11)
            },
            new ObstacleArea //Stain
            {
                CornerA = new Vector3Int(25, 0, 20),
                CornerB = new Vector3Int(21, 0, 16)
            },
            new ObstacleArea //Spirit
            {
                CornerA = new Vector3Int(10, 0, -12),
                CornerB = new Vector3Int(7, 0, -15)
            },
            new ObstacleArea //Purity
            {
                CornerA = new Vector3Int(25, 0, -17),
                CornerB = new Vector3Int(21, 0, -21)
            },
            new ObstacleArea //Believe
            {
                CornerA = new Vector3Int(2, 0, -19),
                CornerB = new Vector3Int(0, 0, -21)
            },
            new ObstacleArea //Mine one
            {
                CornerA = new Vector3Int(-7, 0, -9),
                CornerB = new Vector3Int(-11, 0, -13)
            },
            new ObstacleArea //GOD
            {
                CornerA = new Vector3Int(6, 0, 5),
                CornerB = new Vector3Int(-7, 0, -6)
            },
            new ObstacleArea //Mine 2
            {
                CornerA = new Vector3Int(-17, 0, -9),
                CornerB = new Vector3Int(-21, 0, -13)
            },
            new ObstacleArea //Altar
            {
                CornerA = new Vector3Int(-12, 0, -17),
                CornerB = new Vector3Int(-16, 0, -21)
            }
        };
    }
}
