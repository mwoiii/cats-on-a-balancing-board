using OMC.Util;
using System.Collections.Generic;
using UnityEngine;
using static OMC.DynamicEffectManager;

namespace OMC {
    public class EffectPool {

        public GameObject effectPrefab;

        public Stack<PooledEffect> available;

        public Deque<float> returnTimes;

        // todo - allow per effect. effectdef........
        public const float TimeBeforeCull = 30f;

        private int countActive;

        public EffectPool(GameObject effectPrefab) {
            this.effectPrefab = effectPrefab;
            available = new Stack<PooledEffect>(MaxPoolSize);
            returnTimes = new Deque<float>(MaxPoolSize);
        }

        public void PlaceEffectAt(Vector3 position) {
            if (countActive >= MaxPoolSize) {
                return;
            }

            PooledEffect effect;
            if (available.Count > 0) {
                effect = available.Pop();
                returnTimes.DequeueBack();
                effect.released = false;
                effect.gameObject.SetActive(true);
            } else {
                effect = CreateNewEffect();
            }
            effect.transform.position = position;

            countActive++;
        }

        public PooledEffect CreateNewEffect() {
            GameObject effectObject = Object.Instantiate(effectPrefab);
            PooledEffect effect = effectObject.GetComponent<PooledEffect>();
            effect.pool = this;
            return effect;
        }

        public void Release(PooledEffect effect) {
            if (!effect.released) {
                if (available.Count >= MaxPoolSize) {
                    Object.Destroy(effect.gameObject);
                    return;
                }
                effect.gameObject.SetActive(false);
                available.Push(effect);
                returnTimes.EnqueueBack(Time.time);
                effect.released = true;
                countActive--;
            }
        }
    }
}
