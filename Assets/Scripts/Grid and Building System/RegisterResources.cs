using UnityEngine;

public class RegisterResources : MonoBehaviour
{
    public ResourceType resourceType;

    private IResourceManager resourceProvider;

    private void Start()
    {
        resourceProvider = GetComponent<IResourceManager>();

        if (resourceProvider == null)
        {
            Debug.LogError("IResourceManager not found " + gameObject.name);
            return;
        }

        ResourceCalculator.Instance.RegisterResourceSource(new ResourceManager("Res_fire", resourceType, resourceProvider));

        ResourceCalculator.Instance.RegisterResourceSource(new ResourceManager("Res_light", resourceType, resourceProvider));
    }
}
