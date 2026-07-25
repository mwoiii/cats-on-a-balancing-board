using System.Collections;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    public CatManagerScript catManager;

    public float deletionDelay = 1;
    
    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Wait(collision.collider));
    }

    IEnumerator Wait(Collider other)
    {
        yield return new WaitForSeconds(deletionDelay);
        if (other != null)
        {
            if (other.gameObject.CompareTag("Cat"))
        {
            catManager.RemoveCat(other.gameObject);
        }
        Destroy(other.gameObject);
        }
    }
}