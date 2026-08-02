using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(CatProjectionSystem))]
[BurstCompile]
public partial struct StackingMatrixSystem : ISystem {

    // if the board is always circular then approximately 22% of the matrix is wasted space
    // who up designing a circular array

    public const int width = 320;

    public const int height = 320;

    public const int maxIndex = width * height - 1;

    const float stackingHeight = 0.04f;

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BoardTransform>();
        state.RequireForUpdate<StackingMatrixData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        BoardTransform board = SystemAPI.GetSingleton<BoardTransform>();
        var matrix = SystemAPI.GetSingletonBuffer<StackingMatrixData>().Reinterpret<byte>();

        float rDiameter = 1f / (board.radius * 2f);
        float widthMult = rDiameter * width;
        float heightMult = rDiameter * height;

        foreach (var (catData, localTransform) in SystemAPI.Query<RefRW<CatData>, RefRW<LocalTransform>>().WithDisabled<IsInitialFalling>()) {

            float x = (catData.ValueRO.position.x + board.radius) * widthMult;
            float y = (catData.ValueRO.position.y + board.radius) * heightMult;
            int index = (int)(y * width + x);

            if (index < 0 || index > maxIndex) {
                continue;
            }

            int prevStackIndex = catData.ValueRO.prevStackIndex;

            bool runMinus = prevStackIndex > 0 && prevStackIndex <= maxIndex;

            if (index != prevStackIndex) {
                if (prevStackIndex >= 0 && prevStackIndex <= maxIndex && matrix[prevStackIndex] > 0) {
                    matrix[prevStackIndex] -= 1;
                }
                catData.ValueRW.prevStackIndex = index;
                if (matrix[index] < byte.MaxValue) {
                    matrix[index] += 1;
                }
            }

            localTransform.ValueRW.Position.y += (matrix[index] - 1) * stackingHeight;

        }
    }
}
