using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// for cats fallen off the board
[UpdateBefore(typeof(CatFallCleanupSystem))]
[BurstCompile]
public partial struct CatFallMovementSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState system) {
        float deltaTime = SystemAPI.Time.DeltaTime;

        float3 gravity = new(0, -9.81f, 0);

        foreach (var (fallingData, localTransform) in SystemAPI.Query<RefRW<FallingCatData>, RefRW<LocalTransform>>()) {
            fallingData.ValueRW.velocity += gravity * deltaTime;
            localTransform.ValueRW.Position += fallingData.ValueRW.velocity * deltaTime;
        }
    }
}

public struct FallingCatData : IComponentData, IEnableableComponent {
    public float3 velocity;
}
