using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace OMC.ECS {
    [UpdateBefore(typeof(CatMovementSystem))]
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

            foreach (var (fallenData, localTransform, mmInfo, entity) in SystemAPI.Query<RefRW<FallenCatData>, RefRO<LocalTransform>, RefRO<MaterialMeshInfo>>().WithEntityAccess()) {
                fallenData.ValueRW.timeToExplode -= deltaTime;
                if (fallenData.ValueRO.timeToExplode <= 0) {
                    float3 pos = localTransform.ValueRO.Position;
                    if (lostCount < positionBuffer.Length) {
                        positionBuffer[lostCount] = pos;
                    }
                    lostCount++;
                    ecb.SetComponentEnabled<MaterialMeshInfo>(entity, false);
                }
            }

            catCount.ValueRW.lost += lostCount;

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
