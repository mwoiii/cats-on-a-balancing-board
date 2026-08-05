using Unity.Entities;
using UnityEngine;

namespace OMC.ECS {
    public class EffectSpawnerAuthoring : MonoBehaviour {

        public GameObject explosionPrefab;

        public GameObject supernovaPrefab;

        public int maxExplosionCount = 1000;

        public int maxSupernovaCount = 100;

        class Baker : Baker<EffectSpawnerAuthoring> {
            public override void Bake(EffectSpawnerAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                if (authoring.explosionPrefab.TryGetComponent(out EffectAuthoring explosionAuthoring)) {
                    explosionAuthoring.type = EffectType.Explosion;
                }

                if (authoring.supernovaPrefab.TryGetComponent(out EffectAuthoring supernovaAuthoring)) {
                    supernovaAuthoring.type = EffectType.Supernova;
                }

                AddComponent(entity, new EffectSpawnerConfig {
                    explosionPrefab = GetEntity(authoring.explosionPrefab, TransformUsageFlags.None),
                    supernovaPrefab = GetEntity(authoring.supernovaPrefab, TransformUsageFlags.None),
                    maxExplosionCount = authoring.maxExplosionCount,
                    maxSupernovaCount = authoring.maxSupernovaCount
                });
            }
        }
    }

    public enum EffectType : byte {
        Misc,
        Explosion,
        Supernova,
    }

    public struct EffectSpawnerConfig : IComponentData {
        public Entity explosionPrefab;

        public Entity supernovaPrefab;

        public int maxExplosionCount;

        public int maxSupernovaCount;

        public int currentExplosionCount;

        public int currentSupernovaCount;
    }
}
