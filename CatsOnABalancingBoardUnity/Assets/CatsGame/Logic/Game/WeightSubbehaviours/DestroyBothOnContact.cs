using UnityEngine;

namespace OMC {
    public class DestroyBothOnContact : WeightSubBehaviourBase {
        void OnCollisionEnter(Collision collision) {
            WeightBehaviour colliderBehaviour = collision.collider.gameObject.GetComponent<WeightBehaviour>();
            if (colliderBehaviour && colliderBehaviour.type != WeightBehaviour.WeightType.Antimatter && weightBehaviour.type == WeightBehaviour.WeightType.Antimatter) {
                Destroy(colliderBehaviour.gameObject);
                Destroy(transform.gameObject);
                DynamicEffectManager.PlaySupernovaAtPosition(transform.position);
            }
        }
    }
}
