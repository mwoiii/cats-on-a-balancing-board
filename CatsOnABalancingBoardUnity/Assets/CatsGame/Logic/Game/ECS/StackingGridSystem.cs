using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace OMC.ECS {
    [UpdateAfter(typeof(CatProjectionSystem))]
    [BurstCompile]
    public partial struct StackingGridSystem : ISystem {

        // if the board is always circular then approximately 22% of the array is wasted space
        // who up designing a circular array

        public const int width = 400;

        public const int height = 400;

        public const int maxIndex = width * height - 1;

        public const float stackingHeight = 0.04f;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BoardTransform>();
            state.RequireForUpdate<StackingGridData>();
            state.RequireForUpdate<RefreshGridData>();
            state.RequireForUpdate<RefreshGridValue>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            float deltaTime = SystemAPI.Time.DeltaTime;

            if (deltaTime <= 0) {
                return;
            }

            BoardTransform board = SystemAPI.GetSingleton<BoardTransform>();
            var stackingGrid = SystemAPI.GetSingletonBuffer<StackingGridData>().Reinterpret<ushort>().AsNativeArray();
            var refreshGrid = SystemAPI.GetSingletonBuffer<RefreshGridData>().Reinterpret<byte>().AsNativeArray();
            var refreshGridValue = SystemAPI.GetSingletonRW<RefreshGridValue>();
            refreshGridValue.ValueRW.value += 1;

            float rDiameter = 1f / (board.radius * 2f);
            float widthMult = rDiameter * width;
            float heightMult = rDiameter * height;

            StackingGridJob job = new StackingGridJob() {
                widthMult = widthMult,
                heightMult = heightMult,
                stackingGrid = stackingGrid,
                refreshGrid = refreshGrid,
                refreshValue = refreshGridValue.ValueRO.value,
                board = board,
                deltaTime = deltaTime
            };

            // WE BALL!!!!!!
            job.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct StackingGridJob : IJobEntity {
        public float widthMult;

        public float heightMult;

        [NativeDisableParallelForRestriction] // WE BALL!!!!!!!!!!!!!
        public NativeArray<ushort> stackingGrid;

        [NativeDisableParallelForRestriction] // WE BALL!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public NativeArray<byte> refreshGrid;

        public byte refreshValue;

        public BoardTransform board;

        public float deltaTime;

        const float smoothingRate = 5f;

        [BurstCompile]
        void Execute(ref CatStack catStack, ref LocalTransform localTransform) {
            float x = (localTransform.Position.x + board.radius) * widthMult;
            float y = (localTransform.Position.z + board.radius) * heightMult;
            int index = (int)(y * StackingGridSystem.width + x);

            if (index < 0 || index > StackingGridSystem.maxIndex) {
                return;
            }

            int prevStackIndex = catStack.prevStackIndex;

            // if entered a new cell and the cell hasn't already been updated
            if (index != prevStackIndex && prevStackIndex >= 0 && prevStackIndex <= StackingGridSystem.maxIndex && refreshGrid[prevStackIndex] != refreshValue) {
                refreshGrid[prevStackIndex] = refreshValue;
                stackingGrid[prevStackIndex] = 0;
            }

            catStack.prevStackIndex = index;

            // if the current cell hasn't been refreshed
            if (refreshGrid[index] != refreshValue) {
                refreshGrid[index] = refreshValue;
                stackingGrid[index] = 0;
            }

            // increment the current cell
            if (stackingGrid[index] < ushort.MaxValue) {
                stackingGrid[index] += 1;
            }

            float targetOffset = (stackingGrid[index] - 1) * StackingGridSystem.stackingHeight;
            float t = 1 - math.exp(-smoothingRate * deltaTime);
            catStack.smoothStackOffset = math.lerp(catStack.smoothStackOffset, targetOffset, t);

            localTransform.Position.y += catStack.smoothStackOffset;
        }
    }
}
