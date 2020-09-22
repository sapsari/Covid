using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

// This system updates all entities in the scene with both a RotationSpeed_SpawnAndRemove and Rotation component.

// ReSharper disable once InconsistentNaming
public class RotationSpeedSystem : SystemBase
{
    // OnUpdate runs on the main thread.
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        // The in keyword on the RotationSpeed component tells the job scheduler that this job will not write to rotSpeedSpawnAndRemove
        Entities
            .WithName("RotationSpeedSystem")
            .ForEach((ref Rotation rotation, in RotationSpeed rotSpeed) =>
            {
                // Rotate something about its up vector at the speed given by RotationSpeed_SpawnAndRemove.
                rotation.Value = math.mul(math.normalize(rotation.Value), quaternion.AxisAngle(math.up(), rotSpeed.RadiansPerSecond * deltaTime));
            }).ScheduleParallel();
    }
}
