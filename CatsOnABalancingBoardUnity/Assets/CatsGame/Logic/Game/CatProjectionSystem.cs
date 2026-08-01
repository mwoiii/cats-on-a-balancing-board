using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct CatProjectionSystem : ISystem {
    const float projHeight = 0.1f;

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BoardTransform>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        BoardTransform board = SystemAPI.GetSingleton<BoardTransform>();

        foreach (var (catData, localTransform) in SystemAPI.Query<RefRO<CatData>, RefRW<LocalTransform>>().WithDisabled<IsInitialFalling>()) {
            float3 localOffset = new(catData.ValueRO.position.x, projHeight, catData.ValueRO.position.y);

            localTransform.ValueRW.Position = board.position + math.mul(board.rotation, localOffset);
        }

        if (SystemAPI.HasSingleton<InitialFallData>()) {
            float height = SystemAPI.GetSingleton<InitialFallData>().height;

            foreach (var (catData, localTransform) in SystemAPI.Query<RefRO<CatData>, RefRW<LocalTransform>>().WithAll<IsInitialFalling>()) {
                float3 landedLocalOffset = new(catData.ValueRO.position.x, projHeight, catData.ValueRO.position.y);
                float3 landedWorldPos = board.position + math.mul(board.rotation, landedLocalOffset);
                float3 worldPos = landedWorldPos;

                worldPos.y += height - projHeight;
                localTransform.ValueRW.Position = worldPos;
            }
        }
    }
}
