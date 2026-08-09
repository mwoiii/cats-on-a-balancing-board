using UnityEngine;

namespace OMC {
    public class ReverseOnLanding : WeightSubBehaviourBase {
        GameObject board;

        Rigidbody boardBody;

        public float strength = 1f;

        bool correcting = false;

        bool correctingLock = false;

        Vector3 targetUp;

        public AudioSource source;
        public AudioClip clip;
        public float volume = 0.5f;

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

            Vector3 currentUp = board.transform.up;
            targetUp = new Vector3(-currentUp.x,currentUp.y,-currentUp.z);

            boardBody.angularVelocity = Vector3.zero;
            correcting = true;
            
            if (source && clip)
            {
                source.clip = clip;
                source.volume = volume;
                source.Play();
            }
        }

        void FixedUpdate() {
            if (correcting && !correctingLock) {
                Quaternion correction = Quaternion.FromToRotation(board.transform.up, targetUp);
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

