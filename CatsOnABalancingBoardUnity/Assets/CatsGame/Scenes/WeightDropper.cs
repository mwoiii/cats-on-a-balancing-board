using UnityEngine;
using UnityEngine.InputSystem;

public class WeightDropper : MonoBehaviour
{
    public GameObject weightPrefab;

    void Start()
    {
        
    }

    void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame){
            Debug.Log("Hi");
            Instantiate(weightPrefab, transform.position, Quaternion.identity);
        }
    }
}
