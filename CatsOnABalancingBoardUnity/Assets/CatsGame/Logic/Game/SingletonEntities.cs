using Unity.Entities;
using UnityEngine;

namespace Assets.CatsGame.Logic.Game {
    public class SingletonEntities : MonoBehaviour {
        // someone should populate this with the other singletons.....
        // for it seems that entities and monobehaviours don't run in a predictable order
        // i.e. the other singletons may just not be acquired in the future. (this just happened....)

        public static EntityManager entityManager;


        public static EffectSpawnerConfig effectConfig;

        private EntityQuery effectConfigQuery;

        private bool gotEffectConfig;

        private void Awake() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            effectConfigQuery = entityManager.CreateEntityQuery(typeof(EffectSpawnerConfig));
        }

        private void Update() {
            if (!gotEffectConfig && effectConfigQuery.TryGetSingleton(out effectConfig)) {
                gotEffectConfig = true;
            }
        }
    }
}
