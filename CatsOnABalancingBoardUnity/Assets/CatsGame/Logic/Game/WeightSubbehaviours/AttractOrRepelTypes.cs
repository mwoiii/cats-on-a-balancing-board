using OMC;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class AttractOrRepelTypes : WeightSubBehaviourBase {
    public WeightBehaviour.WeightType[] toAttract;
    public WeightBehaviour.WeightType[] toRepel;

    public float attractForce = 5;
    public float repelVel = 1;

    public float radius = 1;

    void FixedUpdate() {
        foreach (var (obj, b) in WeightDropper.weightBehaviourDict) {
            if (b.state == WeightBehaviour.WeightState.Landed) {
                float3 toTarget = transform.position - obj.transform.position;

                float x = math.length(toTarget);
                if (x > 0.1f && x < radius) {
                    if (toRepel.Contains(b.type)) {
                        obj.GetComponent<Rigidbody>().AddForce(math.normalize(toTarget) * -repelVel, ForceMode.VelocityChange);
                    } else if (toAttract.Contains(b.type)) {
                        obj.GetComponent<Rigidbody>().AddForce(math.normalize(toTarget) * attractForce, ForceMode.Acceleration); // add  * x/radius for inverse proportional instead of constant
                    }
                }
            }
        }
    }
}
