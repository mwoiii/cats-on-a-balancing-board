using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct CatSpawnSystem : ISystem
{
    uint spawnCallCount;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CatSpawnerConfig>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        CatSpawnerConfig config = SystemAPI.GetSingleton<CatSpawnerConfig>();

        int countToSpawn = 0;
        if (!config.Finished)
        {
            countToSpawn = config.Count;
            config.Finished = true;
        }
        else if (config.PendingSpawn > 0)
        {
            countToSpawn = config.PendingSpawn;
            config.PendingSpawn = 0;
        }
        config.SpawnedThisUpdate = countToSpawn;

        if (countToSpawn > 0)
        {
            Random randomSauce = new(676767 + spawnCallCount);
            spawnCallCount++;

            NativeArray<Entity> cats = state.EntityManager.Instantiate(config.Prefab, countToSpawn, Allocator.Temp);

            foreach (Entity cat in cats)
            {
                float2 offset = randomSauce.NextFloat2Direction() * randomSauce.NextFloat(0f, config.Radius);
                CatData data = SystemAPI.GetComponent<CatData>(cat);
                data.Position = offset;
                state.EntityManager.SetComponentData(cat, data);

                LocalTransform catTransform = LocalTransform.FromPositionRotationScale(
                new float3(offset.x, config.DropHeight, offset.y), quaternion.identity, config.Scale
                );
                state.EntityManager.SetComponentData(cat,catTransform);
            }
            SystemAPI.SetSingleton(new InitialFallData {Height = config.DropHeight, Velocity = 0});
        }

        SystemAPI.SetSingleton(config);
    }
}

[UpdateAfter(typeof(CatSpawnSystem))]
public partial struct CatSpawnHUDSystem : ISystem // I want to keep the burst compilation for the main spawn system
{
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<CatSpawnerConfig>()){return;}

        CatSpawnerConfig config = SystemAPI.GetSingleton<CatSpawnerConfig>();
        if (config.SpawnedThisUpdate > 0 && HUDController.instance != null){HUDController.instance.UpdateRemainingCats(config.SpawnedThisUpdate);}
    }
}
