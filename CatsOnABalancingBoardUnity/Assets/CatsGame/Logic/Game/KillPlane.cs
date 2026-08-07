using System.Collections;
using UnityEngine;

namespace OMC {
    public class KillPlane : MonoBehaviour {
        public CatManagerScript catManager;

        public float deletionDelay = 1;

        public float bounceMultiplier = 5;

        private void OnCollisionEnter(Collision collision) {
            Rigidbody colliderBody = collision.rigidbody;
            if (colliderBody) {
                Vector3 norm = collision.GetContact(0).normal;
                float impactSpeed = collision.relativeVelocity.magnitude;
                colliderBody.linearVelocity += -bounceMultiplier * impactSpeed * norm;
                colliderBody.linearVelocity += bounceMultiplier * 0.1f * colliderBody.position; // Sorry
            }
            StartCoroutine(Wait(collision.collider));
        }

        IEnumerator Wait(Collider other) {
            yield return new WaitForSeconds(deletionDelay);
            if (other) {
                EffectController.instance.PlayExplosionAtPosition(other.transform.position);
                if (other.gameObject.CompareTag("Cat")) {
                    catManager.RemoveCat(other.gameObject);
                }
                Destroy(other.gameObject);
            }
        }
    }
}
