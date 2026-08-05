using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace OMC.ECS {
    [UpdateAfter(typeof(CatFallCleanupSystem))]
    [UpdateBefore(typeof(CatCountBridgingSystem))]
    public partial struct CatExplosionSystem : ISystem {

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EffectSpawnerConfig>();
            state.RequireForUpdate<CatCount>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EffectSpawnerConfig config = SystemAPI.GetSingleton<EffectSpawnerConfig>();
            EntityCommandBuffer ecb = new(Allocator.Temp);
            var catCount = SystemAPI.GetSingletonRW<CatCount>();
            var positionBuffer = SystemAPI.GetSingletonBuffer<LostCatPosition>().Reinterpret<float3>().AsNativeArray();
            float deltaTime = SystemAPI.Time.DeltaTime;
            int lostCount = 0;

            foreach (var (fallenData, localTransform, mmInfo, entity) in SystemAPI.Query<RefRW<FallenCatData>, RefRO<LocalTransform>, RefRO<MaterialMeshInfo>>().WithEntityAccess()) {
                fallenData.ValueRW.timeToExplode -= deltaTime;
                if (fallenData.ValueRO.timeToExplode <= 0) {
                    float3 pos = localTransform.ValueRO.Position;
                    if (lostCount < positionBuffer.Length) {
                        positionBuffer[lostCount] = pos;
                    }
                    lostCount++;
                    if (config.currentExplosionCount < config.maxExplosionCount) {
                        Entity explosion = ecb.Instantiate(config.explosionPrefab);
                        ecb.SetComponent(explosion, new LocalTransform { Position = pos, Scale = 0.2f });
                        config.currentExplosionCount++;
                    }
                    ecb.SetComponentEnabled<MaterialMeshInfo>(entity, false);
                }
            }

            catCount.ValueRW.lost += lostCount;

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
