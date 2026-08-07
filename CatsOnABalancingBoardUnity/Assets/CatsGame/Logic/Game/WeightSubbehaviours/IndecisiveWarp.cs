using System.Collections;
using UnityEngine;

namespace OMC {
    public class IndecisiveWarp : WeightSubBehaviourBase {
        public int warpCount = 4;
        public int warpTime = 1;
        bool indecisiving = false;

        public float selfRadiusRelative = 1 / 3;
        float boardRadius = 3;
        float selfRadius;

        public override void Start() {
            base.Start();
            if (BoardController.boardInstance && BoardController.boardInstance.TryGetComponent(out Collider boardCollider)) {
                boardRadius = boardCollider.bounds.extents.x;
            } else {
                Debug.LogWarning("Hello Indecisive Cube reporting for duty! The board doesn't have a collider component");
            }
            selfRadius = boardRadius * selfRadiusRelative;
        }

        void OnCollisionEnter(Collision collision) {
            StartCoroutine(Warp());
        }

        IEnumerator Warp() {
            if (!indecisiving) {
                indecisiving = true;

                yield return new WaitForSeconds(warpTime);
                for (int i = 0; i < warpCount; i++) {
                    if (transform) {
                        Vector2 startPos = new(transform.position.x, transform.position.z);
                        Vector2 tpMovement = Random.insideUnitCircle * boardRadius;
                        // this can cause an infinite loop and crashes the game if startpos is far out enough
                        //while (Vector2.Distance(startPos, tpMovement) > selfRadius) {
                        //    tpMovement = Random.insideUnitCircle * boardRadius;
                        //}
                        transform.position = new Vector3(tpMovement.x, 3, tpMovement.y);
                        weightBehaviour.state = WeightBehaviour.WeightState.Falling;
                        yield return new WaitForSeconds(warpTime);
                    }
                }
                indecisiving = false;
            }
        }
    }
}
