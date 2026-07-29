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
            float3 pos = new(offset.x, config.DropHeight, offset.y);
            state.EntityManager.SetComponentData(cat, LocalTransform.FromPositionRotationScale(pos,quaternion.identity,config.Scale));
        }

        config.Finished = true;
        SystemAPI.SetSingleton(config);
    }
}
