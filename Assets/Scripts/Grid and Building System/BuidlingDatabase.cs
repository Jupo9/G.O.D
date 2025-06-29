using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuidlingDatabase : ScriptableObject
{
    public List<ObjectData> objectsData;
}

[Serializable]
public class ObjectData
{
    [field: SerializeField] public string Name {  get; private set; }
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;

    [field: SerializeField] public PreviewData previewData { get; private set; }

    public GameObject Prefab => previewData != null ? previewData.previewPrefab : null;
}
