using System.Collections.Generic;
using UnityEngine;

public class ObstacleData : MonoBehaviour
{
    [Tooltip("Obstacle have an offset from -0.5, if you want 7.5 you need 7")]
    public List<ObstacleArea> obstacleAreas;

    [Header("Visualisierung")]
    [SerializeField] private bool showCornerMarkers = true;
    [SerializeField] private Material markerMaterial;

    private List<GameObject> spawnedMarkers = new();

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
}
