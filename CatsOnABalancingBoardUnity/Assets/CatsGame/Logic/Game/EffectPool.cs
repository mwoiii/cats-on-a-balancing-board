using UnityEngine;
using UnityEngine.Pool;

namespace OMC {
    public class EffectPool : MonoBehaviour {

        // it would be gangster if we removed effects that weren't used after a long time

        public GameObject effectPrefab;

        public int maxConcurrentEffects = 100;

        private ObjectPool<GameObject> sourcePool;

        private void Awake() {
            sourcePool = new ObjectPool<GameObject>(CreateNewEffect, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, maxConcurrentEffects, maxConcurrentEffects);
        }

        private GameObject CreateNewEffect() {
            GameObject effect = Instantiate(effectPrefab);
            effect.transform.SetParent(transform);
            effect.GetComponent<ReturnToEffectPool>().pool = sourcePool;
            return effect;
        }

        private void OnTakeFromPool(GameObject effect) {
            effect.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(GameObject effect) {
            effect.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(GameObject effect) {
            Destroy(effect.gameObject);
        }


        public void PlaceEffectAt(Vector3 pos) {
            if (sourcePool.CountActive >= maxConcurrentEffects) {
                return;
            }

            GameObject effect = sourcePool.Get();
            effect.transform.position = pos;
        }
    }
}
