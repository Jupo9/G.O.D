

public sealed class Worlds
{
    private static readonly Worlds instance = new Worlds();
    private static WorldStates world;

    static Worlds()
    {
        world = new WorldStates();
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnStateChange += HandleStateChange;
        }
    }

    private Worlds()
    {

    }

    public static Worlds Instance
    {
        get 
        { 
            return instance;
        }
    }

    public WorldStates GetWorld()
    { 
        return world;
    }

    public void InitializeStates()
    {
        //world.ModifyState("evil", 1);
    }

    private static void HandleStateChange(string stateKey, int valueChange)
    {
        world.UpdateStateBasedOnEvent(stateKey, valueChange);
    }
}