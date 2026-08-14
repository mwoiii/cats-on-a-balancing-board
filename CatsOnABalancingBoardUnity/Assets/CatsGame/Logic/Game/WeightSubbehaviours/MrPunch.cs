using OMC;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class MrPunch : WeightSubBehaviourBase {
    GameObject target = null;
    List<GameObject> punched = new();

    Rigidbody body;

    public float punchForce = 100f;
    public float approachForce = 1f;

    public AudioSource source;

    public AudioClip[] clips = new AudioClip[3];
    public AudioClip punch;
    public float volume = 0.5f;

    new void Start() {
        base.Start();
        body = gameObject.GetComponent<Rigidbody>();

        if (source) {
            source.volume = volume;
            source.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            source.Play();
        }
    }

    void FixedUpdate() {
        if (target == null || target.IsDestroyed()) {
            FindTarget();
        } else {
            Vector3 toTarget = target.transform.position - transform.position;
            if (math.length(toTarget) > 0) {
                body.AddForce(math.normalize(toTarget) * approachForce, ForceMode.Force);
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (!collision.collider.gameObject.CompareTag("Board") && collision.collider.attachedRigidbody) // yeah they can punch stuff that isn't the target
        {
            body.linearVelocity = Vector3.zero;
            collision.collider.attachedRigidbody.AddExplosionForce(punchForce, transform.position, 10);
            punched.Add(collision.collider.gameObject);
            target = null;
            if (source && punch) {
                source.clip = punch;
                source.Play();
            }
        }
    }

    void FindTarget() {
        float dist = Mathf.Infinity;
        foreach (var obj in WeightDropper.weightBehaviourDict.Keys) {
            if (obj == gameObject || punched.Contains(obj)) {
                continue;
            }
            float a = math.length(transform.position - obj.transform.position);
            if (a < dist) {
                dist = a;
                target = obj;

                punched.RemoveAll(x => x.IsDestroyed() || x == null); // this goes here because its not every frame but frequent enough
            }
        }
    }
}
