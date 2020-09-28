using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public class AgentSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     float deltaTime = Time.DeltaTime;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.
        
        
        /*
        Entities.ForEach((ref Translation translation, ref Agent agent) => {
            // Implement the work to perform for each entity here.
            // You should only access data that is local or that is a
            // field on this job. Note that the 'rotation' parameter is
            // marked as 'in', which means it cannot be modified,
            // but allows this job to run in parallel with other jobs
            // that want to read Rotation component data.
            // For example,
            //     translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;



        }).Schedule();
        */


        //var changeColorJobHandle =
        Entities.ForEach((ref Agent agent, ref URPMaterialPropertyBaseColor color) =>
        {
            if (agent.State == AgentState.Infected)
            {
                // ColorCombo375 with Hex Colors #F1433F #F7E967 #A9CF54 #70B7BA #3D4C53
                // #F1433F
                color.Value.x = 241f / 255;
                color.Value.y = 67f / 255;
                color.Value.z = 63f / 255;
                color.Value.w = 0f;
            }
        }).ScheduleParallel();
    }
}
