using Unity.Entities;
using UnityEngine;
using Unity.Burst;

[UpdateAfter(typeof(CatSpawnSystem))]
[UpdateBefore(typeof(CatProjectionSystem))]
[BurstCompile]
public partial struct InitialFallSystem : ISystem
{
    const float landHeight = 0.1f;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InitialFallData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        InitialFallData fallState = SystemAPI.GetSingleton<InitialFallData>();
        if (fallState.Height <= landHeight){return;}

        float deltaTime = SystemAPI.Time.DeltaTime;
        fallState.Velocity += -9.81f * deltaTime;
        fallState.Height += fallState.Velocity * deltaTime;

        if (fallState.Height <= landHeight)
        {
            foreach (var falling in SystemAPI.Query<EnabledRefRW<IsInitialFalling>>())
            {
                falling.ValueRW = false;
            }
        }

        SystemAPI.SetSingleton(fallState);
    }
}

public struct InitialFallData : IComponentData
{
    public float Height;
    public float Velocity;
}

