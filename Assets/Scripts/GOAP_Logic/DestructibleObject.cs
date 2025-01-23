using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public string stateKey = "foodAvailable";
    public int valueChangeOnDestruction = -1;

    private void Start()
    {
        Worlds.Instance.GetWorld().SetState(stateKey, 3);

        Debug.Log($"State '{stateKey}' set to: {Worlds.Instance.GetWorld().GetStates()[stateKey]}");
    }

    private void OnDestroy()
    {
        Debug.Log("Test Destruction");
        if (EventManager.Instance == null)
        {
            Debug.LogError("EventManager.Instance ist null!");
        }
        else
        {
            EventManager.Instance.TriggerStateChange(stateKey, valueChangeOnDestruction);
            Debug.Log($"OnDestroy triggered. State '{stateKey}' changed by {valueChangeOnDestruction}. New value: {Worlds.Instance.GetWorld().GetStates()[stateKey]}");
        }
    }
}
