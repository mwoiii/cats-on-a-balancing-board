using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(CatFallMovementSystem))]
[BurstCompile]
public partial struct CatFallCleanupSystem : ISystem {
    const float explodeHeight = -1.25f; // public partial structs cant do unity serialised stuff i think

    const float bounceMult = 1;

    const float delay = 1;

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        float deltaTime = SystemAPI.Time.DeltaTime;
        EntityCommandBuffer ecb = new(Allocator.Temp);
        Unity.Mathematics.Random randomSauce = new(676767);

        foreach (var (fallingData, localTransform, entity) in SystemAPI.Query<RefRW<FallingCatData>, RefRO<LocalTransform>>().WithNone<FallenCatData>().WithEntityAccess()) {
            if (localTransform.ValueRO.Position.y < explodeHeight) {
                float impactSpeed = math.length(fallingData.ValueRO.velocity);
                float3 norm = new(0, 1, 0);
                fallingData.ValueRW.velocity = randomSauce.NextFloat(0.9f, 1.1f) * bounceMult * impactSpeed * norm;
                fallingData.ValueRW.velocity += randomSauce.NextFloat(0.1f, 0.3f) * bounceMult * localTransform.ValueRO.Position; // Sorry once again

                ecb.AddComponent(entity, new FallenCatData { timeToExplode = delay });
            }
        }

        foreach (var fallenData in SystemAPI.Query<RefRW<FallenCatData>>()) {
            fallenData.ValueRW.timeToExplode -= deltaTime;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

[UpdateAfter(typeof(CatFallCleanupSystem))]
public partial struct CatExplosionSystem : ISystem {
    public static event Action CatLost;

    public void OnUpdate(ref SystemState state) {
        EntityCommandBuffer ecb = new(Allocator.Temp);

        foreach (var (fallenData, localTransform, entity) in SystemAPI.Query<RefRO<FallenCatData>, RefRO<LocalTransform>>().WithEntityAccess()) {
            if (fallenData.ValueRO.timeToExplode <= 0) {
                if (ExplosionEffect.instance != null) {
                    float3 pos = localTransform.ValueRO.Position;
                    ExplosionEffect.instance.PlayAt(new UnityEngine.Vector3(pos.x, pos.y, pos.z));
                }
                ecb.DestroyEntity(entity);
                CatLost?.Invoke();

                if (HUDController.instance != null) {
                    HUDController.instance.UpdateRemainingCats(-1);

                    if (HUDController.instance.catCount <= 0 && GameLogicScript.gameRunning && GameLogicScript.instance != null) { GameLogicScript.instance.GameOver(); }
                }
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

public struct FallenCatData : IComponentData {
    public float timeToExplode;
}
