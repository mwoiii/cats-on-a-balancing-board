using Assets.CatsGame.Logic.Game;
using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class KillPlane : MonoBehaviour {
    public CatManagerScript catManager;

    public float deletionDelay = 1;

    public float bounceMultiplier = 5;

    private EntityManager entityManager;

    private void Start() {
        entityManager = StaticEntityData.entityManager;
    }

    private void OnCollisionEnter(Collision collision) {
        Rigidbody b = collision.rigidbody;
        if (b != null) {
            Vector3 norm = collision.GetContact(0).normal;
            float impactSpeed = collision.relativeVelocity.magnitude;
            b.linearVelocity += -bounceMultiplier * impactSpeed * norm;
            b.linearVelocity += bounceMultiplier * 0.1f * b.position; // Sorry
        }
        StartCoroutine(Wait(collision.collider));
    }

    IEnumerator Wait(Collider other) {
        yield return new WaitForSeconds(deletionDelay);
        if (other != null) {
            Entity explosion = entityManager.Instantiate(StaticEntityData.effectConfig.explosionPrefab);
            entityManager.SetComponentData(explosion, new LocalTransform { Position = other.transform.position, Scale = 0.2f });
            AudioPool.instance.PlayExplosionSoundAt(other.transform.position);
            if (other.gameObject.CompareTag("Cat")) {
                catManager.RemoveCat(other.gameObject);
            }
            Destroy(other.gameObject);
        }
    }
}
