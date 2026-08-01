using UnityEngine;

public class BoardMath : MonoBehaviour {
    public float slope { get; private set; }

    public Vector3 slopeDir { get; private set; }

    void FixedUpdate() {
        Vector3 A = transform.up;
        A.y = 0;
        slope = A.magnitude;
        slopeDir = A.normalized;
    }
}
