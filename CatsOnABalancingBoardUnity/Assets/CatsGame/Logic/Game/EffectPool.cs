using OMC.Util;
using System.Collections.Generic;
using UnityEngine;

namespace OMC {
    public class EffectPool {
        public EffectDef effectDef;

        public Stack<PooledEffect> available;

        public Deque<float> returnTimes;

        public Queue<PooledEffect> inUse;

        private int countActive;

        public EffectPool(EffectDef effectDef) {
            this.effectDef = effectDef;
            available = new Stack<PooledEffect>(effectDef.maxPoolSize);
            returnTimes = new Deque<float>(effectDef.maxPoolSize);
            inUse = new Queue<PooledEffect>(effectDef.maxEffectQuantity);
        }

        public void PlaceEffectAt(Vector3 position) {

            PooledEffect effect;

            if (countActive >= effectDef.maxEffectQuantity) {
                if (!effectDef.prioritiseNewEffects) {
                    return;
                }
                inUse.TryDequeue(out effect);
                if (effect) {
                    effect.gameObject.SetActive(false);
                    effect.gameObject.SetActive(true);
                    countActive--;
                }
            } else if (available.Count > 0) {
                effect = available.Pop();
                returnTimes.DequeueBack();
                effect.released = false;
                effect.gameObject.SetActive(true);
            } else {
                effect = CreateNewEffect();
            }

            if (effect) {
                effect.transform.position = position;
                inUse.Enqueue(effect);
                countActive++;
            } else {
                Debug.LogError($"Failed to retrieve effect from inUse queue for {effectDef.name}!");
            }
        }

        public PooledEffect CreateNewEffect() {
            GameObject effectObject = Object.Instantiate(effectDef.effectPrefab);
            PooledEffect effect = effectObject.GetComponent<PooledEffect>();
            effect.pool = this;
            return effect;
        }

        public void Release(PooledEffect effect) {
            if (!effect.released) {
                inUse.TryDequeue(out _);
                countActive--;
                if (available.Count >= effectDef.maxPoolSize) {
                    Object.Destroy(effect.gameObject);
                    return;
                }
                effect.gameObject.SetActive(false);
                available.Push(effect);
                returnTimes.EnqueueBack(Time.time);
                effect.released = true;
            }
        }
    }
}
