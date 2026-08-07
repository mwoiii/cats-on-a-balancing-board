using System.Collections;
using UnityEngine;

namespace OMC {
    public class DecayOverTime : WeightSubBehaviourBase {
        public float shrinkDelay = 0f;

        public float shrinkInterval = 0.8f;

        bool decaying = false;

        void OnCollisionEnter(Collision collision) {
            StartCoroutine(Decay());
        }

        IEnumerator Decay() {
            if (!decaying) {
                decaying = true;
                yield return new WaitForSeconds(shrinkDelay);
                while (gameObject) {
                    weightBehaviour.ShrinkAndCheck();
                    yield return new WaitForSeconds(shrinkInterval);
                }
                decaying = false;
            }
        }
    }
}
