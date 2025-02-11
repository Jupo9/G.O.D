using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    /// <summary>
    /// generate objects in a scene around a max and min area
    /// the min area will colored green for now
    /// calculate all max values to get a random shape
    /// with createNew, a new shape will be create
    /// maxWSxPlus stands for "maximum Worldsize x in positiv direction (+)"
    /// add a dictonary for the tiles to create rules and methodes how the tiles should work
    /// </summary>
    [SerializeField] private GameObject fieldObject;

    [SerializeField] private int maxWSxPlus = 50;
    [SerializeField] private int maxWSxMinus = 50;
    [SerializeField] private int maxWSzPlus = 50;
    [SerializeField] private int maxWSzMinus = 50;

    [SerializeField] private int minWSxPlus = 15;
    [SerializeField] private int minWSxMinus = 15;
    [SerializeField] private int minWSzPlus = 15;
    [SerializeField] private int minWSzMinus = 15;

    [SerializeField] private int gridOffset = 1;

    [SerializeField] private float falloffFactor = 2f;

    public bool createNew = false;

    private Dictionary<Vector2Int, GameObject> placedTiles = new Dictionary<Vector2Int, GameObject>();

    private void Start()
    {
        CreateWorld();
    }

    private void Update()
    {
        if (createNew)
        {
            createNew = false;
            GenerateNewMap();
        }
    }

    public void GenerateNewMap()
    {
        ClearWorld();
        CreateWorld();
    }


    /// <summary>
    /// First clear all Dictonary informations about old tiles
    /// calculate tiles spawn
    /// save tiles data in Dictonary
    /// then remove all tiles that are not connected with an other tiles horizontal or veritcal [RemoveIsolatedTiles]
    /// after this check if gaps exists with only 1 gap (hor. and ver. connected tiles spawned) and if this is the case,
    /// then spawn the tile and fill the gap [FillGaps + SpawnTiles]
    /// </summary>
    private void CreateWorld()
    {
        placedTiles.Clear(); 

        for (int x = -maxWSxMinus; x <= maxWSxPlus; x++)
        {
            for (int z = -maxWSzMinus; z <= maxWSzPlus; z++)
            {
                float probability = GetSpawnProbability(x, z);
                bool isInMinimumArea = (x >= -minWSxMinus && x <= minWSxPlus && z >= -minWSzMinus && z <= minWSzPlus);

                if (isInMinimumArea || Random.value < probability)
                {
                    Vector3 pos = new Vector3(x * gridOffset, 0, z * gridOffset);
                    GameObject field = Instantiate(fieldObject, pos, Quaternion.identity);
                    field.transform.SetParent(this.transform);

                    if (isInMinimumArea)
                    {
                        field.GetComponent<Renderer>().material.color = Color.green;
                    }

                    Vector2Int gridPos = new Vector2Int(x, z);
                    placedTiles[gridPos] = field;
                }
            }
        }

        RemoveIsolatedTiles();
        FillGaps();
    }


    /// <summary>
    /// This methode calculate the chance for a spawning object.
    /// first it calculates the min size, that nothing get deleted in this area.
    /// then it calculates the max size, that's the border/end of the area.
    /// calculate distance between tiles.
    /// and calculate chance for a tile to spawn, with every tile that comes closer to the border,
    /// the chance for the tile to spawn falls ouf
    /// </summary>
    private float GetSpawnProbability(int x, int z)
    {
        int distX = Mathf.Max(0, Mathf.Abs(x) - Mathf.Min(minWSxPlus, minWSxMinus));
        int distZ = Mathf.Max(0, Mathf.Abs(z) - Mathf.Min(minWSzPlus, minWSzMinus));

        int maxDistX = Mathf.Max(1, Mathf.Max(maxWSxPlus, maxWSxMinus) - Mathf.Min(minWSxPlus, minWSxMinus));
        int maxDistZ = Mathf.Max(1, Mathf.Max(maxWSzPlus, maxWSzMinus) - Mathf.Min(minWSzPlus, minWSzMinus));

        float ratioX = (float)distX / maxDistX;
        float ratioZ = (float)distZ / maxDistZ;

        return Mathf.Pow(1 - Mathf.Max(ratioX, ratioZ), falloffFactor);
    }

    private void RemoveIsolatedTiles()
    {
        List<Vector2Int> toRemove = new List<Vector2Int>();

        foreach (var tile in placedTiles)
        {
            Vector2Int pos = tile.Key;
            bool isInMinimumArea = (pos.x >= -minWSxMinus && pos.x <= minWSxPlus && pos.y >= -minWSzMinus && pos.y <= minWSzPlus);

            if (isInMinimumArea)
            {
                continue;
            }

            bool hasNeighbor =
                placedTiles.ContainsKey(new Vector2Int(pos.x + 1, pos.y)) ||
                placedTiles.ContainsKey(new Vector2Int(pos.x - 1, pos.y)) ||
                placedTiles.ContainsKey(new Vector2Int(pos.x, pos.y + 1)) ||
                placedTiles.ContainsKey(new Vector2Int(pos.x, pos.y - 1));

            if (!hasNeighbor)
            {
                toRemove.Add(pos);
            }
        }

        foreach (Vector2Int pos in toRemove)
        {
            Destroy(placedTiles[pos]);
            placedTiles.Remove(pos);
        }
    }

    private void FillGaps()
    {
        List<Vector2Int> toAdd = new List<Vector2Int>();

        for (int x = -maxWSxMinus; x <= maxWSxPlus; x++)
        {
            for (int z = -maxWSzMinus; z <= maxWSzPlus; z++)
            {
                Vector2Int pos = new Vector2Int(x, z);

                if (!placedTiles.ContainsKey(pos)) // Falls kein Tile existiert
                {
                    bool hasAllNeighbors =
                        placedTiles.ContainsKey(new Vector2Int(pos.x + 1, pos.y)) &&
                        placedTiles.ContainsKey(new Vector2Int(pos.x - 1, pos.y)) &&
                        placedTiles.ContainsKey(new Vector2Int(pos.x, pos.y + 1)) &&
                        placedTiles.ContainsKey(new Vector2Int(pos.x, pos.y - 1));

                    if (hasAllNeighbors)
                    {
                        toAdd.Add(pos);
                    }
                }
            }
        }
        foreach (Vector2Int pos in toAdd)
        {
            SpawnTile(pos.x, pos.y, false);
        }
    }

    private void SpawnTile(int x, int z, bool isMinArea)
    {
        Vector3 pos = new Vector3(x * gridOffset, 0, z * gridOffset);
        GameObject field = Instantiate(fieldObject, pos, Quaternion.identity);
        field.transform.SetParent(this.transform);

        if (isMinArea)
        {
            field.GetComponent<Renderer>().material.color = Color.green;
        }

        Vector2Int gridPos = new Vector2Int(x, z);
        placedTiles[gridPos] = field;
    }

    private void ClearWorld()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
