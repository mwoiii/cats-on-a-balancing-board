using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace OMC {
    public static class DynamicEffectManager {

        public static Dictionary<EffectDef, EffectPool> pools;

        public static EffectController effectController;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() {
            pools = new Dictionary<EffectDef, EffectPool>();
            SceneManager.sceneLoaded += (_, _) => {
                pools.Clear();
            };
        }

        public static void SpawnEffect(EffectDef effectDef, Vector3 position) {
            if (pools.ContainsKey(effectDef)) {
                pools[effectDef].PlaceEffectAt(position);
            } else {
                if (effectDef.effectPrefab.TryGetComponent(out PooledEffect effect)) {
                    EffectPool newPool = new EffectPool(effectDef);
                    pools[effectDef] = newPool;
                    newPool.PlaceEffectAt(position);
                } else {
                    Debug.LogError($"Prefab on EffectDef {effectDef.name} is missing a PooledEffect component!");
                }

                if (!effectController) {
                    CreateEffectController();
                }
            }
        }

        private static void CreateEffectController() {
            GameObject controllerObject = new GameObject("EffectController");
            effectController = controllerObject.AddComponent<EffectController>();
        }
    }
}
