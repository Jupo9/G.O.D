using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObstacleArea
{
    public Vector3Int CornerA;
    public Vector3Int CornerB;

    public List<Vector3Int> GetAllCoveredCells()
    {
        List<Vector3Int> cells = new();

        int minX = Mathf.Min(CornerA.x, CornerB.x);
        int maxX = Mathf.Max(CornerA.x,CornerB.x);
        int minZ = Mathf.Min(CornerA.z, CornerB.z);
        int maxZ = Mathf.Max(CornerA.z, CornerB.z);


        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                cells.Add(new Vector3Int(x, 0, z));
            }
        }

        return cells;
    }
}
