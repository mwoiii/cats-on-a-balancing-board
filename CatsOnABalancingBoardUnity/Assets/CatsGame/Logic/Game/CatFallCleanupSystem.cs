using Assets.CatsGame.Logic.Game;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(CatFallMovementSystem))]
[BurstCompile]
public partial struct CatFallCleanupSystem : ISystem {
    const float explodeHeight = -1.25f; // public partial structs cant do unity serialised stuff i think

    const float bounceMult = 1;

    const float delay = 1;

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        EntityCommandBuffer ecb = new(Allocator.Temp);
        Unity.Mathematics.Random randomSauce = new(676767);

        foreach (var (fallingData, localTransform, entity) in SystemAPI.Query<RefRW<FallingCatData>, RefRO<LocalTransform>>().WithDisabled<FallenCatData>().WithEntityAccess()) {
            if (localTransform.ValueRO.Position.y < explodeHeight) {
                float impactSpeed = math.length(fallingData.ValueRO.velocity);
                float3 norm = new(0, 1, 0);
                fallingData.ValueRW.velocity = randomSauce.NextFloat(0.9f, 1.1f) * bounceMult * impactSpeed * norm;
                fallingData.ValueRW.velocity += randomSauce.NextFloat(0.1f, 0.3f) * bounceMult * localTransform.ValueRO.Position; // Sorry once again

                ecb.SetComponent(entity, new FallenCatData { timeToExplode = delay });
                ecb.SetComponentEnabled<FallenCatData>(entity, true);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

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
                ecb.DestroyEntity(entity);
            }
        }

        catCount.ValueRW.lost += lostCount;

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}



public struct FallenCatData : IComponentData, IEnableableComponent {
    public float timeToExplode;
}
