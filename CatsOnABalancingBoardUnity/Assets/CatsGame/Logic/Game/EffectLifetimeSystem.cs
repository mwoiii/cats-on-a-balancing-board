using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Assets.CatsGame.Logic.Game {

    [BurstCompile]
    public partial struct EffectLifetimeSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EffectSpawnerConfig>();
        }

        public void OnUpdate(ref SystemState state) {
            EffectSpawnerConfig config = SystemAPI.GetSingleton<EffectSpawnerConfig>();
            float deltaTime = SystemAPI.Time.DeltaTime;

            EntityCommandBuffer ecb = new(Allocator.Temp);

            foreach (var (explosion, entity) in SystemAPI.Query<RefRW<EffectData>>().WithEntityAccess()) {
                explosion.ValueRW.lifetime -= deltaTime;
                if (explosion.ValueRW.lifetime <= 0) {
                    ecb.DestroyEntity(entity);
                    config.currentExplosionCount -= 1;
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    public struct EffectData : IComponentData {
        public float lifetime;
    }
}
