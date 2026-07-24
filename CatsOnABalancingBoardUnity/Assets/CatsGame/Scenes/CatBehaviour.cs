using UnityEngine;

public class CatBehaviour : MonoBehaviour
{
    public float climbForce = 5f;

    public float lookAheadDist = 0.3f;
    public float edgeCheckDist =  1f;

    Rigidbody body;
    Transform board;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        board = GameObject.FindGameObjectWithTag("Board").transform;
    }

    void FixedUpdate()
    {
        Vector3 up = board.up;
        Vector3 uphill = new Vector3(-up.x,0,-up.z);

        if (WouldRunOffEdge(uphill))
        {
            Vector3 vel = body.linearVelocity;
            body.linearVelocity = new Vector3(0,vel.y,0);
            return;
        }

        //uphill.Normalize();
        body.AddForce(uphill * climbForce, ForceMode.Acceleration);
    }

    bool WouldRunOffEdge(Vector3 dir)
    {
        if (dir.sqrMagnitude <= 0) { return false;}

        Vector3 checkpoint = body.transform.position + dir.normalized * lookAheadDist;
        checkpoint.y += 0.5f;

        return !Physics.Raycast(checkpoint, Vector3.down,edgeCheckDist);
    }
}
