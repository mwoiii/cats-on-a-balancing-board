using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;

namespace Assets.CatsGame.Logic.Game {
    public class StaticEntityData : MonoBehaviour {

        public static EntityManager entityManager;

        public const float maxWaitTime = 1f;


        public static EffectSpawnerConfig effectConfig;


        private void Awake() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            // assumes the effectconfig is unchanging!!!!!
            StartCoroutine(GetSingletonComponent(
                entityManager.CreateEntityQuery(typeof(EffectSpawnerConfig)),
                (EffectSpawnerConfig component, bool success) => {
                    TryAssignValue("EffectConfig", out effectConfig, component, success);
                })
            );
        }

        private IEnumerator GetSingletonComponent<T>(EntityQuery query, Action<T, bool> callback) where T : unmanaged, IComponentData {
            float stopwatch = 0f;
            bool success = false;
            T component = default;

            while (stopwatch < maxWaitTime && !success) {
                success = query.TryGetSingleton(out component);
                stopwatch += Time.deltaTime;
                yield return null;
            }

            callback(component, success);
        }

        private void TryAssignValue<T>(string name, out T holder, T value, bool success) {
            if (success) {
                holder = value;
            } else {
                holder = default;
                Debug.LogError($"Failed to get singleton entity component {name}!!");
            }
        }
    }
}
