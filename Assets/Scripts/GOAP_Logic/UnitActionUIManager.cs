using UnityEngine;

public class UnitActionUIManager : MonoBehaviour
{
    public static UnitActionUIManager Instance;

    public WorkAndTransportButton workAndTransport;

    private void Awake()
    {
        Instance = this;
    }

    public void SetSelectedAgent(Agents agent)
    {
        workAndTransport.SetSelectedAgent(agent);
    }
}
