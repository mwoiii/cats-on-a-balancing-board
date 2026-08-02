using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(CatProjectionSystem))]
[BurstCompile]
public partial struct CatMovementSystem : ISystem {
    const float friction = 2;

    const float moveForce = 1.2f;

    const float reactDistance = 1.5f;

    const float dispersionPerCat = 0.0002f;

    const float maxDispersion = 1f;

    const float catnipContactRadius = 0.05f;

    EntityQuery catQuery;
    uint frameCounter; // new random every frame

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BoardTransform>();
        state.RequireForUpdate<WeightSnapshot>();
        catQuery = SystemAPI.QueryBuilder().WithAll<CatData>().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        BoardTransform board = SystemAPI.GetSingleton<BoardTransform>();
        DynamicBuffer<WeightSnapshot> weights = SystemAPI.GetSingletonBuffer<WeightSnapshot>();
        DynamicBuffer<WeightContactPulse> pulses = SystemAPI.GetSingletonBuffer<WeightContactPulse>();
        float deltaTime = SystemAPI.Time.DeltaTime;

        float3 gravityWorld = new(0, -9.81f, 0);
        float3 gravityLocal = math.mul(math.inverse(board.rotation), gravityWorld); // Imagine rotating a cube by thirty degrees
        float2 down = new(gravityLocal.x, gravityLocal.z);

        int catCount = catQuery.CalculateEntityCount();
        float dispersionStrength = math.min(catCount * dispersionPerCat, maxDispersion);
        frameCounter++;

        EntityCommandBuffer ecb = new(Allocator.Temp);

        foreach (var (catData, catVelocity, entity) in SystemAPI.Query<RefRW<CatData>, RefRW<CatVelocity>>().WithDisabled<IsInitialFalling>().WithEntityAccess()) {
            Random randomSauce = Random.CreateFromIndex((uint)entity.Index * 67 + frameCounter * 21); // some Bezout shenanigans going on here

            // weight reactive behaviour
            float2 catPos = catData.ValueRO.position;

            float nearestBasicDist = reactDistance + randomSauce.NextFloat(-0.2f, 0.2f);
            float2 nearestBasicPos = float2.zero;
            bool hasBasic = false;

            float nearestCatnipDist = float.MaxValue;
            float2 nearestCatnipPos = float2.zero;
            int nearestCatnipIndex = -1;
            bool hasCatnip = false;

            float nearestLemonDist = reactDistance + randomSauce.NextFloat(-0.2f, 0.2f); ;
            float2 nearestLemonPos = float2.zero;
            bool hasLemon = false;

            for (int i = 0; i < weights.Length; i++) {
                WeightSnapshot w = weights[i];
                float dist = math.distancesq(catPos, w.localPosition); // distance squared saves a square root operation but i maybe should be more precise with variable names

                if (w.type == WeightBehaviour.WeightType.None && w.state == WeightBehaviour.WeightState.Falling && dist < nearestBasicDist) {
                    nearestBasicDist = dist;
                    nearestBasicPos = w.localPosition;
                    hasBasic = true;
                }
                if (w.type == WeightBehaviour.WeightType.Catnip && dist < nearestCatnipDist) {
                    nearestCatnipDist = dist;
                    nearestCatnipPos = w.localPosition;
                    nearestCatnipIndex = i;
                    hasCatnip = true;
                }
                if (w.type == WeightBehaviour.WeightType.Lemon && dist < nearestLemonDist) {
                    nearestLemonDist = dist;
                    nearestLemonPos = w.localPosition;
                    hasLemon = true;
                }
            }

            float2 weightForce = float2.zero;
            if (hasBasic) // can the cat has basic
            {
                float2 toTarget = nearestBasicPos - catPos;
                if (math.lengthsq(toTarget) > 0) { weightForce -= math.normalize(toTarget) * moveForce; }
            }
            if (hasCatnip) {
                float2 toTarget = nearestCatnipPos - catPos;
                if (math.lengthsq(toTarget) > 0) { weightForce += math.normalize(toTarget) * moveForce; }

                if (nearestCatnipDist < catnipContactRadius && weights[nearestCatnipIndex].state == WeightBehaviour.WeightState.Landed) {
                    pulses.ElementAt(nearestCatnipIndex).count++;
                }
            }
            if (hasLemon) {
                float2 toTarget = nearestLemonPos - catPos;
                if (math.lengthsq(toTarget) > 0) { weightForce -= math.normalize(toTarget) * moveForce; }
            }

            // random dispersion
            float2 dispersion = randomSauce.NextFloat2Direction() * dispersionStrength;

            // forces applied (gravity and friction also thrown in here)
            catVelocity.ValueRW.value += (down + weightForce + dispersion) * deltaTime;
            catVelocity.ValueRW.value *= math.max(0, 1 - friction * deltaTime);
            catData.ValueRW.position += catVelocity.ValueRW.value * deltaTime;


            if (math.length(catData.ValueRO.position) > board.radius) // if cat fallen off...
            {
                float3 lastLocalPos = new(catData.ValueRO.position.x, 0.1f, catData.ValueRO.position.y);
                float3 worldPos = board.position + math.mul(board.rotation, lastLocalPos);

                float3 lastLocalVel = new(catVelocity.ValueRO.value.x, 0, catVelocity.ValueRO.value.y);
                float3 worldVel = math.mul(board.rotation, lastLocalVel);

                ecb.RemoveComponent<CatData>(entity);
                ecb.RemoveComponent<CatVelocity>(entity);

                ecb.SetComponent(entity, LocalTransform.FromPosition(worldPos));
                ecb.AddComponent(entity, new FallingCatData { velocity = worldVel });
            }
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
