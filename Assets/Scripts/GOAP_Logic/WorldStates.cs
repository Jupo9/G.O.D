using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldState
{
    /// <summary>
    /// "key" represents the state that the Object get or have when it execute an action
    /// for example hungy as key state before the npc get some food, and after the npc ate something the state changed to saturated
    /// "value" is the number of keys that exist in the scene
    /// for example 2 foodstands that are available
    /// </summary>
    public string key;
    public int value;
}

public class WorldStates
{
    public Dictionary<string, int> states;

    public WorldStates()
    {
        states = new Dictionary<string, int>();
    }

    public bool HasState(string key)
    {
        if (states == null)
        {
            return false;
        }

        return states.ContainsKey(key);
    }

    void AddState(string key, int value)
    {
        if (!states.ContainsKey(key))
        {
            states.Add(key, value);
        }
    }

    public void ModifyState(string key, int value)
    {
        if (states.ContainsKey(key))
        {
            states[key] += value;
            if (states[key] <= 0)
            {
                RemoveState(key);
            }
        }
        else
        {
            states.Add(key, value);
        }
    }

    public void RemoveState(string key)
    {
        if (states.ContainsKey(key))
        {
            states.Remove(key);
        }
    }

    public void UpdateStateBasedOnEvent(string key, int valueChange)
    {
        if (states.ContainsKey(key))
        {
            states[key] += valueChange;
            if (states[key] <= 0)
            {
                RemoveState(key);
            }
        }
        else
        {
            Debug.Log($"State '{key}' does not exist. Adding it dynamically.");
            AddState(key, valueChange);
        }
    }

    public void SetState(string key, int value)
    {
        if (states.ContainsKey(key))
        {
            states[key] = value;
        }
        else
        {
            states.Add(key, value); 
        }
    }

    public Dictionary<string, int> GetStates()
    {
        return states;
    }
}

