public class ResourceWatcher
{
    public static event System.Action OnWorldResourceChanged;

    public static void Notify()
    {
        OnWorldResourceChanged?.Invoke();
    }
}
