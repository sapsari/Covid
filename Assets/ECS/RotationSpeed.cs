using System;
using Unity.Entities;

// ReSharper disable once InconsistentNaming
[Serializable]
public struct RotationSpeed : IComponentData
{
    public float RadiansPerSecond;
}
