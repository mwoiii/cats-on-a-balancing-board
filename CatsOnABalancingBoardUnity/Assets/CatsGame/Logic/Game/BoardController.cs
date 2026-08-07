using UnityEngine;

namespace OMC {
    public class BoardController : MonoBehaviour {
        public static GameObject boardInstance;

        public float slope { get; private set; }

        public Vector3 slopeDir { get; private set; }

        private void Awake() {
            boardInstance = gameObject;
        }

        void FixedUpdate() {
            Vector3 A = transform.up;
            A.y = 0;
            slope = A.magnitude;
            slopeDir = A.normalized;
        }
    }
}
