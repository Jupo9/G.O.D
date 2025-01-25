using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    /// <summary>
    /// In this Script the object will shown and placed
    /// by using this the object get enabled, this can caused some isses!
    /// also the roation need to be adjust and removing is obsolete after preview
    /// or need to be changed to cancel building and get ressoruces back
    /// </summary>
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

    //ApplyPreviewMaterial need to be active after placed, i think there is a better solution for this
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

    //register object, good for a remove after build methode
    public int RegisterPlacedObject(GameObject placedObject)
    {
        objectsInScene.Add(placedObject);
        return objectsInScene.Count - 1;
    }
}

