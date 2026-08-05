using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace OMC.ECS {
    [BurstCompile]
    public partial struct CatProjectionSystem : ISystem {
        const float projHeight = 0.1f;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BoardTransform>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            BoardTransform board = SystemAPI.GetSingleton<BoardTransform>();

            state.Dependency = new ProjectionJob { board = board }.ScheduleParallel(state.Dependency);

            if (SystemAPI.HasSingleton<InitialFallData>()) {
                float height = SystemAPI.GetSingleton<InitialFallData>().height;

                state.Dependency = new InitialFallingJob {
                    board = board,
                    height = height
                }.ScheduleParallel(state.Dependency);
            }
        }

        [WithDisabled(typeof(IsInitialFalling))]
        [BurstCompile]
        partial struct ProjectionJob : IJobEntity {
            public BoardTransform board;

            void Execute(in CatData catData, ref LocalTransform localTransform) {
                float3 localOffset = new(catData.position.x, projHeight, catData.position.y);

                localTransform.Position = board.position + math.mul(board.rotation, localOffset);
            }
        }

        [WithAll(typeof(IsInitialFalling))]
        [BurstCompile]
        partial struct InitialFallingJob : IJobEntity {
            public BoardTransform board;
            public float height;

            void Execute(in CatData catData, ref LocalTransform localTransform) {
                float3 landedLocalOffset = new(catData.position.x, projHeight, catData.position.y);
                float3 landedWorldPos = board.position + math.mul(board.rotation, landedLocalOffset);
                float3 worldPos = landedWorldPos;

                worldPos.y += height - projHeight;
                localTransform.Position = worldPos;
            }
        }
    }
}
