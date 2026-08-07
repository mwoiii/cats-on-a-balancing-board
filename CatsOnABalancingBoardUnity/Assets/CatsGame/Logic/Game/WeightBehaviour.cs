using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace OMC {
    public class WeightBehaviour : MonoBehaviour {

        // we could probably do Something to make this kind of dynamic but for another time // The Burst Compiler forbids it my liege
        public enum WeightType {
            None,
            Catnip,
            Lemon,
            Antimatter,
            Indecisive,
            BoardGlue,
            BlueRaspberry,
            FreeWill,
            Whirlpool
        }

        [HideInInspector]
        public WeightType type = WeightType.None;

        public enum WeightState { Falling, Landed }

        [HideInInspector]
        public WeightState State = WeightState.Falling;

        private float shrinkAmount = 0.05f;

        private float minScale = 0.01f;

        private string catTag = "Cat";

        private float shrinkInterval = 0.1f; // seconds between shrink ticks

        private float shrinkTimer = 0f;

        void Update() {
            if (shrinkTimer > 0f) {
                shrinkTimer -= Time.deltaTime;
            }
        }

        void OnCollisionEnter(Collision collision) {
            if (State == WeightState.Falling) {
                State = WeightState.Landed;
            }
        }

        void OnCollisionStay(Collision collision) {
            if (type != WeightType.Catnip) return;
            if (!collision.collider.CompareTag(catTag)) return;
            if (shrinkTimer > 0f) return; // still on cooldown

            ShrinkAndCheck();
            shrinkTimer = shrinkInterval;
        }

        public void ShrinkAndCheck() {
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
