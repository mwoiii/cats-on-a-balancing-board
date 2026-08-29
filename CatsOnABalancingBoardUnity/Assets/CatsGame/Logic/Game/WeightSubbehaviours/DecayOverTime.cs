using System;
using System.Collections;
using UnityEngine;

namespace OMC {
    public class DecayOverTime : WeightSubBehaviourBase {
        public float shrinkDelay = 0f;

        public float shrinkInterval = 0.8f;

        public bool useRealTime = false;

        public event System.Action DecayStart;

        Coroutine delay;

        void OnCollisionEnter(Collision collision) {
            if (delay == null)
            {
                delay = StartCoroutine(Decay());
            }
        }

        IEnumerator Decay() {
            yield return useRealTime ? new WaitForSecondsRealtime(shrinkDelay) : new WaitForSeconds(shrinkDelay);
            DecayStart?.Invoke();
            while (gameObject) {
                weightBehaviour.ShrinkAndCheck();
                yield return useRealTime ? new WaitForSecondsRealtime(shrinkInterval) : new WaitForSeconds(shrinkInterval);
            }
        }
    }
}
