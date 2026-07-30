using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[UpdateBefore(typeof(CatProjectionSystem))]
[BurstCompile]
public partial struct CatMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BoardTransform>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BoardTransform board = SystemAPI.GetSingleton<BoardTransform>();
        float deltaTime = SystemAPI.Time.DeltaTime;

        float3 gravityWorld = new(0,-9.81f,0);

        float3 gravityLocal = math.mul(math.inverse(board.Rotation), gravityWorld);
        float2 down = new(gravityLocal.x,gravityLocal.z);

        foreach (var (catData, catVelocity) in SystemAPI.Query<RefRW<CatData>, RefRW<CatVelocity>>())
        {
            catVelocity.ValueRW.Value += down * deltaTime;
            catData.ValueRW.Position += catVelocity.ValueRW.Value * deltaTime;
        }
    }
}
