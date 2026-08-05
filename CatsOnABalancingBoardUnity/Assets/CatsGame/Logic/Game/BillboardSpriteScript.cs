using UnityEngine;

namespace OMC {
    public class BillboardSpriteScript : MonoBehaviour {
        void LateUpdate() {
            transform.LookAt(transform.position + Camera.main.transform.forward);
        }
    }
}
