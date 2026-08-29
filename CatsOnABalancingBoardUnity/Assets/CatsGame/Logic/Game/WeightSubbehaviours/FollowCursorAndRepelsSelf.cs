using NUnit.Framework;
using OMC;
using System.Linq;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class FolowCursorAndRepelsSelf : WeightSubBehaviourBase {
    public WeightBehaviour.WeightType[] toRepel;

    public float followForce = 5;
    public float repelVel = 1;

    public float radius = 1;

    Rigidbody body;

    public override void Start()
    {
        base.Start();
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        foreach (var (obj, b) in WeightDropper.weightBehaviourDict) {
            if (b.state == WeightBehaviour.WeightState.Landed) {
                float3 toTarget = transform.position - obj.transform.position;

                float x = math.length(toTarget);
                if (x > 0.1f && x < radius) {
                    if (toRepel.Contains(b.type)) {
                        obj.GetComponent<Rigidbody>().AddForce(math.normalize(toTarget) * -repelVel, ForceMode.VelocityChange);
                    }
                }
            }
        }

        if (weightBehaviour.state == WeightBehaviour.WeightState.Landed && WeightDropper.instance)
            {
                UnityEngine.Vector3 toIndicator = WeightDropper.instance.shadowPos - transform.position;
                toIndicator.y = 0;

                if (toIndicator.magnitude > 0.1f)
                {
                    body.AddForce(followForce * Time.fixedDeltaTime * toIndicator.normalized, ForceMode.VelocityChange);
                }
            }
    }
}