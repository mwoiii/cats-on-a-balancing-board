using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(CatProjectionSystem))]
[BurstCompile]
public partial struct CatMovementSystem : ISystem
{
    const float friction = 2;
    const float moveForce = 1.2f;
    const float reactDistance = 1.5f;
    const float dispersionPerCat = 0.002f;
    const float maxDispersion =2f;
    const float catnipContactRadius = 0.1f;
    
    EntityQuery catQuery;
    uint frameCounter; // new random every frame

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BoardTransform>();
        state.RequireForUpdate<WeightSnapshot>();
        catQuery = SystemAPI.QueryBuilder().WithAll<CatData>().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BoardTransform board = SystemAPI.GetSingleton<BoardTransform>();
        DynamicBuffer<WeightSnapshot> weights = SystemAPI.GetSingletonBuffer<WeightSnapshot>();
        float deltaTime = SystemAPI.Time.DeltaTime;

        float3 gravityWorld = new(0,-9.81f,0);
        float3 gravityLocal = math.mul(math.inverse(board.Rotation), gravityWorld); // Imagine rotating a cube by thirty degrees
        float2 down = new(gravityLocal.x,gravityLocal.z);

        int catCount = catQuery.CalculateEntityCount();
        float dispersionStrength = math.min(catCount * dispersionPerCat, maxDispersion);
        frameCounter++;

        EntityCommandBuffer ecb = new(Allocator.Temp);

        foreach (var (catData, catVelocity, entity) in SystemAPI.Query<RefRW<CatData>, RefRW<CatVelocity>>().WithEntityAccess())
        {
            // weight reactive behaviour
            float2 catPos = catData.ValueRO.Position;
            
            float nearestBasicDist = reactDistance;
            float2 nearestBasicPos = float2.zero;
            bool hasBasic = false;

            float nearestCatnipDist = float.MaxValue;
            float2 nearestCatnipPos = float2.zero;
            int nearestCatnipIndex = -1;
            bool hasCatnip = false;

            float nearestLemonDist = reactDistance;
            float2 nearestLemonPos = float2.zero;
            bool hasLemon = false;

            for (int i = 0; i < weights.Length; i++)
            {
                WeightSnapshot w = weights[i];
                float dist = math.distancesq(catPos, w.LocalPosition); // distance squared saves a square root operation but i maybe should be more precise with variable names

                if (w.Type == WeightBehaviour.WeightType.None && w.State == WeightBehaviour.WeightState.Falling && dist < nearestBasicDist)
                {
                    nearestBasicDist = dist;
                    nearestBasicPos = w.LocalPosition;
                    hasBasic = true;
                }
                if (w.Type == WeightBehaviour.WeightType.Catnip && dist < nearestCatnipDist)
                {
                    nearestCatnipDist = dist;
                    nearestCatnipPos = w.LocalPosition;
                    nearestCatnipIndex = i;
                    hasCatnip = true;
                }
                if (w.Type == WeightBehaviour.WeightType.Lemon && dist < nearestLemonDist)
                {
                    nearestLemonDist = dist;
                    nearestLemonPos = w.LocalPosition;
                    hasLemon = true;
                }
            }

            float2 weightForce = float2.zero;
            if (hasBasic) // can the cat has basic
            {
                float2 toTarget = nearestBasicPos - catPos;
                if (math.lengthsq(toTarget) > 0){weightForce -= math.normalize(toTarget)*moveForce;}
            }
            if (hasCatnip)
            {
                // random dispersion
                Random randomSauce = Random.CreateFromIndex((uint)entity.Index * 67 + frameCounter * 21);
                float2 dispersion = randomSauce.NextFloat2Direction() * dispersionStrength;
                float2 toTarget = nearestCatnipPos - catPos + dispersion;
                if (math.lengthsq(toTarget) > 0){weightForce += math.normalize(toTarget)*moveForce;}

                if (nearestCatnipDist < catnipContactRadius && weights[nearestCatnipIndex].State == WeightBehaviour.WeightState.Landed)
                {
                    DynamicBuffer<WeightContactPulse> pulses = SystemAPI.GetSingletonBuffer<WeightContactPulse>();
                    pulses.ElementAt(nearestCatnipIndex).Count++;
                }
            }
            if (hasLemon)
            {
                float2 toTarget = nearestLemonPos - catPos;
                if (math.lengthsq(toTarget) > 0){weightForce -= math.normalize(toTarget)*moveForce;}
            }

            // forces applied (gravity and friction also thrown in here)
            catVelocity.ValueRW.Value += (down + weightForce) * deltaTime;
            catVelocity.ValueRW.Value *= math.max(0,1-friction * deltaTime);
            catData.ValueRW.Position += catVelocity.ValueRW.Value * deltaTime;


            if (math.length(catData.ValueRO.Position) > board.Radius) // if cat fallen off...
            {
                float3 lastLocalPos = new(catData.ValueRO.Position.x,0,catData.ValueRO.Position.y);
                float3 worldPos = board.Position + math.mul(board.Rotation, lastLocalPos);
                
                float3 lastLocalVel = new(catVelocity.ValueRO.Value.x,0,catVelocity.ValueRO.Value.y);
                float3 worldVel = math.mul(board.Rotation,lastLocalVel);

                ecb.RemoveComponent<CatData>(entity);
                ecb.RemoveComponent<CatVelocity>(entity);
                
                ecb.SetComponent(entity, LocalTransform.FromPosition(worldPos));
                ecb.AddComponent(entity, new FallingCatData{Velocity = worldVel});
            }
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
