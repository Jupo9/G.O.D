using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class Worlds
{
    private static readonly Worlds instance = new Worlds();
    private static WorldStates world;

    static Worlds()
    {
        world = new WorldStates();
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
}
