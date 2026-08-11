using UnityEngine;

namespace OMC {
    public class RandomRotation : MonoBehaviour {
        public Vector3 minAngles;
        public Vector3 maxAngles;

        private void OnEnable() {
            transform.localEulerAngles = new Vector3(
                Random.Range(minAngles.x, maxAngles.x),
                Random.Range(minAngles.y, maxAngles.y),
                Random.Range(minAngles.z, maxAngles.z)
            );
        }
    }
}
