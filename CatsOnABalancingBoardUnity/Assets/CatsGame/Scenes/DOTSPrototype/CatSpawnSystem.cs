using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct CatSpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CatSpawnerConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        CatSpawnerConfig config = SystemAPI.GetSingleton<CatSpawnerConfig>();
        if (config.Finished) return;

        Random randomSauce = new(676767);
        NativeArray<Entity> cats = state.EntityManager.Instantiate(config.Prefab, config.Count, Allocator.Temp);

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

        config.Finished = true;
        SystemAPI.SetSingleton(config);
    }
}
