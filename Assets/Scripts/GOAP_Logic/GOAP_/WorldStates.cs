using System.Collections.Generic;

[System.Serializable]
public class WorldState
{
    public string key;
    public int value;
}

public class WorldStates
{
    public delegate void WorldStateChanged(string key, int newValue);
    public event WorldStateChanged OnStateChanged;
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
            OnStateChanged?.Invoke(key, states[key]);

            if (states[key] <= 0)
            {
                RemoveState(key);
            }
        }
        else
        {
            states.Add(key, value);
            OnStateChanged?.Invoke(key, value);
        }
    }

    public void RemoveState(string key)
    {
        if (states.ContainsKey(key))
        {
            states.Remove(key);
            OnStateChanged?.Invoke(key, 0);
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
            //Debug.Log($"State '{key}' does not exist. Adding it dynamically.");
            AddState(key, valueChange);
        }
    }

    public void SetState(string key, int value)
    {
        states[key] = value;
        OnStateChanged?.Invoke(key, value);
    }

    public Dictionary<string, int> GetStates()
    {
        return states;
    }
}

