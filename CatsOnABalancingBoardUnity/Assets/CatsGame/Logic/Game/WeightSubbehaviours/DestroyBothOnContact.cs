using UnityEngine;

namespace OMC {
    public class DestroyBothOnContact : WeightSubBehaviourBase {
        void OnCollisionEnter(Collision collision) {
            WeightBehaviour colliderBehaviour = collision.collider.gameObject.GetComponent<WeightBehaviour>();
            if (colliderBehaviour && colliderBehaviour.type != weightBehaviour.type) {
                if(colliderBehaviour.type != WeightBehaviour.WeightType.Matter){ // unique matter interaction
                    Destroy(colliderBehaviour.gameObject);
                }
                Destroy(transform.gameObject);
                OMCEffectSpawner.PlaySupernovaAtPosition(transform.position);
            }
        }
    }
}
