using OMC.ECS;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;


namespace OMC {
    public static class DynamicEffectManager {

        // todo - allow per-effect. effectdef
        public const int MaxPoolSize = 100;

        public static Dictionary<GameObject, EffectPool> pools;

        public static EffectController effectController;

        // todo - left in for ECS purposes, would be better to clean up
        public static GameObject explosionPrefab;

        public static GameObject supernovaPrefab;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() {
            pools = new Dictionary<GameObject, EffectPool>();
            SceneManager.sceneLoaded += (_, _) => {
                pools.Clear();
            };

            LoadStaticAssets();
            CatCountBridgingSystem.CatCountChangePositions += PlayExplosionAtPosition;
        }

        public static void SpawnEffect(GameObject prefab, Vector3 position) {
            if (pools.ContainsKey(prefab)) {
                pools[prefab].PlaceEffectAt(position);
            } else {
                if (prefab.TryGetComponent(out PooledEffect effect)) {
                    EffectPool newPool = new EffectPool(prefab);
                    pools[prefab] = newPool;
                    newPool.PlaceEffectAt(position);
                }

                if (!effectController) {
                    CreateEffectController();
                }
            }
        }

        private static void LoadStaticAssets() {
            explosionPrefab = Addressables.LoadAssetAsync<GameObject>("Effects/ExplosionPrefab").WaitForCompletion();
            supernovaPrefab = Addressables.LoadAssetAsync<GameObject>("Effects/SupernovaPrefab").WaitForCompletion();
        }

        private static void CreateEffectController() {
            GameObject controllerObject = new GameObject("EffectController");
            effectController = controllerObject.AddComponent<EffectController>();
        }

        public static void PlaySupernovaAtPosition(Vector3 pos) {
            AudioPool.instance.PlaySupernovaSoundAt(new Vector3(pos.x, pos.y, pos.z));
            SpawnEffect(supernovaPrefab, pos);
        }

        public static void PlayExplosionAtPosition(Vector3 pos) {
            AudioPool.instance.PlayExplosionSoundAt(new Vector3(pos.x, pos.y, pos.z));
            SpawnEffect(explosionPrefab, pos);
        }

        private static void PlayExplosionAtPosition(int difference, DynamicBuffer<LostCatPosition> positions) {
            if (difference < 0) {
                int count = math.min(math.abs(difference), 50);
                for (int i = 0; i < count; i++) {
                    float3 pos = positions[i].value;
                    PlayExplosionAtPosition(pos);
                }
            }
        }
    }
}
