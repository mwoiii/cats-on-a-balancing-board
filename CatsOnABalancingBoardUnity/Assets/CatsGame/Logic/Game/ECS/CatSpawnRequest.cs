using Unity.Entities;

namespace OMC.ECS {
    public static class CatSpawnRequest {
        public static void Enqueue(int count) {
            World world = World.DefaultGameObjectInjectionWorld;
            if (world == null) {
                return;
            }

            EntityManager boss = world.EntityManager;
            EntityQuery query = boss.CreateEntityQuery(typeof(CatSpawnerConfig));
            if (query.IsEmpty) {
                return;
            }

            Entity configEntity = query.GetSingletonEntity();

            CatSpawnerConfig config = boss.GetComponentData<CatSpawnerConfig>(configEntity);
            config.pendingSpawn += count;
            boss.SetComponentData(configEntity, config);
        }
    }
}
