using System;
using UnityEngine;


namespace OMC {
    public class PooledEffect : MonoBehaviour {
        [NonSerialized]
        public EffectPool pool;

        public float effectLifetime = 1f;

        [NonSerialized]
        public bool released;

        private float effectCountdown;

        private void OnEnable() {
            effectCountdown = effectLifetime;
        }

        private void Update() {
            effectCountdown -= Time.deltaTime;
            if (effectCountdown <= 0f) {
                pool.Release(this);
            }
        }
    }
}
