using UnityEngine;

public class SelectionOutline : MonoBehaviour
{
    /// <summary>
    /// THIS SCRIPT CONTAINS:
    /// Public Outline Objects, no Shader. Simple Active/Deactivate methode
    /// </summary>

    [SerializeField] private GameObject outlineObject;

    public void SetOutline(bool active)
    {
        if (outlineObject != null)
        {
            outlineObject.SetActive(active);
        }
    }
}
