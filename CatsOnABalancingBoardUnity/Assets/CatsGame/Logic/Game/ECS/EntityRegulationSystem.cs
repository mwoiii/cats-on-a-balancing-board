using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace OMC.ECS {
    [UpdateAfter(typeof(CatMassSystem))]
    [UpdateAfter(typeof(StackingGridSystem))]
    [BurstCompile]
    public partial struct EntityRegulationSystem : ISystem
    {
        const int catCountSplitThreshold = 20000;
        bool hasSplit;

        const int entitiesOverTargetCombineThreshold = 20000;
        const int targetEntityCount = 100000; // should be the same as in CatSpawnSystem
        EntityQuery query;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CatValue>();

            query = SystemAPI.QueryBuilder().WithAll<CatValue, MaterialMeshInfo>().WithDisabled<IsInitialFalling,FallingCatData,FallenCatData>().Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int catCount = 0;
            foreach (var catVal in SystemAPI.Query<RefRO<CatValue>>().WithAll<MaterialMeshInfo>().WithDisabled<FallingCatData,FallenCatData>())
            {
                catCount += catVal.ValueRO.value;
            }
            int entityCount = query.CalculateEntityCount();
            
            //Debug.Log($"cat count {catCount} ::: entity count {entityCount}");

            CheckSplit(ref state, catCount);
            
            CheckCombine(ref state, entityCount);
        }

        void CheckSplit(ref SystemState state, int catCount)
        {
            if (catCount >= catCountSplitThreshold){hasSplit = false; return;}
            if (hasSplit){return;}
            hasSplit = true;

            Unity.Mathematics.Random randomSauce = new(676767);
            EntityCommandBuffer ecb = new(Allocator.Temp);
            
            foreach(var (catVal, catData, entity) in SystemAPI.Query<RefRW<CatValue>, RefRO<CatData>>().WithAll<MaterialMeshInfo>().WithEntityAccess())
            {
                int val = catVal.ValueRO.value;
                if (val <= 1){continue;}

                catVal.ValueRW.value = 1;

                for (int i = 0; i < val-1; i++)
                {
                    ecb.Instantiate(entity);
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            //Debug.Log("split");
        }

        void CheckCombine(ref SystemState state, int entityCount)
        {
            if (entityCount > targetEntityCount + entitiesOverTargetCombineThreshold)
            {
                NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);

                int cullableCount = 0;
                for (int i = 0; i < entities.Length; i++)
                {
                    if (SystemAPI.IsComponentEnabled<CanCull>(entities[i]))
                    {
                        (entities[cullableCount],entities[i]) = (entities[i],entities[cullableCount]);
                        cullableCount++;
                    }
                }

                int excess = math.min(entityCount -  targetEntityCount,cullableCount);
                int survivors = entityCount - excess;

                EntityCommandBuffer ecb = new(Allocator.Temp);

                for (int i=0; i < excess; i++)
                {
                    Entity food = entities[i];
                    Entity hungry = entities[excess + (i % survivors)];

                    CatValue foodVal = SystemAPI.GetComponent<CatValue>(food);
                    CatValue hungryVal = SystemAPI.GetComponent<CatValue>(hungry);
                    hungryVal.value += foodVal.value;
                    SystemAPI.SetComponent(hungry,hungryVal);

                    SystemAPI.SetComponentEnabled<MaterialMeshInfo>(food,false); // cat cleanup should get this
                }

                ecb.Playback(state.EntityManager);
                ecb.Dispose();
                entities.Dispose();
                //Debug.Log("combined");
            }
        }
    }
}
