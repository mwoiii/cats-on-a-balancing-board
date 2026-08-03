using Unity.Entities;
using UnityEngine;

public class EffectSpawnerAuthoring : MonoBehaviour {
    public GameObject explosionPrefab;

    public GameObject supernovaPrefab;

    public int maxExplosionCount = 1000;

    class Baker : Baker<EffectSpawnerAuthoring> {
        public override void Bake(EffectSpawnerAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new EffectSpawnerConfig {
                explosionPrefab = GetEntity(authoring.explosionPrefab, TransformUsageFlags.None),
                supernovaPrefab = GetEntity(authoring.supernovaPrefab, TransformUsageFlags.None),
                maxExplosionCount = authoring.maxExplosionCount
            });
        }
    }
}

public struct EffectSpawnerConfig : IComponentData {
    public Entity explosionPrefab;

    public Entity supernovaPrefab;

    public int maxExplosionCount;

    public int currentExplosionCount;
}
