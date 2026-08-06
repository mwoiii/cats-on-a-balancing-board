using System.Collections;
using UnityEngine;
using static OMC.WeightBehaviour;

namespace OMC {
    public class CatBehaviour : MonoBehaviour {
        public float moveForce = 1;

        public float centerBiasForce = 0.1f;

        public float reactDistance = 1.5f;

        public float slopeTolerance = 0.05f;

        public float baseDamping = 3;

        public float gripDamping = 10;

        public float gripTimeMin = 2;

        public float gripTimeMax = 8;

        public float gripCooldown = 5;

        Transform board;

        BoardMath boardMath;

        Rigidbody body;

        bool preventGripping = false;

        bool gripping = false;

        bool canGrip = true;

        void Start() {
            body = GetComponent<Rigidbody>();

            GameObject boardObject = GameObject.FindGameObjectWithTag("Board");
            board = boardObject.transform;
            boardMath = boardObject.GetComponent<BoardMath>();

            body.linearDamping = baseDamping;
        }

        void FixedUpdate() {
            SurvivalInstinct();

            GameObject[] weights = FindNearestWeights(); // 0 basic, 1 catnip, 2 lemon

            //if (target == null){CenterBias(); return;}

            //Vector3 toTarget = target.position - body.transform.position;
            //toTarget.y = 0;
            //if (toTarget.sqrMagnitude <= 0){CenterBias(); return;}

            //Vector3 dir = toTarget.normalized;

            if (weights[0] != null) {
                Vector3 toTarget = weights[0].transform.position - transform.position;
                toTarget.y = 0;
                if (toTarget.sqrMagnitude <= 0) { CenterBias(); return; }

                if (WeightDropper.weightBehaviourDict[weights[0]].State == WeightState.Falling) // repelled by falling weights
                {
                    body.AddForce(toTarget.normalized * -moveForce, ForceMode.Acceleration);
                }
            }
            if (weights[1] != null) {
                Vector3 toTarget = weights[1].transform.position - transform.position;
                toTarget.y = 0;
                if (toTarget.sqrMagnitude <= 0) { CenterBias(); return; }

                body.AddForce(toTarget.normalized * moveForce, ForceMode.Acceleration);
            }
            if (weights[2] != null) {
                Vector3 toTarget = weights[2].transform.position - transform.position;
                toTarget.y = 0;
                if (toTarget.sqrMagnitude <= 0) { CenterBias(); return; }

                body.AddForce(toTarget.normalized * -moveForce, ForceMode.Acceleration);
            }
        }

        void CenterBias() {
            Vector3 toCenter = board.position - transform.position;
            if (toCenter.sqrMagnitude > 0) {
                body.AddForce(centerBiasForce * toCenter.normalized, ForceMode.Acceleration);
            }

        }

        GameObject[] FindNearestWeights() // does not account for y distance
        {
            GameObject nearestWeight = null;
            float weightWinner = reactDistance;
            GameObject nearestCatnip = null;
            float catnipWinner = Mathf.Infinity;
            GameObject nearestLemon = null;
            float lemonWinner = reactDistance;

            foreach (GameObject w in WeightDropper.weightBehaviourDict.Keys) {
                float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(w.transform.position.x, w.transform.position.z));
                if (dist < weightWinner && WeightDropper.weightBehaviourDict[w].type == WeightType.None) {
                    nearestWeight = w;
                    weightWinner = dist;
                }
                if (dist < catnipWinner && WeightDropper.weightBehaviourDict[w].type == WeightType.Catnip) {
                    nearestCatnip = w;
                    catnipWinner = dist;
                }
                if (dist < lemonWinner && WeightDropper.weightBehaviourDict[w].type == WeightType.Lemon) {
                    nearestLemon = w;
                    lemonWinner = dist;
                }
            }
            return new GameObject[3] { nearestWeight, nearestCatnip, nearestLemon };
        }

        void SurvivalInstinct() {
            if (preventGripping) {
                return;
            }
            if (boardMath.slope > slopeTolerance) {
                if (canGrip && !gripping) {
                    body.linearDamping = gripDamping;
                    gripping = true;
                    canGrip = false;
                    StartCoroutine(Grip());
                }
            } else {
                body.linearDamping = baseDamping;
                gripping = false;
                canGrip = true;
                StopCoroutine(Grip());
            }
        }

        IEnumerator Grip() {
            yield return new WaitForSeconds(Random.Range(gripTimeMin, gripTimeMax));
            body.linearDamping = baseDamping;
            gripping = false;
        }

        void OnCollisionExit(Collision collision) // fall off the board without damping
        {
            if (collision.collider.CompareTag("Board")) {
                preventGripping = true;
                body.linearDamping = 0;
            }
        }
    }
}
