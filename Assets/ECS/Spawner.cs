using Unity.Entities;
using UnityEngine;

// ReSharper disable once InconsistentNaming
public struct Spawner : IComponentData
{
    public int CountX;
    public int CountY;
    public Entity Prefab;

    //public int[] Counts;
    public int TotalHealthy;
    public int TotalInfected;
    public int TotalRecovered;
    public int TotalDeceased;

    public float InitialInfectedRatio;

    public Vector2 Area;
}
