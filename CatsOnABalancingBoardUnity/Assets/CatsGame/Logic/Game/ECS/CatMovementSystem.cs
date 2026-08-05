using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static OMC.WeightBehaviour;

namespace OMC.ECS {
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

            EntityCommandBuffer ecb = new(Allocator.TempJob);
            NativeQueue<int> catnipHits = new(Allocator.TempJob);

            var job = new CatMovementJob {
                weights = weights,
                board = board,
                down = down,
                deltaTime = deltaTime,
                frameCounter = frameCounter,
                dispersionStrength = dispersionStrength,
                catnipHits = catnipHits.AsParallelWriter(),
                ecb = ecb.AsParallelWriter()
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
            state.Dependency.Complete();

            while (catnipHits.TryDequeue(out int index)) {
                pulses.ElementAt(index).count++;
            }
            catnipHits.Dispose();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [WithDisabled(typeof(IsInitialFalling))]
        [BurstCompile]
        partial struct CatMovementJob : IJobEntity {
            [ReadOnly] public DynamicBuffer<WeightSnapshot> weights;
            public BoardTransform board;
            public float2 down;
            public float deltaTime;
            public uint frameCounter;
            public float dispersionStrength;
            public NativeQueue<int>.ParallelWriter catnipHits;
            public EntityCommandBuffer.ParallelWriter ecb;

            [BurstCompile]
            void Execute([ChunkIndexInQuery] int sortKey, Entity entity, ref CatData catData, ref CatVelocity catVelocity) {
                Random randomSauce = Random.CreateFromIndex((uint)entity.Index * 67 + frameCounter * 21); // some Bezout shenanigans going on here

                // weight reactive behaviour
                float2 catPos = catData.position;

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

                    switch (w.type) {
                        case WeightType.None:
                            if (w.state == WeightState.Falling && dist < nearestBasicDist) {
                                nearestBasicDist = dist;
                                nearestBasicPos = w.localPosition;
                                hasBasic = true;
                            }
                            break;
                        case WeightType.Catnip:
                            if (dist < nearestCatnipDist) {
                                nearestCatnipDist = dist;
                                nearestCatnipPos = w.localPosition;
                                nearestCatnipIndex = i;
                                hasCatnip = true;
                            }
                            break;
                        case WeightType.Lemon:
                            if (dist < nearestLemonDist) {
                                nearestLemonDist = dist;
                                nearestLemonPos = w.localPosition;
                                hasLemon = true;
                            }
                            break;
                    }
                }

                float2 weightForce = float2.zero;
                if (hasBasic) // can the cat has basic
                {
                    float2 toTarget = nearestBasicPos - catPos;
                    if (math.lengthsq(toTarget) > 0) {
                        weightForce -= math.normalize(toTarget) * moveForce;
                    }
                }
                if (hasCatnip) {
                    float2 toTarget = nearestCatnipPos - catPos;
                    if (math.lengthsq(toTarget) > 0) {
                        weightForce += math.normalize(toTarget) * moveForce;
                    }

                    if (nearestCatnipDist < catnipContactRadius && weights[nearestCatnipIndex].state == WeightBehaviour.WeightState.Landed) {
                        catnipHits.Enqueue(nearestCatnipIndex);
                    }
                }
                if (hasLemon) {
                    float2 toTarget = nearestLemonPos - catPos;
                    if (math.lengthsq(toTarget) > 0) { weightForce -= math.normalize(toTarget) * moveForce; }
                }

                // random dispersion
                float2 dispersion = randomSauce.NextFloat2Direction() * dispersionStrength;

                // forces applied (gravity and friction also thrown in here)
                catVelocity.value += (down + weightForce + dispersion) * deltaTime;
                catVelocity.value *= math.max(0, 1 - friction * deltaTime);
                catData.position += catVelocity.value * deltaTime;

                if (math.length(catData.position) > board.radius) // if cat fallen off...
                {
                    float3 lastLocalPos = new(catData.position.x, 0.1f, catData.position.y);
                    float3 worldPos = board.position + math.mul(board.rotation, lastLocalPos);

                    float3 lastLocalVel = new(catVelocity.value.x, 0, catVelocity.value.y);
                    float3 worldVel = math.mul(board.rotation, lastLocalVel);

                    ecb.SetComponentEnabled<CatData>(sortKey, entity, false);
                    ecb.SetComponentEnabled<CatVelocity>(sortKey, entity, false);

                    ecb.SetComponent(sortKey, entity, LocalTransform.FromPosition(worldPos));
                    ecb.SetComponentEnabled<FallingCatData>(sortKey, entity, true);
                    ecb.SetComponent(sortKey, entity, new FallingCatData { velocity = worldVel });
                }
            }
        }
    }
}
