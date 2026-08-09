using UnityEngine;

public class StickOnLanding : WeightSubBehaviourBase {
    public AudioSource source;
    public AudioClip clip;
    public float volume = 0.5f;
    void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.CompareTag("Board")){return;} // get it
        
        var gloop = gameObject.AddComponent<FixedJoint>();
        gloop.connectedBody = collision.rigidbody;
        gloop.breakForce = 10000;
        gloop.breakTorque = 10000;
        if (source && clip)
        {
            source.clip = clip;
            source.volume = volume;
            source.time = 0.2f;
            source.Play();
        }
    }
}
