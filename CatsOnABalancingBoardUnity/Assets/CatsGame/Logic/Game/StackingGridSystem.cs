using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(CatProjectionSystem))]
[BurstCompile]
public partial struct StackingGridSystem : ISystem {

    // if the board is always circular then approximately 22% of the array is wasted space
    // who up designing a circular array

    public const int width = 320;

    public const int height = 320;

    public const int maxIndex = width * height - 1;

    public const float stackingHeight = 0.04f;

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BoardTransform>();
        state.RequireForUpdate<StackingGridData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        BoardTransform board = SystemAPI.GetSingleton<BoardTransform>();
        var grid = SystemAPI.GetSingletonBuffer<StackingGridData>().Reinterpret<ushort>().AsNativeArray();

        float rDiameter = 1f / (board.radius * 2f);
        float widthMult = rDiameter * width;
        float heightMult = rDiameter * height;

        StackingGridJob job = new StackingGridJob() {
            widthMult = widthMult,
            heightMult = heightMult,
            grid = grid,
            board = board,
        };

        // job.run if cat count is below some threshold
        // or maybe always. it depends how we optimize things
        job.Schedule();
    }
}

[BurstCompile]
public partial struct StackingGridJob : IJobEntity {
    public float widthMult;

    public float heightMult;

    public NativeArray<ushort> grid;

    public BoardTransform board;

    [BurstCompile]
    void Execute(ref CatData catData, ref LocalTransform localTransform) {
        float x = (catData.position.x + board.radius) * widthMult;
        float y = (catData.position.y + board.radius) * heightMult;
        int index = (int)(y * StackingGridSystem.width + x);

        if (index < 0 || index > StackingGridSystem.maxIndex) {
            return;
        }

        int prevStackIndex = catData.prevStackIndex;

        if (index != prevStackIndex) {
            if (prevStackIndex >= 0 && prevStackIndex <= StackingGridSystem.maxIndex && grid[prevStackIndex] > 0) {
                grid[prevStackIndex] -= 1;
            }
            catData.prevStackIndex = index;
            if (grid[index] < ushort.MaxValue) {
                grid[index] += 1;
            }
        }

        localTransform.Position.y += (grid[index] - 1) * StackingGridSystem.stackingHeight;
    }
}
