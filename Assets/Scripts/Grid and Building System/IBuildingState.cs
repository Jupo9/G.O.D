using UnityEngine;

public interface IBuildingState
{
    /// <summary>
    /// Interface for Buidling States
    /// </summary>
    void EndState();
    void OnAction(Vector3Int gridPosition);
    void UpdateState(Vector3Int gridPosition);
}