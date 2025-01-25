using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuidlingDatabase : ScriptableObject
{
    /// <summary>
    /// store information for buildings, like size, name, ID or the prefab
    /// for the placement system, only buildings that get in the DataBase can be placed
    /// </summary>
    public List<ObjectData> objectsData;
}

[Serializable]
public class ObjectData
{
    [field: SerializeField] public string Name {  get; private set; }
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field: SerializeField] public GameObject Prefab { get; private set; }
}
