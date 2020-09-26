using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

public struct LifeTime : IComponentData
{
    public float Value;
}

// This system updates all entities in the scene with both a RotationSpeed and Rotation component.
public class LifeTimeSystem : SystemBase
{
    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    static int GetPositionHash(float3 position, int cellSize)
    {
        //var hash = (int)math.hash(new int3(math.floor(position / cellSize)));

        var x = (int)math.floor(position.x);
        var z = (int)math.floor(position.z);

        const int offset = 10000;

        var hash = x * offset + z;

        return hash;
    }

    // OnUpdate runs on the main thread.
    protected override void OnUpdate()
    {
        var commandBuffer = m_Barrier.CreateCommandBuffer().AsParallelWriter();

        var deltaTime = Time.DeltaTime;

        
        var ASDASD = 100 * 100;
        var cellSize = 2; // 2 meters
        var hashMap = new NativeMultiHashMap<int, int>(ASDASD, Allocator.TempJob);

        var parallelHashMap = hashMap.AsParallelWriter();
        var hashPositionsJobHandle = Entities
            .WithName("HashPositionsJob")
            .WithAll<Agent>()
            .ForEach((int entityInQueryIndex, in LocalToWorld localToWorld) =>
            {
                //var hash = (int)math.hash(new int3(math.floor(localToWorld.Position / cellSize)));
                var hash = GetPositionHash(localToWorld.Position, cellSize);
                parallelHashMap.Add(hash, entityInQueryIndex);
            })
            .ScheduleParallel(Dependency);


        var calculateInfected = Entities
            .WithName("CalculateInfectedJob")
            .WithAll<Agent>()
            .ForEach((int entityInQueryIndex, in LocalToWorld localToWorld) =>
            {

            })
            .ScheduleParallel(hashPositionsJobHandle);

        /*
        Entities.ForEach((Entity entity, int nativeThreadIndex, ref LifeTime lifetime) =>
        {
            lifetime.Value -= deltaTime;

            if (lifetime.Value < 0.0f)
            {
                commandBuffer.DestroyEntity(nativeThreadIndex, entity);
            }
        }).ScheduleParallel();
        */

        /*
        Entities.ForEach((Entity entity, int nativeThreadIndex, ref LifeTime lifetime, ref URPMaterialPropertyBaseColor color) =>
        {
            lifetime.Value -= deltaTime;

            if (lifetime.Value < 0.0f)
            {
                //commandBuffer.DestroyEntity(nativeThreadIndex, entity);

                // ColorCombo375 with Hex Colors #F1433F #F7E967 #A9CF54 #70B7BA #3D4C53
                // #F1433F
                color.Value.x = 241f / 255;
                color.Value.y = 67f / 255;
                color.Value.z = 63f / 255;
                color.Value.w = 0f;
            }
        }).ScheduleParallel();
        */

        /*

        //var changeColorJobHandle =
            Entities.ForEach((Entity entity, int nativeThreadIndex, ref Agent agent, ref URPMaterialPropertyBaseColor color) =>
        {
            //lifetime.Value -= deltaTime;

            if (agent.State == AgentState.Infected)
            {
                //commandBuffer.DestroyEntity(nativeThreadIndex, entity);

                // ColorCombo375 with Hex Colors #F1433F #F7E967 #A9CF54 #70B7BA #3D4C53
                // #F1433F
                color.Value.x = 241f / 255;
                color.Value.y = 67f / 255;
                color.Value.z = 63f / 255;
                color.Value.w = 0f;
            }
        }).ScheduleParallel();

        */

        Dependency = hashPositionsJobHandle;
        var disposeJobHandle = hashMap.Dispose(Dependency);
        Dependency = disposeJobHandle;




        m_Barrier.AddJobHandleForProducer(Dependency);
    }
}
