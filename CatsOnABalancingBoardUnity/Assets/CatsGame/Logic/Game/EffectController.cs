using UnityEngine;
using static OMC.EffectPool;

namespace OMC {
    public class EffectController : MonoBehaviour {

        public static EffectController instance;

        private float nextCullTime;

        private void Awake() {
            instance = this;
        }

        private void Update() {
            if (Time.time > nextCullTime) {
                CullStaleEffects();
            }
        }

        private void CullStaleEffects() {
            float currentTime = Time.time;

            foreach (EffectPool pool in DynamicEffectManager.pools.Values) {
                if (pool.available.Count <= 0) {
                    continue;
                }

                int toCull = 0;

                foreach (float time in pool.returnTimes) {
                    if (currentTime > time + TimeBeforeCull) {
                        toCull++;
                    } else {
                        break;
                    }
                }

                if (toCull > 0) {
                    Debug.Log($"Culling {toCull} stale effects from {pool.effectPrefab.name} pool");
                }

                for (int i = 0; i < toCull; i++) {
                    pool.returnTimes.DequeueFront();
                    Destroy(pool.available.Pop().gameObject);
                }
            }

            nextCullTime = currentTime + 10f;
        }
    }
}
