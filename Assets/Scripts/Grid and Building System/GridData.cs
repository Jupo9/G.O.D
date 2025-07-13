using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData 
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();
    private HashSet<Vector3Int> blockedCells = new();

    public void BlockCell(Vector3Int gridPosition)
    {
        blockedCells.Add(gridPosition);
    }

    public void BlockCells(IEnumerable<Vector3Int> gridPositions)
    {
        foreach (var pos in gridPositions)
        {
            blockedCells.Add(pos);
        }

    }

    public bool IsCellBlocked(Vector3Int pos)
    {
        return blockedCells.Contains(pos);
    }

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        
        foreach(var pos in positionToOccupy) 
        {
            if (placedObjects.ContainsKey(pos) || blockedCells.Contains(pos))
            {
                throw new Exception($"Cell {pos} is either already placed or blocked.");
            }
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();

        for (int x = 0; x < objectSize.x; x++) 
        {
            for (int y = 0; y < objectSize.y; y++) 
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);

        foreach (var pos in positionToOccupy) 
        {
            if (placedObjects.ContainsKey(pos) || blockedCells.Contains(pos))
            {
                return false;
            }
        }
        return true;
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
        {
            return -1;
        }

        return placedObjects[gridPosition].PlacedObjectIndex;

    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiedPositions) 
        {
            placedObjects.Remove(pos);
        }
    }
}


public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}