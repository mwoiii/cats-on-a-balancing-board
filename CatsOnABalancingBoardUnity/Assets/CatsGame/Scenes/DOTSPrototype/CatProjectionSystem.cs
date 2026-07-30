using Unity.Burst;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct CatProjectionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BoardTransform>();
        state.RequireForUpdate<CatSpawnerConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BoardTransform board = SystemAPI.GetSingleton<BoardTransform>();

        CatSpawnerConfig config = SystemAPI.GetSingleton<CatSpawnerConfig>();

        foreach (var (catData, localTransform) in SystemAPI.Query<RefRO<CatData>, RefRW<LocalTransform>>())
        {
            float3 localOffset = new(catData.ValueRO.Position.x, config.DropHeight,catData.ValueRO.Position.y);
            
            float3 worldPos = board.Position + math.mul(board.Rotation, localOffset);

            localTransform.ValueRW.Position = worldPos;
        }
    }
}
