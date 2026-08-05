using Assets.CatsGame.Logic.Game;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateBefore(typeof(CatCountBridgingSystem))]
public partial struct CatSpawnSystem : ISystem {
    uint spawnCallCount;

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<CatSpawnerConfig>();
        state.RequireForUpdate<CatCount>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        CatSpawnerConfig config = SystemAPI.GetSingleton<CatSpawnerConfig>();
        var catCount = SystemAPI.GetSingletonRW<CatCount>();

        int countToSpawn = 0;
        if (!config.finished) {
            countToSpawn = config.count;
            config.finished = true;
        } else if (config.pendingSpawn > 0) {
            countToSpawn = config.pendingSpawn;
            config.pendingSpawn = 0;
        }
        config.spawnedThisUpdate = countToSpawn;
        catCount.ValueRW.gained += countToSpawn;


        if (countToSpawn > 0) {
            Random randomSauce = new(676767 + spawnCallCount);
            spawnCallCount++;

            NativeArray<Entity> cats = state.EntityManager.Instantiate(config.prefab, countToSpawn, Allocator.Temp);

            foreach (Entity cat in cats) {
                float2 offset = randomSauce.NextFloat2Direction() * randomSauce.NextFloat(0f, config.radius);
                CatData data = SystemAPI.GetComponent<CatData>(cat);
                data.position = offset;
                state.EntityManager.SetComponentData(cat, data);

                LocalTransform catTransform = LocalTransform.FromPositionRotationScale(
                new float3(offset.x, config.dropHeight, offset.y), quaternion.identity, config.scale
                );
                state.EntityManager.SetComponentData(cat, catTransform);

                // Spribnkes...
                //float3 hugh = HSVToRGBBurstable(randomSauce.NextFloat(), 1f, 1f);
                //state.EntityManager.SetComponentData(cat, new URPMaterialPropertyBaseColor { Value = new float4(hugh,1f)});

            }
            SystemAPI.SetSingleton(new InitialFallData { height = config.dropHeight, velocity = 0 });
        }

        SystemAPI.SetSingleton(config);
    }

    static float3 HSVToRGBBurstable(float h, float s, float v) {
        float3 p = math.abs(math.frac(h + new float3(1f, 2f / 3f, 1f / 3f)) * 6f - 3f);
        float3 rgb = math.saturate(p - 1f);
        return v * math.lerp(new float3(1f), rgb, s);
    }
}
