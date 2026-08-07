using UnityEngine;

public class StickOnLanding : WeightSubBehaviourBase {
    void OnCollisionEnter(Collision collision) {
        var gloop = gameObject.AddComponent<FixedJoint>();
        gloop.connectedBody = collision.rigidbody;
        gloop.breakForce = 10000;
        gloop.breakTorque = 10000;
    }
}
