using UnityEngine;

public class CatBehaviour : MonoBehaviour
{
    public float moveForce = 1f;
    public float ejectDistance = 0.01f;
    
    Rigidbody body;
    float catRadius;
    int overlapMask;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        catRadius = GetComponent<Collider>().bounds.extents.y;
        overlapMask = ~LayerMask.GetMask("Cat");
    }

    void FixedUpdate()
    {
        if (CheckCovered()){return;}

        Transform target = FindNearestWeight();
        if (target == null){return;}

        Vector3 toTarget = target.position - body.transform.position;
        toTarget.y = 0;
        if (toTarget.sqrMagnitude <= 0){return;}

        Vector3 dir = toTarget.normalized;

        body.AddForce(dir * moveForce, ForceMode.Acceleration);
    }

    Transform FindNearestWeight()
    {
        GameObject[] weights = GameObject.FindGameObjectsWithTag("Catnip");
        if (weights.Length == 0){return null;}

        Transform nearest = null;
        float winner = Mathf.Infinity;
        
        foreach (GameObject w in weights)
        {
            float dist = Vector3.Distance(body.transform.position,w.transform.position);
            if (dist < winner)
            {
                winner = dist;
                nearest = w.transform;
            }
        }
        return nearest;
    }

    bool CheckCovered()
    {
        return false;
    }
}
