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

            foreach (var (effect, entity) in SystemAPI.Query<RefRW<EffectData>>().WithEntityAccess()) {
                effect.ValueRW.lifetime -= deltaTime;
                if (effect.ValueRW.lifetime <= 0) {
                    ecb.DestroyEntity(entity);
                    switch (effect.ValueRW.type) {
                        case EffectType.Explosion:
                            config.currentExplosionCount--;
                            break;
                        case EffectType.Supernova:
                            config.currentSupernovaCount--;
                            break;
                    }
                    config.currentExplosionCount -= 1;
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    public struct EffectData : IComponentData {
        public float lifetime;

        public EffectType type;
    }
}
