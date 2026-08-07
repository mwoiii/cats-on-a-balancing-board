using UnityEngine;

namespace OMC {
    public class RandomDrop : WeightSubBehaviourBase {
        public override void Start() {
            base.Start();

            float boardRadius = BoardController.boardInstance.GetComponent<Collider>().bounds.extents.x;
            Vector2 randomPos = Random.insideUnitCircle * boardRadius;
            transform.position = new Vector3(randomPos.x, 3, randomPos.y);
        }
    }
}
