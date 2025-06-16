using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField] private Material previewMaterialPrefab;
    private Material previewMaterialInstance;


    private Renderer cellIndicatorRenderer;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public GameObject CurrentPreviewObject
    {
        get { return previewObject; }
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);

        if (previewObject != null)
        {
            ResetPreviewMaterial(previewObject);
            previewObject = null; 
        }
    }

    private void ResetPreviewMaterial(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = new Material(renderer.sharedMaterials[i]);
            }
            renderer.materials = materials;
        }
    }

    public void UpdatePosition(Vector3 position, Vector2Int size, bool validity)
    {
        if (previewObject != null)
        {
            MovePreview(position, size);
            ApplyFeedbackToPreview(validity);
        }

        MoveCursor(position, size);
        ApplyFeedbackToCursor(validity);
    }

    public void UpdateRotation(Quaternion rotation)
    {
        Debug.Log($"UpdateRotation called with rotation: {rotation.eulerAngles}");
        previewObject.transform.rotation = rotation;
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;

        c.a = 0.5f;

        previewMaterialInstance.color = c;
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        cellIndicatorRenderer.material.color = c;
        c.a = 0.5f;
    }

    // Show Cell Indicator (Outlines for Grid Size of an Object or in this case the Pentagram in the Preview for the grid size)

    private void MoveCursor(Vector3 position, Vector2Int size)
    {
        cellIndicator.transform.position = new Vector3(position.x, 0f, position.z);
    }

    private void MovePreview(Vector3 position, Vector2Int size)
    {
        Vector3 centeredPosition = new Vector3(position.x + size.x * 0.5f, 0.5f, position.z + size.y * 0.5f);
        previewObject.transform.position = centeredPosition;
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }

}