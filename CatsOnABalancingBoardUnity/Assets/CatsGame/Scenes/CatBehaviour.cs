using System.Linq;
using UnityEngine;

public class CatBehaviour : MonoBehaviour
{
    public float moveForce = 1;

    public float reactDistance = 1;
    
    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
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

        WeightBehaviour ba = target.GetComponent<WeightBehaviour>();
        if (ba.State == WeightBehaviour.WeightState.Falling) // repelled by falling weights
        {
            body.AddForce(dir * -moveForce, ForceMode.Acceleration);
        }
        else if (ba.Type == WeightBehaviour.WeightType.Catnip) // attracted to catnip weights
        {
            body.AddForce(dir * moveForce, ForceMode.Acceleration);
        }
        
    }

    Transform FindNearestWeight() // does not account for y distance
    {
        GameObject[] weights = GameObject.FindGameObjectsWithTag("Weight");
        if (weights.Length == 0){return null;}

        Transform nearest = null;
        float winner = reactDistance;
        
        foreach (GameObject w in weights)
        {
            float dist = Vector2.Distance(new Vector2(transform.position.x,transform.position.z),new Vector2(w.transform.position.x,w.transform.position.z));
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
