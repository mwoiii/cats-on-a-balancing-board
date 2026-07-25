using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeightDropper : MonoBehaviour
{
    public GameObject[] weightPrefabs;
    public GameObject shadowPrefab;
    public Transform board;
    public float moveSpeed = 5f;
    public float surfaceOffset = 0.01f;
    public float dropHeight = 5f;

    public GameObject nextPrefab {get; private set;}

    GameObject shadow;

    void Start()
    {
        shadow = Instantiate(shadowPrefab, board.position + board.up * surfaceOffset, board.rotation);
        shadow.transform.SetParent(board);

        nextPrefab = weightPrefabs[Random.Range(0,weightPrefabs.Length)];
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

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            Instantiate(nextPrefab, shadow.transform.position + Vector3.up * dropHeight, Quaternion.identity);
            nextPrefab = weightPrefabs[Random.Range(0,weightPrefabs.Length)];
    }
}