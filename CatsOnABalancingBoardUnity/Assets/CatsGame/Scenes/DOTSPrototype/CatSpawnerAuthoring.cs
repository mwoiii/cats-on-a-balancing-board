using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CatSpawnerAuthoring : MonoBehaviour
{
    public GameObject catPrefab;
    public float dropHeight = 0.5f;
    public int count = 1000;
    public float radius = 5f;
    public float scale = 0.05f;

    class Baker : Baker<CatSpawnerAuthoring>
    {
        public override void Bake(CatSpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new CatSpawnerConfig
            {
                Prefab = GetEntity(authoring.catPrefab, TransformUsageFlags.Dynamic),
                Count = authoring.count,
                Radius = authoring.radius,
                DropHeight = authoring.dropHeight,
                Scale = authoring.scale,
                Finished = false
            });
            AddComponent(entity, new InitialFallData{Height = 0, Velocity = 0});
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

public struct CatSpawnerConfig : IComponentData
{
    public Entity Prefab;
    public int Count;
    public float Radius;
    public float DropHeight;
    public float Scale;
    public bool Finished;

    public int PendingSpawn;
    public int SpawnedThisUpdate;
}
