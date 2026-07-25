using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    public CatManagerScript catManager;

    public float deletionDelay = 1;

    public float bounceMultiplier = 5;
    
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody b = collision.rigidbody;
        if (b != null)
        {
            Vector3 norm = collision.GetContact(0).normal;
            float impactSpeed = collision.relativeVelocity.magnitude;
            b.linearVelocity += -bounceMultiplier * impactSpeed * norm;
            b.linearVelocity += bounceMultiplier * 0.1f * b.position; // Sorry
        }
        StartCoroutine(Wait(collision.collider));
    }

    IEnumerator Wait(Collider other)
    {
        yield return new WaitForSeconds(deletionDelay);
        if (other != null)
        {
            ExplosionEffect.Instance.PlayAt(other.transform.position);
            if (other.gameObject.CompareTag("Cat"))
            {
                catManager.RemoveCat(other.gameObject);
            }
            Destroy(other.gameObject);
        }
    }
}