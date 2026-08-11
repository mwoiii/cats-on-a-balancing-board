using OMC.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace OMC {
    public class OMCEffectSpawner : MonoBehaviour {
        public static EffectDef explosionDef;

        public static EffectDef supernovaDef;

        private static bool assetsLoaded;

        private void Awake() {
            if (!assetsLoaded) {
                LoadAssets();
                assetsLoaded = true;
            }

            CatCountBridgingSystem.CatCountChangePositions += PlayExplosionAtPosition;
        }

        private void OnDestroy() {
            CatCountBridgingSystem.CatCountChangePositions -= PlayExplosionAtPosition;
        }

        private static void LoadAssets() {
            explosionDef = Addressables.LoadAssetAsync<EffectDef>("Effects/edExplosion").WaitForCompletion();
            supernovaDef = Addressables.LoadAssetAsync<EffectDef>("Effects/edSupernova").WaitForCompletion();
        }

        public static void PlaySupernovaAtPosition(Vector3 pos) {
            AudioPool.instance.PlaySupernovaSoundAt(new Vector3(pos.x, pos.y, pos.z));
            DynamicEffectManager.SpawnEffect(supernovaDef, pos);
        }

        public static void PlayExplosionAtPosition(Vector3 pos) {
            AudioPool.instance.PlayExplosionSoundAt(new Vector3(pos.x, pos.y, pos.z));
            DynamicEffectManager.SpawnEffect(explosionDef, pos);
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
