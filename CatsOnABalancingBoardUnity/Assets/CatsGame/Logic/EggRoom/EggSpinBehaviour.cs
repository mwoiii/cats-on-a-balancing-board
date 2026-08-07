using UnityEngine;

public class EggSpinBehaviour : MonoBehaviour {
    public Transform previewPivot;

    public float spinSpeed = 60f;

    void Update() {
        if (previewPivot) {
            previewPivot.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);
        }
    }
}
