using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> placedGameObjects = new();
    [SerializeField] private Material previewMaterialPrefab;
    private List<GameObject> objectsInScene = new List<GameObject>();

    public int PlaceObject(GameObject prefab, Vector3 position, Quaternion rotation, bool isPreview = false)
    {
        GameObject placedObject = Instantiate(prefab, position, rotation);
        if (isPreview)
        {
            ApplyPreviewMaterial(placedObject);
        }

        objectsInScene.Add(placedObject);
        return objectsInScene.Count - 1;
    }

    private void ApplyPreviewMaterial(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = new Material(previewMaterialPrefab); 
            }
            renderer.materials = materials;
        }
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null) 
        {
            return;
        }
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
