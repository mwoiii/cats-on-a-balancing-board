using UnityEngine;
using OMC;

public class DestroyBothOnContact : WeightSubbehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        WeightBehaviour colliderBehaviour = collision.collider.gameObject.GetComponent<WeightBehaviour>();
        if (colliderBehaviour) {
            if (colliderBehaviour.type != WeightBehaviour.WeightType.Antimatter && weightBehaviour.type == WeightBehaviour.WeightType.Antimatter) {
                Destroy(colliderBehaviour.gameObject);
                Destroy(transform.gameObject);
                EffectController.instance.PlaySupernovaAtPosition(transform.position);
            }
        }
    }
}
