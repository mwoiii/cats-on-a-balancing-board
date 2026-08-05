using Unity.Burst;
using Unity.Entities;

namespace OMC.ECS {
    [UpdateAfter(typeof(CatSpawnSystem))]
    [UpdateBefore(typeof(CatProjectionSystem))]
    [BurstCompile]
    public partial struct InitialFallSystem : ISystem {
        const float landHeight = 0.1f;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InitialFallData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            InitialFallData fallState = SystemAPI.GetSingleton<InitialFallData>();
            if (fallState.height <= landHeight) { return; }

            float deltaTime = SystemAPI.Time.DeltaTime;
            fallState.velocity += -9.81f * deltaTime;
            fallState.height += fallState.velocity * deltaTime;

            if (fallState.height <= landHeight) {
                foreach (var falling in SystemAPI.Query<EnabledRefRW<IsInitialFalling>>()) {
                    falling.ValueRW = false;
                }
            }

            SystemAPI.SetSingleton(fallState);
        }
    }

    public struct InitialFallData : IComponentData {
        public float height;
        public float velocity;
    }
}
