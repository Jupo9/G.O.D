
public enum ResourceType
{
    Altar,
    Mine,
    Locked
}

public class ResourceManager
{
    public string key;
    public ResourceType resourceType;
    public IResourceManager provider;

    public ResourceManager(string key, ResourceType resourceType, IResourceManager provider)
    {
        this.key = key;
        this.resourceType = resourceType;
        this.provider = provider;
    }
}
