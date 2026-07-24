using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CatSway : MonoBehaviour
{
    public float minInterval = 1.5f;
    public float maxInterval = 4f;
    public float minForce = 2f;
    public float maxForce = 5f;

    Rigidbody rb;
    float timer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        timer = Random.Range(minInterval, maxInterval);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
            rb.AddForce(dir * Random.Range(minForce, maxForce), ForceMode.Impulse);
            timer = Random.Range(minInterval, maxInterval);
        }
    }
}