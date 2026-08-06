using System.Collections;
using UnityEngine;

namespace OMC {
    public class WeightBehaviour : MonoBehaviour {

        // we could probably do Something to make this kind of dynamic but for another time
        public enum WeightType {
            None,
            Catnip,
            Lemon,
            Antimatter,
            Indecisive
        }

        [HideInInspector]
        public WeightType type = WeightType.None;

        public enum WeightState { Falling, Landed }

        public WeightState State { get; private set; } = WeightState.Falling;

        private float shrinkAmount = 0.05f;

        private float minScale = 0.01f;

        private string catTag = "Cat";

        private float shrinkInterval = 0.1f; // seconds between shrink ticks

        private float shrinkIntervalLemon = 0.8f;

        private float shrinkTimer = 0f;

        private int indecisiveWarpTime = 1;

        void Update() {
            if (shrinkTimer > 0f) {
                shrinkTimer -= Time.deltaTime;
            }
        }

        void OnCollisionEnter(Collision collision) {
            if (State == WeightState.Falling) {
                State = WeightState.Landed;

                switch (type) {
                    case WeightType.Lemon:
                        StartCoroutine(Decay());
                        break;
                    case WeightType.Indecisive:
                        StartCoroutine(Decay());
                        StartCoroutine(IndecisiveWarp());
                        break;
                }
            }
            WeightBehaviour colliderBehaviour = collision.collider.gameObject.GetComponent<WeightBehaviour>();
            if (colliderBehaviour) {
                if (colliderBehaviour.type == WeightType.Antimatter && type != WeightType.Antimatter) {
                    Destroy(colliderBehaviour.gameObject);
                    Destroy(transform.gameObject);
                    EffectController.instance.PlaySupernovaAtPosition(transform.position);
                }
            }
        }

        IEnumerator IndecisiveWarp() {
            yield return new WaitForSeconds(indecisiveWarpTime);
            while (transform != null) {
                Vector2 a = UnityEngine.Random.insideUnitCircle * 3;
                transform.position = new Vector3(a.x, 3, a.y);
                yield return new WaitForSeconds(indecisiveWarpTime);
            }
        }

        IEnumerator Decay() {
            while (gameObject != null) {
                ShrinkAndCheck();
                yield return new WaitForSeconds(shrinkIntervalLemon);
            }
        }

        void OnCollisionStay(Collision collision) {
            if (type != WeightType.Catnip) return;
            if (!collision.collider.CompareTag(catTag)) return;
            if (shrinkTimer > 0f) return; // still on cooldown

            ShrinkAndCheck();
            shrinkTimer = shrinkInterval;
        }

        private void ShrinkAndCheck() {
            Vector3 newScale = transform.localScale - Vector3.one * shrinkAmount;
            transform.localScale = newScale;

            if (newScale.x < minScale || newScale.y < minScale || newScale.z < minScale) {
                Destroy(gameObject);
            }
        }

        void OnDestroy() {
            WeightDropper.weightBehaviourDict.Remove(transform.gameObject);
        }

        public void NotifyCatContact() {
            if (type != WeightType.Catnip) { return; }
            if (shrinkTimer > 0) { return; }

            ShrinkAndCheck();
            shrinkTimer = shrinkInterval;
        }
    }
}
