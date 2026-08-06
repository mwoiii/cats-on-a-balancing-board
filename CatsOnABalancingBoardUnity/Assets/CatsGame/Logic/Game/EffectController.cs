using OMC.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace OMC {
    public class EffectController : MonoBehaviour {

        public static EffectController instance;

        [SerializeField]
        private EffectPool explosionPool;

        [SerializeField]
        private EffectPool supernovaPool;

        private void Awake() {
            instance = this;
            CatCountBridgingSystem.CatCountChangePositions += PlayExplosionAtPosition;
        }

        public void PlaySupernovaAtPosition(Vector3 pos) {
            AudioPool.instance.PlaySupernovaSoundAt(new Vector3(pos.x, pos.y, pos.z));
            supernovaPool.PlaceEffectAt(pos);
        }

        public void PlayExplosionAtPosition(Vector3 pos) {
            AudioPool.instance.PlayExplosionSoundAt(new Vector3(pos.x, pos.y, pos.z));
            explosionPool.PlaceEffectAt(pos);
        }

        private void PlayExplosionAtPosition(int difference, DynamicBuffer<LostCatPosition> positions) {
            if (difference < 0) {
                int count = math.min(math.abs(difference), 50);
                for (int i = 0; i < count; i++) {
                    float3 pos = positions[i].value;
                    AudioPool.instance.PlayExplosionSoundAt(new Vector3(pos.x, pos.y, pos.z));
                    explosionPool.PlaceEffectAt(pos);
                }
            }
        }

        private void OnDestroy() {
            CatCountBridgingSystem.CatCountChangePositions -= PlayExplosionAtPosition;
        }
    }
}
