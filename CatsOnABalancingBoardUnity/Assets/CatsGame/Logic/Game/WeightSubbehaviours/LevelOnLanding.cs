using UnityEngine;

namespace OMC {
    public class LevelOnLanding : WeightSubBehaviourBase {
        GameObject board;

        Rigidbody boardBody;

        public float strength = 10f;

        bool correcting = false;

        bool correctingLock = false;

        public override void Start() {
            base.Start();
            board = BoardController.boardInstance;
            boardBody = board.GetComponent<Rigidbody>();
        }

        void OnCollisionEnter(Collision collision) {
            if (!collision.collider.CompareTag("Board")) {
                return;
            }

            if (correcting || correctingLock) {
                return;
            }

            boardBody.angularVelocity = Vector3.zero;
            correcting = true;
        }

        void FixedUpdate() {
            if (correcting && !correctingLock) {
                Quaternion correction = Quaternion.FromToRotation(board.transform.up, Vector3.up);
                correction.ToAngleAxis(out float angle, out Vector3 axis);

                if (angle > 180) {
                    angle -= 360;
                }

                if (angle < 1) {
                    correcting = false;
                    correctingLock = true;
                    return;
                }

                boardBody.AddTorque(angle * Mathf.Deg2Rad * strength * axis.normalized, ForceMode.VelocityChange);
            }
        }
    }
}
