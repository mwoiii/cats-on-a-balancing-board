using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static OMC.WeightBehaviour;

namespace OMC.ECS {
    [UpdateBefore(typeof(CatProjectionSystem))]
    [UpdateAfter(typeof(CatExplosionSystem))]
    [BurstCompile]
    public partial struct CatMovementSystem : ISystem {
        const float Friction = 2;

        const float GripStrength = 2;

        const float MoveForce = 1.2f;

        const float ReactDistance = 1.5f;

        const float DispersionPerCat = 0.0002f;

        const float MaxDispersion = 1f;

        const float CatnipContactRadius = 0.05f;

        EntityQuery catQuery;
        uint frameCounter; // new random every frame

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BoardTransform>();
            state.RequireForUpdate<WeightSnapshot>();
            catQuery = SystemAPI.QueryBuilder().WithAll<CatData>().Build();
            state.EntityManager.CreateSingleton<CatMovementHold>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            BoardTransform board = SystemAPI.GetSingleton<BoardTransform>();
            DynamicBuffer<WeightSnapshot> weights = SystemAPI.GetSingletonBuffer<WeightSnapshot>();
            float deltaTime = SystemAPI.Time.DeltaTime;

            float3 gravityWorld = new(0, -9.81f, 0);
            float3 gravityLocal = math.mul(math.inverse(board.rotation), gravityWorld); // Imagine rotating a cube by thirty degrees
            float2 down = new(gravityLocal.x, gravityLocal.z);

            float frictionWithGrip = Friction + GripStrength * math.length(down);

            int catCount = catQuery.CalculateEntityCount();
            float dispersionStrength = math.min(catCount * DispersionPerCat, MaxDispersion);
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
                frictionWithGrip = frictionWithGrip,
                catnipHits = catnipHits.AsParallelWriter(),
                ecb = ecb.AsParallelWriter()
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);

            SystemAPI.SetSingleton(new CatMovementHold { ecb = ecb, catnipHits = catnipHits, pending = true });
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
            public float frictionWithGrip;
            public NativeQueue<int>.ParallelWriter catnipHits;
            public EntityCommandBuffer.ParallelWriter ecb;

            [BurstCompile]
            void Execute([ChunkIndexInQuery] int sortKey, Entity entity, ref CatData catData, ref CatVelocity catVelocity) {
                Random randomSauce = Random.CreateFromIndex((uint)entity.Index * 67 + frameCounter * 21); // some Bezout shenanigans going on here

                // weight reactive behaviour
                float2 catPos = catData.position;

                float nearestNoneDist = ReactDistance + randomSauce.NextFloat(-0.2f, 0.2f);
                float2 nearestNonePos = float2.zero;
                bool hasNone = false;

                float nearestCatnipDist = float.MaxValue;
                float2 nearestCatnipPos = float2.zero;
                int nearestCatnipIndex = -1;
                bool hasCatnip = false;

                float nearestLemonDist = ReactDistance + randomSauce.NextFloat(-0.2f, 0.2f);
                float2 nearestLemonPos = float2.zero;
                bool hasLemon = false;

                float nearestWhirlpoolDist = float.MaxValue;
                float2 nearestWhirlpoolPos = float2.zero;
                bool hasWhirlpool = false;

                for (int i = 0; i < weights.Length; i++) {
                    WeightSnapshot w = weights[i];
                    float dist = math.distancesq(catPos, w.localPosition); // distance squared saves a square root operation but i maybe should be more precise with variable names

                    switch (w.type) {
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
                        case WeightType.Whirlpool:
                            if (dist < nearestWhirlpoolDist) {
                                nearestWhirlpoolDist = dist;
                                nearestWhirlpoolPos = w.localPosition;
                                hasWhirlpool = true;
                            }
                            break;
                        default:
                            if (w.state == WeightState.Falling && dist < nearestNoneDist) {
                                nearestNoneDist = dist;
                                nearestNonePos = w.localPosition;
                                hasNone = true;
                            }
                            break;
                    }
                }

                float2 weightForce = float2.zero;
                if (hasNone) // can the cat has None
                {
                    float2 toTarget = nearestNonePos - catPos;
                    if (math.lengthsq(toTarget) > 0) {
                        weightForce -= math.normalize(toTarget) * MoveForce;
                    }
                }
                if (hasCatnip) {
                    float2 toTarget = nearestCatnipPos - catPos;
                    if (math.lengthsq(toTarget) > 0) {
                        weightForce += math.normalize(toTarget) * MoveForce;
                    }

                    if (nearestCatnipDist < CatnipContactRadius && weights[nearestCatnipIndex].state == WeightBehaviour.WeightState.Landed) {
                        catnipHits.Enqueue(nearestCatnipIndex);
                    }
                }
                if (hasLemon) {
                    float2 toTarget = nearestLemonPos - catPos;
                    if (math.lengthsq(toTarget) > 0) { weightForce -= math.normalize(toTarget) * MoveForce; }
                }
                if (hasWhirlpool) {
                    float2 toTarget = nearestWhirlpoolPos - catPos;
                    if (math.lengthsq(toTarget) > 0) {
                        float2 dir = math.normalize(toTarget);
                        float2 tangent = new float2(-dir.y, dir.x);
                        weightForce += (dir + tangent) * MoveForce;
                    }
                }

                // random dispersion
                float2 dispersion = randomSauce.NextFloat2Direction() * dispersionStrength;

                // forces applied (gravity and friction also thrown in here)
                catVelocity.value += (down + weightForce + dispersion) * deltaTime;
                catVelocity.value *= math.max(0, 1 - frictionWithGrip * deltaTime);
                catData.position += catVelocity.value * deltaTime;

                if (math.length(catData.position) > board.radius) {  // if cat fallen off...
                    float3 lastLocalPos = new(catData.position.x, 0.1f, catData.position.y);

                    float3 lastLocalVel = new(catVelocity.value.x, 0, catVelocity.value.y);
                    float3 worldVel = math.mul(board.rotation, lastLocalVel);

                    ecb.SetComponentEnabled<CatData>(sortKey, entity, false);
                    ecb.SetComponentEnabled<CatVelocity>(sortKey, entity, false);
                    ecb.SetComponentEnabled<CatStack>(sortKey, entity, false);

                    ecb.SetComponentEnabled<FallingCatData>(sortKey, entity, true);
                    ecb.SetComponent(sortKey, entity, new FallingCatData { velocity = worldVel });
                }
            }
        }
    }

    public struct CatMovementHold : IComponentData {
        public EntityCommandBuffer ecb;
        public NativeQueue<int> catnipHits;
        public bool pending;
    }

    [UpdateAfter(typeof(CatMovementSystem))]
    [UpdateBefore(typeof(CatProjectionSystem))]
    public partial struct CatMovementResolveSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<CatMovementHold>();
        }

        public void OnUpdate(ref SystemState state) {
            var hold = SystemAPI.GetSingleton<CatMovementHold>();
            if (!hold.pending) { return; }

            state.Dependency.Complete();

            DynamicBuffer<WeightContactPulse> pulses = SystemAPI.GetSingletonBuffer<WeightContactPulse>();
            while (hold.catnipHits.TryDequeue(out int index)) {
                pulses.ElementAt(index).count++;
            }
            hold.catnipHits.Dispose();

            hold.ecb.Playback(state.EntityManager);
            hold.ecb.Dispose();

            hold.pending = false;
            SystemAPI.SetSingleton(hold);
        }
    }
}
