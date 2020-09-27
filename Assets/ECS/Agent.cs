﻿using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public enum AgentState { Healthy, Infected, Recovered, Deceased, Count }

[Serializable]
public struct Agent : IComponentData
{
    public AgentState State;

    public bool IsWearingMask;
    public float RiskFactor;

    public float DeltaTime;



    // Add fields to your component here. Remember that:
    //
    // * A component itself is for storing data and doesn't 'do' anything.
    //
    // * To act on the data, you will need a System.
    //
    // * Data in a component must be blittable, which means a component can
    //   only contain fields which are primitive types or other blittable
    //   structs; they cannot contain references to classes.
    //
    // * You should focus on the data structure that makes the most sense
    //   for runtime use here. Authoring Components will be used for 
    //   authoring the data in the Editor.
    
    
}
