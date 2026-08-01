using UnityEngine;

public class BillboardSpriteScript : MonoBehaviour {
    void LateUpdate() {
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
