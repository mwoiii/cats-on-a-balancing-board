using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeightDropper : MonoBehaviour
{
    public GameObject[] weightPrefabs;
    public GameObject shadowPrefab;
    public Transform board;
    public float moveSpeed = 1f;
    public float surfaceOffset = 0.01f;
    public float dropHeight = 5f;
    public float shadowBoundRadius = 5f;
    
    public float spinSpeed = 180f; // degrees/sec
    public float shadowScale = 0.5f;
    public GameObject nextPrefab {get; private set;}

    float spinAngle;
    GameObject shadow;

    public System.Action<GameObject> OnNextPrefab;

    void Start()
    {
        shadow = Instantiate(shadowPrefab, board.position + board.up * surfaceOffset, board.rotation);
        shadow.transform.localScale = Vector3.one * shadowScale;
        shadow.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        shadow.transform.SetParent(board);

        nextPrefab = weightPrefabs[Random.Range(0,weightPrefabs.Length)];
        OnNextPrefab?.Invoke(nextPrefab);
        spinAngle = 0f;
    }

    void Update()
    {
        Vector2 input = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f; 
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;
        if (input.sqrMagnitude > 0f)
            shadow.transform.localPosition += new Vector3(input.x, 0f, input.y).normalized * moveSpeed * Time.deltaTime;
        
        // clamp to board radius
        Vector3 pos = shadow.transform.localPosition;
        Vector2 posXZ = new Vector2(pos.x, pos.z);
        posXZ = Vector2.ClampMagnitude(posXZ, shadowBoundRadius);
        pos.x = posXZ.x;
        pos.z = posXZ.y;
        shadow.transform.localPosition = pos;
        
        spinAngle += spinSpeed * Time.deltaTime;
        shadow.transform.localRotation = Quaternion.Euler(90f, spinAngle, 0f);
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Instantiate(nextPrefab, shadow.transform.position + Vector3.up * dropHeight, Quaternion.identity);
            nextPrefab = weightPrefabs[Random.Range(0,weightPrefabs.Length)];
            OnNextPrefab?.Invoke(nextPrefab);
        }  
    }
    
}