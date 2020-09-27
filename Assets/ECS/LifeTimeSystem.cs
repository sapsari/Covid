using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine.XR.WSA;
using System.Linq;
using System.Diagnostics;

public struct LifeTime : IComponentData
{
    public float Value;
}

public struct AgentFactor
{
    public int Index;
    public float3 Position;
    public float Factor;
    public AgentState State;
}

public static class Constants
{
    public const float CellSize = 2f;
    public const float TickTime = 1f;
}

public static class JobLogger
{
    [BurstDiscard] public static void Log<T>(T segment) => UnityEngine.Debug.Log(AppendToString(segment));

    [BurstDiscard]
    public static string AppendToString(params object[] parts)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Clear();
        for (int i = 0, len = parts.Length; i < len; i++) sb.Append(parts[i].ToString());
        return sb.ToString();
    }
}


// This system updates all entities in the scene with both a RotationSpeed and Rotation component.
public class LifeTimeSystem : SystemBase
{
    EntityCommandBufferSystem m_Barrier;
    float deltaTime;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    static int GetPositionHash(float3 position)
    {
        //var hash = (int)math.hash(new int3(math.floor(position / cellSize)));

        var x = (int)math.floor(position.x / Constants.CellSize);
        var z = (int)math.floor(position.z / Constants.CellSize);

        const int offset = 10000;

        var hash = x * offset + z;

        return hash;
    }


    // OnUpdate runs on the main thread.
    protected override void OnUpdate()
    {
        var commandBuffer = m_Barrier.CreateCommandBuffer().AsParallelWriter();

        //var deltaTime = Time.DeltaTime;
        this.deltaTime += Time.DeltaTime;

        //Debug.WriteLine($"dt:{deltaTime}");
        //JobLogger.Log($"dt:{deltaTime}");

        if (deltaTime > Constants.TickTime)
            deltaTime -= Constants.TickTime;
        else
            return;

        //NativeArray<Entity> entities = EntityManager.GetAllEntities(Allocator.Persistent);
        


        const int ASDASD = 100 * 100;
        const int cellSize = 2; // 2 meters
        var hashMap = new NativeMultiHashMap<int, AgentFactor>(ASDASD, Allocator.TempJob);

        var parallelHashMap = hashMap.AsParallelWriter();
        var hashPositionsJobHandle = Entities
            .WithName("HashPositionsJob")
            .WithAll<Agent>()
            .ForEach((int entityInQueryIndex, in LocalToWorld localToWorld, in Agent agent) =>
            {
                var position = localToWorld.Position;
                //var hash = (int)math.hash(new int3(math.floor(localToWorld.Position / cellSize)));
                var hash = GetPositionHash(localToWorld.Position);
                var riskFactor = agent.State == AgentState.Infected ? 0.01f : 0f;
                var agentFactor = new AgentFactor
                {
                    Index = entityInQueryIndex,
                    Position = position,
                    Factor = riskFactor,
                    State = agent.State,
                };
                parallelHashMap.Add(hash, agentFactor);
            }).ScheduleParallel(Dependency);
            //.ScheduleParallel(JobHandle.CombineDependencies(Dependency,
              //  World.GetOrCreateSystem<StepPhysicsWorld>().FinalJobHandle);



        hashPositionsJobHandle.Complete();

        //var parallelHashMap2 = hashMap.AsParallel();
        //hashMap.as

        var increaseInfectionJobHandle = Entities
            .WithName("IncreaseInfectionJob")
            .WithAll<Agent>()
            .ForEach((int entityInQueryIndex, ref LocalToWorld localToWorld, ref Agent agent) =>
            {
                if (agent.State == AgentState.Healthy)
                {

                    var random = new Random(1);

                    var position = localToWorld.Position;
                    //var hash = (int)math.hash(new int3(math.floor(localToWorld.Position / cellSize)));
                    var hash = GetPositionHash(localToWorld.Position);

                    var neighbours = hashMap.GetValuesForKey(hash);


                    //using (var enumerator = neighbours.GetEnumerator())
                    var enumerator = neighbours.GetEnumerator();
                    {
                        while (enumerator.MoveNext())
                        {
                            var neighbour = enumerator.Current;

                            if (neighbour.State != AgentState.Infected)
                                continue;

                            if (math.distancesq(neighbour.Position, position) < 2 * 2)
                            {
                                var transmissionRisk = agent.IsWearingMask ? .05f : 0.02f;

                                agent.RiskFactor += transmissionRisk;
                                //neighbour.Factor;
                                //agent.f
                            }
                        }
                    }


                    if (agent.RiskFactor > random.NextFloat())
                    {
                        agent.State = AgentState.Infected;
                    }

                    /*
                    foreach (var neighbour in neighbours)
                    {
                        if (math.distancesq(neighbour.Position, position) < 2 * 2)
                        {
                            //neighbour.Factor;
                        }
                    }*/

                }
            }).WithReadOnly(hashMap).ScheduleParallel(Dependency);




        /*
        var keys = hashMap.GetKeyArray(Allocator.TempJob);

        foreach (var key in keys)
        {
            var values = hashMap.GetValuesForKey(key);
            foreach (var value in values)
            {
                //value.
                //entities[value].
            }

        }

        keys.Dispose();*/

        /*
        var calculateInfected = Entities
            .WithName("CalculateInfectedJob")
            .WithAll<Agent>()
            .ForEach((int entityInQueryIndex, in LocalToWorld localToWorld) =>
            {

            })
            .ScheduleParallel(hashPositionsJobHandle);
        */

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

        Dependency = increaseInfectionJobHandle;
        var disposeJobHandle = hashMap.Dispose(Dependency);
        Dependency = disposeJobHandle;




        m_Barrier.AddJobHandleForProducer(Dependency);
    }
}
