using UnityEngine;

public class BillboardSpriteScript : MonoBehaviour
{
    void LateUpdate()
    {
        // Face the sprite toward the camera
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
