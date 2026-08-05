using OMC.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace OMC {
    namespace Assets.CatsGame.Logic.Game {
        public class ExplosionSoundController : MonoBehaviour {
            private void Awake() {
                CatCountBridgingSystem.CatCountChangePositions += PlaySoundsAtPositions;
            }

            private void PlaySoundsAtPositions(int difference, DynamicBuffer<LostCatPosition> positions) {
                if (difference < 0) {
                    int count = math.min(math.abs(difference), 50);
                    for (int i = 0; i < count; i++) {
                        float3 pos = positions[i].value;
                        AudioPool.instance.PlayExplosionSoundAt(new Vector3(pos.x, pos.y, pos.z));
                    }
                }
            }

            private void OnDestroy() {
                CatCountBridgingSystem.CatCountChangePositions -= PlaySoundsAtPositions;
            }
        }
    }
}
