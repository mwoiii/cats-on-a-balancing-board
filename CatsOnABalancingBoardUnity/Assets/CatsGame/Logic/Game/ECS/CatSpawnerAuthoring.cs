using Unity.Entities;
using UnityEngine;

namespace OMC.ECS {
    public class CatSpawnerAuthoring : MonoBehaviour {
        public GameObject catPrefab;

        public float dropHeight = 0.5f;

        public int count = 1000;

        public float radius = 5f;

        public float scale = 0.05f;

        class Baker : Baker<CatSpawnerAuthoring> {
            public override void Bake(CatSpawnerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new CatSpawnerConfig {
                    prefab = GetEntity(authoring.catPrefab, TransformUsageFlags.Dynamic),
                    count = authoring.count,
                    radius = authoring.radius,
                    dropHeight = authoring.dropHeight,
                    scale = authoring.scale,
                    finished = false,
                    batchEntityTarget = 100000
                });
                AddComponent(entity, new InitialFallData { height = 0, velocity = 0 });
            }
        }
    }

    public struct CatSpawnerConfig : IComponentData {
        public Entity prefab;
        public int count;
        public float radius;
        public float dropHeight;
        public float scale;
        public bool finished;
        public int pendingSpawn;
        public int spawnedThisUpdate;
        public int batchEntityTarget;
    }
}
