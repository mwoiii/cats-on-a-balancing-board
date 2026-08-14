using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace OMC.ECS {
    [UpdateAfter(typeof(CatMassSystem))]
    [UpdateAfter(typeof(StackingGridSystem))]
    [BurstCompile]
    public partial struct EntityRegulationSystem : ISystem {
        const int CatCountSplitThreshold = 20000;

        const int EntitiesOverTargetCombineThreshold = 20000;

        const int TargetEntityCount = 100000;

        const int BatchEntityMinimum = 17000;

        bool hasSplit;

        EntityQuery query;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<CatValue>();

            query = SystemAPI.QueryBuilder().WithAll<CatValue, MaterialMeshInfo>().WithDisabled<IsInitialFalling, FallingCatData, FallenCatData>().Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            int catCount = 0;
            foreach (var catVal in SystemAPI.Query<RefRO<CatValue>>().WithAll<MaterialMeshInfo>().WithDisabled<FallingCatData, FallenCatData>()) {
                catCount += catVal.ValueRO.value;
            }
            int entityCount = query.CalculateEntityCount();

            CatSpawnerConfig config = SystemAPI.GetSingleton<CatSpawnerConfig>();
            config.batchEntityTarget = math.max(BatchEntityMinimum, TargetEntityCount - entityCount);
            SystemAPI.SetSingleton(config);

            //Debug.Log($"cat count {catCount} ::: entity count {entityCount} ::: next batch entity target {config.batchEntityTarget}");

            CheckSplit(ref state, catCount);

            CheckCombine(ref state, entityCount);
        }

        void CheckSplit(ref SystemState state, int catCount) {
            if (catCount >= CatCountSplitThreshold) { hasSplit = false; return; }
            if (hasSplit) { return; }
            hasSplit = true;

            Unity.Mathematics.Random randomSauce = new(676767);
            EntityCommandBuffer ecb = new(Allocator.Temp);

            foreach (var (catVal, catData, entity) in SystemAPI.Query<RefRW<CatValue>, RefRO<CatData>>().WithAll<MaterialMeshInfo>().WithEntityAccess()) {
                int val = catVal.ValueRO.value;
                if (val <= 1) { continue; }

                catVal.ValueRW.value = 1;

                for (int i = 0; i < val - 1; i++) {
                    ecb.Instantiate(entity);
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            //Debug.Log("split");
        }

        void CheckCombine(ref SystemState state, int entityCount) {
            if (entityCount > TargetEntityCount + EntitiesOverTargetCombineThreshold) {
                NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);

                int cullableCount = 0;
                for (int i = 0; i < entities.Length; i++) {
                    if (SystemAPI.IsComponentEnabled<CanCull>(entities[i])) {
                        (entities[cullableCount], entities[i]) = (entities[i], entities[cullableCount]);
                        cullableCount++;
                    }
                }

                int excess = math.min(entityCount - TargetEntityCount, cullableCount);
                int survivors = entityCount - excess;

                EntityCommandBuffer ecb = new(Allocator.Temp);

                for (int i = 0; i < excess; i++) {
                    Entity food = entities[i];
                    Entity hungry = entities[excess + (i % survivors)];

                    CatValue foodVal = SystemAPI.GetComponent<CatValue>(food);
                    CatValue hungryVal = SystemAPI.GetComponent<CatValue>(hungry);
                    hungryVal.value += math.min(foodVal.value, int.MaxValue - hungryVal.value);
                    SystemAPI.SetComponent(hungry, hungryVal);

                    SystemAPI.SetComponent(food, new CatValue { value = 0 });
                    SystemAPI.SetComponentEnabled<MaterialMeshInfo>(food, false);
                    SystemAPI.SetComponentEnabled<CatData>(food, false);
                    SystemAPI.SetComponentEnabled<CatVelocity>(food, false);
                    SystemAPI.SetComponentEnabled<CatStack>(food, false);

                }

                ecb.Playback(state.EntityManager);
                ecb.Dispose();
                entities.Dispose();
                //Debug.Log("combined");
            }
        }
    }
}
