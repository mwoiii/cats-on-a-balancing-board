using UnityEngine;

public class KillPlane : MonoBehaviour
{
    public CatManagerScript catManager;
    
    private void OnTriggerEnter(Collider other)
    {
        //check if object is in cat layer
        if (other.gameObject.CompareTag("Cat"))
        {
            catManager.RemoveCat(other.gameObject);
        }
        Destroy(other.gameObject);
    }
}