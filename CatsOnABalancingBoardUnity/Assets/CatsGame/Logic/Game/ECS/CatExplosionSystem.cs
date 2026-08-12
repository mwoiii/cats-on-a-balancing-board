using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace OMC.ECS {
    [UpdateAfter(typeof(CatFallCleanupSystem))]
    [UpdateBefore(typeof(CatMovementSystem))]
    [UpdateBefore(typeof(CatCountBridgingSystem))]
    public partial struct CatExplosionSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<CatCount>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new(Allocator.Temp);
            var catCount = SystemAPI.GetSingletonRW<CatCount>();
            var positionBuffer = SystemAPI.GetSingletonBuffer<LostCatPosition>().Reinterpret<float3>().AsNativeArray();
            float deltaTime = SystemAPI.Time.DeltaTime;
            int lostCount = 0;
            int lostValue = 0;

            foreach (var (fallenData, localTransform, catValue, entity) in SystemAPI.Query<RefRW<FallenCatData>, RefRO<LocalTransform>, RefRO<CatValue>>().WithEntityAccess()) {
                fallenData.ValueRW.timeToExplode -= deltaTime;
                if (fallenData.ValueRO.timeToExplode <= 0) {
                    float3 pos = localTransform.ValueRO.Position;
                    if (lostCount < positionBuffer.Length) {
                        positionBuffer[lostCount] = pos;
                    }
                    lostCount++;
                    lostValue += catValue.ValueRO.value;
                    ecb.SetComponentEnabled<MaterialMeshInfo>(entity, false);
                    ecb.SetComponentEnabled<FallenCatData>(entity, false);
                    ecb.SetComponentEnabled<FallingCatData>(entity, false);
                }
            }

            catCount.ValueRW.lost += lostValue;
            catCount.ValueRW.lostRaw += lostCount;

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
