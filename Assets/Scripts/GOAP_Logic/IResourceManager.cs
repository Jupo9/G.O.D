public interface IResourceManager
{
    /// <summary>
    /// THIS SCRIPT CONTAINS:
    /// Interface logic for Locking Resources to deny conflicts with to many request for only one Resource
    /// </summary>
    /// <returns></returns>
    bool HasAvailableResource(string resourceType);
    bool LockResource(string resourceType);
    void ReleaseLock(string resourceType);
    void ConsumeLockedRessource(string resourceType);
}

