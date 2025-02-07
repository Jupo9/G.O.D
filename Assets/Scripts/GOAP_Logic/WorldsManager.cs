using System.Collections.Generic;
using UnityEngine;

public class WorldsManager : MonoBehaviour
{
    /// <summary>
    /// add at the beginning all existing states that were implimentet in the scene (global useage)
    /// </summary>

    [System.Serializable]
    public struct StateDefinition
    {
        public string key;
        public int value;
    }

    [SerializeField] private List<StateDefinition> initialStates;

    void Awake()
    {
        InitializeWorldStates();
    }

    /// <summary>
    /// Add ressources for Player in Storage, this is important because every ressource need to be activate by
    /// a methode for the world states and UI informations
    /// </summary>


    private void InitializeWorldStates()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        foreach (var state in initialStates)
        {
            if (!worldStates.HasState(state.key))
            {
                worldStates.SetState(state.key, state.value);
                Debug.Log($"Initialized state '{state.key}' with value {state.value}");
            }
        }
    }
}

