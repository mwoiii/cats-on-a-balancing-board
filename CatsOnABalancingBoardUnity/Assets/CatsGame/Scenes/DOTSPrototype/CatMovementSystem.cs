using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct CatMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float delta = SystemAPI.Time.DeltaTime;
        foreach (var (transform, vel) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<CatVelocity>>()) // What?
        {
            transform.ValueRW.Position.x += vel.ValueRO.value.x * delta;
            transform.ValueRW.Position.z += vel.ValueRO.value.y * delta;
        }
    }
}
