using Unity.Burst;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct CatProjectionSystem : ISystem
{
    const float projHeight = 0.1f;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BoardTransform>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BoardTransform board = SystemAPI.GetSingleton<BoardTransform>();

        foreach (var (catData, localTransform) in SystemAPI.Query<RefRO<CatData>, RefRW<LocalTransform>>().WithDisabled<IsInitialFalling>())
        {
            float3 localOffset = new(catData.ValueRO.Position.x, projHeight ,catData.ValueRO.Position.y);
            
            localTransform.ValueRW.Position = board.Position + math.mul(board.Rotation, localOffset);
        }

        if (SystemAPI.HasSingleton<InitialFallData>())
        {
            float height = SystemAPI.GetSingleton<InitialFallData>().Height;

            foreach (var (catData, localTransform) in SystemAPI.Query<RefRO<CatData>, RefRW<LocalTransform>>().WithAll<IsInitialFalling>())
            {       
                float3 landedLocalOffset = new(catData.ValueRO.Position.x, projHeight, catData.ValueRO.Position.y);
                float3 landedWorldPos = board.Position + math.mul(board.Rotation, landedLocalOffset);
                float3 worldPos = landedWorldPos;
                
                worldPos.y += height - projHeight;
                localTransform.ValueRW.Position = worldPos;
            }
        }
    }
}
