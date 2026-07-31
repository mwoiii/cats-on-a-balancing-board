using Unity.Entities;
using UnityEngine;
using Unity.Burst;

[UpdateAfter(typeof(CatSpawnSystem))]
[UpdateBefore(typeof(CatProjectionSystem))]
[BurstCompile]
public partial struct InitialFallSystem : ISystem
{
    const float landHeight = 0.1f;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        float gravity = -9.81f;

        EntityCommandBuffer ecb = new(Unity.Collections.Allocator.Temp);

        foreach (var (initialFallData, entity) in SystemAPI.Query<RefRW<InitialFallData>>().WithEntityAccess())
        {
            initialFallData.ValueRW.Velocity += gravity * deltaTime;
            initialFallData.ValueRW.Height += initialFallData.ValueRW.Velocity * deltaTime;

            if (initialFallData.ValueRO.Height <= landHeight)
            {
                ecb.RemoveComponent<InitialFallData>(entity);
            }
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

public struct InitialFallData : IComponentData
{
    public float Height;
    public float Velocity;
}

